using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using BestBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BestBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]
public class OrderController : Controller {
    private readonly IUnitOfWork _unitOfWork;
    public OrderController(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index() {
        return View();
    }
    #region API CALLS
    public IActionResult GetAll() {
        IEnumerable<OrderHeader> orderHeaders;
        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) {
        orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
        } else {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
        }
        return Json(new {
            data = orderHeaders
        });
    }
    #endregion
}
