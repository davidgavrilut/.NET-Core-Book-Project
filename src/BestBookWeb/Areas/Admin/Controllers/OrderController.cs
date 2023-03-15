using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BestBookWeb.Areas.Admin.Controllers; 
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
        IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
        return Json(new {
            data = orderHeaders
        });
    }
    #endregion
}
