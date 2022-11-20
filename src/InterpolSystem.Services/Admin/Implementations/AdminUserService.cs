using Org.BouncyCastle.Math.EC.Rfc7748;

namespace InterpolSystem.Services.Admin.Implementations
{
    using AngleSharp.Css;
    using Data;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminUserService : IAdminUserService
    {
        private readonly InterpolDbContext db;

        public AdminUserService(InterpolDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<AdminUserListingServiceModel> All()
        {
            var userRoles = this.db.UserRoles.ToList();
            var roles = this.db.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToList();

            var users = this.db.Users
                .Select(u => new AdminUserListingServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email
                    // RoleNames = roles
                    //.Where(r => userRoles
                    //    .Where(ur => ur.UserId == u.Id)
                    //    .Select(ur => ur.RoleId).Contains(r.Id))
                    // .Select(r => r.Name)
                })
                .ToList()
                .Select(x => new AdminUserListingServiceModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    RoleNames = roles
                    .Where(r => userRoles
                        .Where(ur => ur.UserId == x.Id)
                        .Select(ur => ur.RoleId).Contains(r.Id))
                     .Select(r => r.Name)
                });

            return users;
        }
    }
}
