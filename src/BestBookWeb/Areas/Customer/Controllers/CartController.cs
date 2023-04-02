using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using BestBook.Models.ViewModels;
using BestBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace BestBookWeb.Areas.Customer.Controllers;
[Area("Customer")]
[Authorize]
public class CartController : Controller {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    [BindProperty]
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
    public int OrderTotal { get; set; }
    public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender) {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;

    }
    public IActionResult Index() {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel = new ShoppingCartViewModel() {
            ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
            OrderHeader = new()
        };

        foreach(var cart in ShoppingCartViewModel.ListCart) {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartViewModel);
    }
    public IActionResult Summary() {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel = new ShoppingCartViewModel() {
            ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
            OrderHeader = new()
        };

        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

        ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
        ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
        ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
        ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

        foreach (var cart in ShoppingCartViewModel.ListCart) {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartViewModel);
    }

    [HttpPost]
    [ActionName("Summary")]
    [ValidateAntiForgeryToken]
    public IActionResult SummaryPOST() {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

        ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
        ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;    
        ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

        foreach (var cart in ShoppingCartViewModel.ListCart) {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }
        _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
        _unitOfWork.Save();

        foreach (var cart in ShoppingCartViewModel.ListCart) {
            OrderDetails orderDetails = new() {
                ProductId = cart.ProductId,
                OrderId = ShoppingCartViewModel.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count
            };
            _unitOfWork.OrderDetails.Add(orderDetails);
            _unitOfWork.Save();
        }

        // Stripe Settings
        var domain = "https://localhost:44380/";
        var options = new SessionCreateOptions {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
            CancelUrl = domain+$"customer/cart/index",
        };

        foreach (var item in ShoppingCartViewModel.ListCart) {
            var sessionLineItem = new SessionLineItemOptions {
                PriceData = new SessionLineItemPriceDataOptions {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions {
                        Name = item.Product.Title,
                    },
                },
                Quantity = item.Count,
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = service.Create(options);
        _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult OrderConfirmation(int id) {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
        var service = new SessionService();
        Session session = service.Get(orderHeader.SessionId);
        // check stripe status
        if(session.PaymentStatus.ToLower() == "paid") {
            _unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
            _unitOfWork.Save();
        }
        _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Book Order Confirmed", "<p>A new order has been created at the Best Book Online Store!</p>");
        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        HttpContext.Session.Clear();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();
        return View(id);
    }

    public IActionResult Plus(int cartId) {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }    
    
    public IActionResult Minus(int cartId) {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        if (cart.Count <= 1) {
            _unitOfWork.ShoppingCart.Remove(cart);
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
        } else {
            _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
        }
            _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId) {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Save();
        var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
        HttpContext.Session.SetInt32(SD.SessionCart, count);
        return RedirectToAction(nameof(Index));
    }

    private double GetPriceBasedOnQuantity(int quantity, double price, double price50, double price100) {
        if (quantity <= 50) {
            return price;
        } else if (quantity <= 100) {
            return price50;
        } else {
            return price100;
        }
    }
}
