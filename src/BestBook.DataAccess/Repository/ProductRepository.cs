﻿using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBook.DataAccess.Repository {
    public class ProductRepository : Repository<Product>, IProductRepository {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }

        public void Update(Product obj) {
            var objFromDb = _db.Products.FirstOrDefault(e => e.Id == obj.Id);
            if (objFromDb != null) {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                objFromDb.CoverTypeId = obj.CoverTypeId;
                
                if (obj.ImageUrl != null) {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
