using ECommerce.UserService.DataAccess.Data;
using ECommerce.UserService.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _dbContext;
        public IApplicationUserRepository _applicationUserRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _applicationUserRepository = new ApplicationUserRepository(_dbContext);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }


        public void ClearTracking()
        {
            _dbContext.ChangeTracker.Clear();
        }
    }
}
