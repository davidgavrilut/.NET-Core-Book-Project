using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BestBookWeb.Areas.Customer.Controllers;
[Area("Customer")]
[Authorize]
public class CartController : Controller {
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
    public CartController(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index() {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartViewModel = new ShoppingCartViewModel() {
            ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
        };

        return View(ShoppingCartViewModel);
    }
}
