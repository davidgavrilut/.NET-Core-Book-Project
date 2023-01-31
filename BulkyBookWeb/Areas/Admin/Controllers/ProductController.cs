using BulkyBook.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Controllers {
    [Area("Admin")]
    public class ProductController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public IActionResult Index() {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll();
            return View(objProductList);
        }

        // GET
        public IActionResult Upsert(int? id) {
            if (id == null || id == 0) {
                // create product
            } else {
                // update product
            }
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj) {
            if (ModelState.IsValid) {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) return NotFound();
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);
            if (productFromDb == null) return NotFound();
            return View(productFromDb);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {
            var obj = _unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);
            if (obj != null) {
                _unitOfWork.Product.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction("Index");
            } else {
                return NotFound();
            }
        }
    }
}
