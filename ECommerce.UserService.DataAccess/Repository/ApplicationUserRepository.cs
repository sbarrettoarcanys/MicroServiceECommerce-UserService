using ECommerce.UserService.DataAccess.Data;
using ECommerce.UserService.DataAccess.Repository.IRepository;
using ECommerce.UserService.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _dbContext;
        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        // for deletion set isActive column to false from the business logic layer
        public void Update(ApplicationUser applicationUserModel)
        {
            _dbContext.ApplicationUsers.Update(applicationUserModel);
        }
    }
}
