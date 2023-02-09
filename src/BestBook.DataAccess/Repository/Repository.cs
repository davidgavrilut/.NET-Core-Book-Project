using BestBook.Data;
using BestBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BestBook.DataAccess.Repository {
    public class Repository<T> : IRepository<T> where T : class {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db) {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public void Add(T entity) {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(string? includeProperties = null) {
            IQueryable<T> query = dbSet;
            if (includeProperties != null) {
                foreach (var includeProperty in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null) {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (includeProperties != null) {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProperty);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity) {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity) {
            dbSet.RemoveRange(entity);
        }
    }
}
