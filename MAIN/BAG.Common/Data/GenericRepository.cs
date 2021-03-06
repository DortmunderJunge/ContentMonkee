﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data
{
    public class GenericRepository<TEntity>
         where TEntity : class
    {
        internal Context context;
        internal IDbSet<TEntity> set; //install-package entityframework

        public GenericRepository(Context context)
        {
            this.context = context;
            this.set = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          string includeProperties = "")
        {
            IQueryable<TEntity> query = set;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }


        public virtual IEnumerable<TEntity> Get()
        {
            IQueryable<TEntity> query = set;
            return query.ToList();
        }
        public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = set;
            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        public virtual TEntity GetByID(object id)
        {
            return set.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            set.Add(entity);
        }
        public virtual void Insert(IEnumerable<TEntity> entitis)
        {
            foreach (TEntity e in entitis)
            {
                set.Add(e);
            }
        }



        public virtual void Delete(object id)
        {
            TEntity entityToDelete = set.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(IEnumerable<TEntity> entitysToDelete)
        {
            foreach (var entityToDelete in entitysToDelete)
            {
                Delete(entityToDelete);
            }
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                set.Attach(entityToDelete);
            }
            set.Remove(entityToDelete);
        }

        public virtual void Update(List<TEntity> entitysToUpdate)
        {
            foreach (var entityToUpdate in entitysToUpdate)
            {
                Update(entityToUpdate);
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            set.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> filter)
        {
            return context.Set<TEntity>().SingleOrDefault(filter);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await context.Set<TEntity>().SingleOrDefaultAsync(filter);
        }

        public ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> filter)
        {
            return context.Set<TEntity>().Where(filter).ToList();
        }

        public async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await context.Set<TEntity>().Where(filter).ToListAsync();
        }


        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            return context.Set<TEntity>().Count(filter);
        }

        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await context.Set<TEntity>().CountAsync(filter);
        }
    }
}
