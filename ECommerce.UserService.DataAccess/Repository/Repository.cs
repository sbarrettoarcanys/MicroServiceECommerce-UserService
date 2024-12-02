using ECommerce.UserService.DataAccess.Data;
using ECommerce.UserService.DataAccess.Repository.IRepository;
using ECommerce.UserService.Models;
using ECommerce.UserService.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _dbContext;
        DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();

        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        //add isactive in filter from business logic layer
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.AsNoTracking().FirstOrDefault();

        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.AsNoTracking().ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        public void SaveList<TViewModel>(Expression<Func<T, bool>> getAllFilter,
            List<TViewModel> viewModelList,
            Func<T, TViewModel, bool> compareFilter,
            Func<TViewModel, T> viewModelToModelExpression,
            Action<T, TViewModel> UpdateExistingModelExpression = null,
            Action<T> deleteExpression = null)
             where TViewModel : class
        {
            List<T> existingModels = _dbSet.Where(getAllFilter).AsNoTracking().ToList();

            List<T> newModels = new List<T>();
            List<T> deletedModels = new List<T>();

            existingModels?.ForEach(existingModel =>
            {
                var viewmodel = viewModelList?.FirstOrDefault(viewModel => compareFilter(existingModel, viewModel));
                if (viewmodel == null)
                {
                    deletedModels.Add(existingModel);
                }
            });

            viewModelList?.ForEach(viewModel =>
            {
                var existingModel = existingModels?.FirstOrDefault(model => compareFilter(model, viewModel));
                if (existingModel == null)
                {
                    newModels.Add(viewModelToModelExpression(viewModel));
                }
                else
                {
                    UpdateExistingModelExpression?.Invoke(existingModel, viewModel);
                }
            });


            if (newModels?.Count > 0)
            {
                _dbSet.AddRange(newModels);
            }

            if (deletedModels?.Count > 0)
            {
                if (deleteExpression == null)
                {
                    _dbSet.RemoveRange(deletedModels);
                }
                else
                {
                    foreach (var deleteModel in deletedModels)
                    {
                        deleteExpression(deleteModel);
                    }
                }
            }

            _dbContext.SaveChanges();


        }

    }
}
