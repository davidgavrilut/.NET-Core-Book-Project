using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestBook.Controllers {
    [Area("Admin")]
    public class CategoryController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public IActionResult Index() {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }

        // GET
        public IActionResult Create() {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj) {
            if (obj.Name == obj.DisplayOrder.ToString()) {
                ModelState.AddModelError("name", "The Name cannot be the same as the Display Order");
            }
            if (ModelState.IsValid) {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category added successfully";
                return RedirectToAction("Index");
            } return View(obj);
        }

        // GET
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) return NotFound();
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (categoryFromDb == null) return NotFound();
            return View(categoryFromDb);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj) {
            if (obj.Name == obj.DisplayOrder.ToString()) {
                ModelState.AddModelError("name", "The Name cannot be the same as the Display Order");
            }
            if (ModelState.IsValid) {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) return NotFound();
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (categoryFromDb == null) return NotFound();
            return View(categoryFromDb);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {
            var obj = _unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (obj != null) {
                _unitOfWork.Category.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            } else {
                return NotFound();
            }
        }
    }
}
