using PayLend.Core.DTO.Response.Commom;
using PayLend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Paylend.Business.Core
{
    /// <summary>
    /// Abstract Factory
    /// </summary>
    public interface IActionManager<TEntity>
    {
        SimpleReturnDTO Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> filter);

        void SaveChanges();
    }
}
