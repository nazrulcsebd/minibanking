using Microsoft.EntityFrameworkCore;
using MiniBanking.Core.Models.DTOModels;
using MiniBanking.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MiniBanking.Core.Services
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly MiniBankingDbContext _context;

        public GenericRepository(MiniBankingDbContext context)
        {
            _context = context;
        }

        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Delete(Guid id)
        {
            var entity = Get(id);
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual TEntity Get(Guid id)
        {
            return _context.Set<TEntity>()
                 .AsNoTracking()
                 .FirstOrDefault(e => e.Id == id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            //_context.Entry<TEntity>(entity).Property(x => x.CreatedBy).IsModified = false;
            _context.Entry<TEntity>(entity).Property(x => x.CreatedOn).IsModified = false;
        }

        /// <summary>
        /// Update Particular Entity Property Value Which is supplied as Lamda Expression
        /// </summary>
        /// <param name="entity">The Table Need to Update</param>
        /// <param name="propertiesToUpdate">Which Property Need to Update</param>
        public void Update(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
        {
            _context.Set<TEntity>().Attach(entity);

            foreach (var p in propertiesToUpdate)
            {
                _context.Entry(entity).Property(p).IsModified = true;
            }

            //if (entity.GetType().GetProperty("CreatedBy") != null)
            //{
            //    _context.Entry(entity).Property("CreatedBy").IsModified = false;
            //}
            //if (entity.GetType().GetProperty("CreatedOn") != null)
            //{
            //    _context.Entry(entity).Property("CreatedOn").IsModified = false;
            //}
        }

        public virtual PagedList<TEntity> GetAll(PagingParams pagingParams)
        {
            var query = _context.Set<TEntity>().Skip(pagingParams.PageSize * (pagingParams.PageNumber - 1)).Take(pagingParams.PageSize);
            var count = _context.Set<TEntity>().Count();
            return new PagedList<TEntity>(query, count, 0, pagingParams.PageNumber, pagingParams.PageSize);
        }

    }
}
