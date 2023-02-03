using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestBook.Controllers {
    [Area("Admin")]
    public class CoverTypeController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public IActionResult Index() {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypeList);
        }

        // GET
        public IActionResult Create() {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj) {
            if (ModelState.IsValid) {
                _unitOfWork.CoverType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type added successfully";
                return RedirectToAction("Index");
            } return View(obj);
        }

        // GET
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) return NotFound();
            var coverTypeFromDb = _unitOfWork.CoverType.GetFirstOrDefault(e => e.Id == id);
            if (coverTypeFromDb == null) return NotFound();
            return View(coverTypeFromDb);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj) {
            if (ModelState.IsValid) {
                _unitOfWork.CoverType.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) return NotFound();
            var coverTypeFromDb = _unitOfWork.CoverType.GetFirstOrDefault(e => e.Id == id);
            if (coverTypeFromDb == null) return NotFound();
            return View(coverTypeFromDb);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(e => e.Id == id);
            if (obj != null) {
                _unitOfWork.CoverType.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type deleted successfully";
                return RedirectToAction("Index");
            } else {
                return NotFound();
            }
        }
    }
}
