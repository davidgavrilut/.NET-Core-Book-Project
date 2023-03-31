using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using BestBook.Models.ViewModels;
using BestBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BestBook.Controllers; 
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller {

    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment) {
        _unitOfWork = unitOfWork; 
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index() {
        return View();
    }

    public IActionResult Upsert(int? id) {
        ProductViewModel productViewModel = new() {
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(e => new SelectListItem {
                Text = e.Name,
                Value = e.Id.ToString()
            }),
            CoverTypeList = _unitOfWork.CoverType.GetAll().Select(e => new SelectListItem {
                Text = e.Name,
                Value = e.Id.ToString()
            })
        };
        if (id == null || id == 0) {
            return View(productViewModel);
        } else {
            productViewModel.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productViewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel obj, IFormFile? file) {
        if (ModelState.IsValid) {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null) {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"img\products");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Product.ImageUrl != null) {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath)) {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\img\products\" + fileName + extension;
                }

                if (obj.Product.Id == 0) {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                } else {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
    }

    #region API CALLS
    public IActionResult GetAll() {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return Json(new {
            data = productList
        });
    }

    [HttpDelete]
    public IActionResult Delete(int? id) {
        var obj = _unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);
        if (obj == null) {
            return Json(new {
                success = false,
                message = "Error while deleting"
            });
        }
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath)) {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new {
            success = true,
            message = "Record deleted successfully"
        });
    }
    #endregion
}
