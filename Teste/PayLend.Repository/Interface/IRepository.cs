using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PayLend.Core.Entities;

namespace PayLend.Repository.Interface
{
    public interface IEmailRepository<TEntity> where TEntity: class
    {
        #region new IRepository
        void Add(TEntity obj);
        void Create<T>(IEnumerable<T> entity) where T : class;
        T Create<T>(T entity) where T : class;
        TEntity GetById(int id);
        void Update(TEntity obj);
        void Remove(TEntity obj);
        void Dispose();
        void SaveLogChanges();
        T Get<T>(int id) where T : class;
        void Rollback();
        void Delete<T>(IQueryable<T> entities) where T : class;
        IQueryable<T> FindById<T>(Expression<Func<T, bool>> filter = null, bool noTracking = true) where T : class;
        IQueryable<T> GetAll<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null, bool NoTracking = true) where T : class;
        #endregion

        #region initial IRepository
        //IQueryable<T> FindById<T>(Expression<Func<T, bool>> filter = null, bool noTracking = true) where T : class;
      
        //T Create<T>(T entity) where T : class;
        //void Create<T>(IEnumerable<T> entity) where T : class;
        //T Update<T>(T entity) where T : class;
        //void Delete<T>(T entity) where T : class;
        //void Delete<T>(IQueryable<T> entities) where T : class;
        //void RollBack();
        //void SaveChanges(bool isLogActive=false);
        //T Unchange<T>(T entity) where T : class;
        #endregion
    }
}
