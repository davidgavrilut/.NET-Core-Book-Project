using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using BestBook.Models.ViewModels;
using BestBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BestBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]
public class OrderController : Controller {
    private readonly IUnitOfWork _unitOfWork;
    public OrderViewModel OrderViewModel { get; set; }
    public OrderController(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index() {
        return View();
    }
    public IActionResult Details(int orderId) {
        OrderViewModel = new OrderViewModel() {
            OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(e => e.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetails = _unitOfWork.OrderDetails.GetAll(e => e.Id == orderId, includeProperties: "Product")
        };
        return View(OrderViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderDetails() {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
        orderHeaderFromDb.Name = OrderViewModel.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
        orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;
        if (OrderViewModel.OrderHeader.Carrier != null) {
            orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
        }
        if (OrderViewModel.OrderHeader.TrackingNumber != null) {

        }
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();
        TempData["success"] = "Order details updated successfully";
        return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
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
