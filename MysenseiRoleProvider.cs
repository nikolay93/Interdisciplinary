using InterdisciplinaryDomainModel.Database;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;

namespace Interdisciplinary
{
    public class MysenseiRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get ;
            set ;
            
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Any(un => String.IsNullOrWhiteSpace(un)))
                throw new ArgumentException("Usernames cannot contain username that is null, empty string or whitespace");
            if (roleNames.Any(rn => String.IsNullOrWhiteSpace(rn)))
                throw new ArgumentException("RoleName cannot contain role name that is null, empty string or whitespace");

            using (var db = new MysenseiEntities())
            {
                var users = db.Users.Where(u => usernames.Contains(u.Email)).ToList();
                var roles = db.Roles.Where(r => roleNames.Contains(r.RoleNname)).ToList();

                foreach (var user in users)
                {
                    foreach (var role in roles)
                    {
                        if (!user.Roles.Contains(role))
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public override void CreateRole(string roleName)
        {
            ThrowOnNullOrWhiteSpace(roleName, "roleName");

            if (roleName.Contains(','))
                throw new ArgumentException("roleName should not contain comma (\",\")");
            if (roleName.Length > 50)
                throw new ArgumentException("roleName cannot be longer than 50 characters");

            using (var db = new MysenseiEntities())
            {
                var role = new Role() { RoleNname = roleName };
                db.Roles.Add(role);
                db.SaveChanges();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            ThrowOnNullOrWhiteSpace(roleName, "roleName");

            using (var db = new MysenseiEntities())
            {
                var role = db.Roles.FirstOrDefault(r => r.RoleNname == roleName);

                if (role == null)
                    throw new Exception("Role with given roleName does not exist");
                if (throwOnPopulatedRole && role.Users.Any())
                    throw new ProviderException("Role is populated with users");

                db.Roles.Remove(role);
                db.SaveChanges();
                return true;
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            ThrowOnNullOrWhiteSpace(roleName, "roleName");
            ThrowOnNullOrWhiteSpace(usernameToMatch, "usernameToMatch");

            using (var db = new MysenseiEntities())
            {
                var usernames = db.Users.Where(u => u.Email == usernameToMatch && u.Roles.Any(r => r.RoleNname == roleName)).Select(s => s.Email).OrderBy(o => o).ToArray();
                return usernames;
            }
        }

        public override string[] GetAllRoles()
        {
            using (var db = new MysenseiEntities())
            {
                var roleNames = db.Roles.Select(s => s.RoleNname).OrderBy(o => o).ToArray();
                return roleNames;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            ThrowOnNullOrWhiteSpace(username, "username");

            using (var db = new MysenseiEntities())
            {
                var user = db.Users.Include(u => u.Roles).FirstOrDefault(a => a.Email == username);
                if (user == null)
                    throw new Exception("Given username does not exist");

                return user.Roles.Select(s => s.RoleNname).OrderBy(ʖ => ʖ).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            ThrowOnNullOrWhiteSpace(roleName, "roleName");

            using (var db = new MysenseiEntities())
            {
                var role = db.Roles.Include(i => i.Users).FirstOrDefault(r => r.RoleNname == roleName);

                if (role == null)
                    throw new ProviderException("Specified roleName does not exist");

                return role.Users.Select(s => s.Email).ToArray();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            ThrowOnNullOrWhiteSpace(username, "username");
            ThrowOnNullOrWhiteSpace(roleName, "roleName");

            using (var db = new MysenseiEntities())
            {
                return db.Users.Any(a => a.Email == username && a.Roles.Any(r => r.RoleNname == roleName));
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Any(un => String.IsNullOrWhiteSpace(un)))
                throw new ArgumentException("Usernames cannot contain username that is null, empty string or whitespace");
            if (roleNames.Any(rn => String.IsNullOrWhiteSpace(rn)))
                throw new ArgumentException("RoleName cannot contain role name that is null, empty string or whitespace");

            using (var db = new MysenseiEntities())
            {
                var users = db.Users.Include(i => i.Roles).Where(a => usernames.Contains(a.Email));
                foreach (var user in users)
                {
                    foreach (var roleName in roleNames)
                    {
                        var role = user.Roles.FirstOrDefault(r => r.RoleNname == roleName);
                        if (role != null)
                        {
                            user.Roles.Remove(role);
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public override bool RoleExists(string roleName)
        {
            ThrowOnNullOrWhiteSpace(roleName, "roleName");

            using (var db = new MysenseiEntities())
            {
                return db.Roles.Any(r => r.RoleNname == roleName);
            }
        }
        private void ThrowOnNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            if (String.IsNullOrWhiteSpace(argumentValue))
                throw new ArgumentException(String.Format("{0} cannot be null, epmty string or whitespace", argumentName));
        }
    }
}