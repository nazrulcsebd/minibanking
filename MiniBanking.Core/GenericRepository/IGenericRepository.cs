using MiniBanking.Core.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBanking.Core.Services
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity Get(Guid id);
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(Guid id);
        void SaveChanges();
        PagedList<TEntity> GetAll(PagingParams pagingParams);        
    }
}
