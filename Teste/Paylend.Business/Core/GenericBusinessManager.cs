using Paylend.Business.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PayLend.Repository.IdentityRpository;

namespace PayLend.Business.Core
{
    public abstract class GenericBusinessManager<TEntity> : BusinessManager<TEntity> where TEntity : class, new()
    {
        private readonly IIdentityRepository _repository;

        protected GenericBusinessManager(IIdentityRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public void Create(TEntity entity)
        {
            _repository.Create<TEntity>(entity);
            _repository.SaveChanges(true);
        }
        public void Update(TEntity entity)
        {

        }
        public void Delete(TEntity entity)
        {

        }
        public IEnumerable<TEntity> GetAll()
        {
            return _repository.GetAll<TEntity>().ToList();
        }
        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> filter)
        {
            return _repository.GetAll<TEntity>(filter).ToList();
        }

        public void SaveChanges()
        {

        }
    }
}