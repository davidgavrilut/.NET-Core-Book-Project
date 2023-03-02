﻿using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBook.DataAccess.Repository {
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}