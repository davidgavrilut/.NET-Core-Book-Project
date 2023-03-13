using BestBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBook.DataAccess.Repository.IRepository {
    public interface IOrderDetailsRepository : IRepository<OrderDetails> {
        void Update(OrderDetails obj);
    }
}
