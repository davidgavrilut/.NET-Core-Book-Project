using BulkyBook.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            Product product = new();
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(u => new SelectListItem {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            if (id == null || id == 0) {
                // create product
                ViewBag.CategoryList = CategoryList;
                ViewData["CoverTypeList"] = CoverTypeList;
                return View(product);
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
