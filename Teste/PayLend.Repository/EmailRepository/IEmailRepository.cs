using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PayLend.Repository.EmailRepository
{
    public interface IEmailRepository
    {
        IQueryable<T> FindById<T>(Expression<Func<T, bool>> filter = null, bool NoTracking = true) where T : class;
        IQueryable<T> GetAll<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null, bool NoTracking = true) where T : class;
        T Get<T>(int id) where T : class;
        T Create<T>(T entity) where T : class;
        void Create<T>(IEnumerable<T> entity) where T : class;
        T Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Delete<T>(IQueryable<T> entities) where T : class;
        void RollBack();
        void SaveChanges(bool isLogActive = false);
        T Unchange<T>(T entity) where T : class;
    }
}
