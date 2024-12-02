using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

        public void SaveList<TViewModel>(Expression<Func<T, bool>> getAllFilter,
            List<TViewModel> viewModelList,
            Func<T, TViewModel, bool> compareFilter,
            Func<TViewModel, T> viewModelToModelExpression,
            Action<T, TViewModel> UpdateExistingModelExpression = null,
            Action<T> deleteExpression = null) where TViewModel : class;
    }
}
