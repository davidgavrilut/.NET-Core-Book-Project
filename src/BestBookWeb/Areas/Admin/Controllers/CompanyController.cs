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
public class CompanyController : Controller {

    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork; 
    }

    public IActionResult Index() {
        return View();
    }

    public IActionResult Upsert(int? id) {
        Company company = new();
        if (id == null || id == 0) {
            return View(company);
        } else {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj) {
        if (ModelState.IsValid) {
                if (obj.Id == 0) {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company added successfully";
                } else {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
    }

    #region API CALLS
    public IActionResult GetAll() {
        var companyList = _unitOfWork.Company.GetAll();
        return Json(new {
            data = companyList
        });
    }

    [HttpDelete]
    public IActionResult Delete(int? id) {
        var obj = _unitOfWork.Company.GetFirstOrDefault(e => e.Id == id);
        if (obj == null) {
            return Json(new {
                success = false,
                message = "Error while deleting"
            });
        };
        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new {
            success = true,
            message = "Record deleted successfully"
        });
    }
    #endregion
}
