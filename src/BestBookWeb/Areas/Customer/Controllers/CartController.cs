using Microsoft.AspNetCore.Mvc;

namespace BestBookWeb.Areas.Customer.Controllers;
[Area("Customer")]
public class CartController : Controller {
    public IActionResult Index() {
        return View();
    }
}
