using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using BestBook.Models.ViewModels;
using BestBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BestBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int productId) {
            ShoppingCart cartObj = new() {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart) {
            // Get the user claims object
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Access logged user id (nameidentifier)
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // add id to ApplicationUserId
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(i => i.ApplicationUserId == claim.Value && i.ProductId == shoppingCart.ProductId);
            if (cartFromDb == null) {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                // count number of different items in the cart
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            } else {
               _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}