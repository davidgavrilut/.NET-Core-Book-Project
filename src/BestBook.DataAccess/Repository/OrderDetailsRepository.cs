using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using BestBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBook.DataAccess.Repository {
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository {
        private ApplicationDbContext _db;
        public OrderDetailsRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }

        public void Update(OrderDetails obj) {
            _db.OrderDetails.Update(obj);
        }


    }
}
