using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CustomUserStore.UserManagement
{
    public class CustomUserStore<TUser> : IUserStore<TUser>, IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>, 
        IUserLockoutStore<TUser, string>, IUserTwoFactorStore<TUser, string>//, IQueryableUserStore<TUser> 
        where TUser: CustomUser<string>, IUser<string>
    {

        string storageFile = @"c:\temp\aspnetusers.json";
        List<TUser> users;

        public CustomUserStore()
        {
            if (File.Exists(storageFile))
            {
                string contents = File.ReadAllText(storageFile);
                users = JsonConvert.DeserializeObject<List<TUser>>(contents);
                if (users == null)
                    users = new List<TUser>();
            }
            else
                users = new List<TUser>();
        }

        #region IUserStore implementation

        public Task CreateAsync(TUser user)
        {
            HttpContext ctx = HttpContext.Current;
            users.Add(user);
            saveFileUsers();
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TUser user)
        {
            TUser existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                users.Remove(existingUser);
                saveFileUsers();
            }
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            HttpContext ctx = HttpContext.Current;
            return Task.FromResult<TUser>(users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            HttpContext ctx = HttpContext.Current;
            return Task.FromResult<TUser>(users.FirstOrDefault(u => string.Compare(u.UserName, userName, true) == 0));
        }

        public Task UpdateAsync(TUser user)
        {
            TUser existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
                users.Remove(existingUser);
            users.Add(user);
            saveFileUsers();
            return Task.FromResult(0);
        }

        #endregion

        #region IUserPasswordStore

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            return Task.FromResult<bool>(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentException("user");
            CustomUser<string> cUser = user as CustomUser<string>;
            cUser.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserEmailStore

        public Task SetEmailAsync(TUser user, string email)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.Email == null) throw new ArgumentNullException("email");
            user.Email = email;
            TUser existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                saveFileUsers();
            }
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            if (user.Email == null)
                throw new InvalidOperationException("Cannot get the confirmation status of the e-mail because user doesn't have an e-mail.");
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentException("user");
            if (user.Email == null)
                throw new InvalidOperationException("Cannot get the confirmation status of the e-mail because user doesn't have an e-mail.");
            user.EmailConfirmed = confirmed; 
            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (email == null)
                throw new ArgumentException("email");
            TUser user = users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult<TUser>(user);
        }

        #endregion

        private void saveFileUsers()
        {
            File.WriteAllText(storageFile, JsonConvert.SerializeObject(users));
        }

        //private void saveLogins()
        //{
        //    File.WriteAllText(storageFile, JsonConvert.SerializeObject(logins));
        //}

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.PhoneNumber == null) throw new ArgumentNullException("phoneNumber");
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentException("user");
            if (user.PhoneNumber == null)
                throw new InvalidOperationException("Cannot get the confirmation status of the phoneNumber because user doesn't have an phoneNumber.");
            user.PhoneNumberConfirmed = confirmed; // don't we need to save this as well?
            TUser existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            return Task.FromResult(0);
        }

        #endregion

        #region IUserLockoutStore

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (!user.LockoutEndDate.HasValue)
                return Task.FromResult(new DateTimeOffset(DateTime.UtcNow.AddMinutes(-5)));
            else
                return Task.FromResult(user.LockoutEndDate.Value);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.LockoutEndDate = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.IsLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.IsLockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.IsTwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");
            CustomUserLogin l = new CustomUserLogin { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, UserId = user.Id };
            user.Logins.Add(l);
            saveFileUsers();
            return Task.FromResult(0);
            
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (login == null) throw new ArgumentNullException("login");
            CustomUserLogin l = user.Logins.FirstOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            if (l != null)
            {
                user.Logins.Remove(l);
                saveFileUsers();
            }
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            IList<UserLoginInfo> userLogins = user.Logins.Where(x => x.UserId == user.Id).Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToList();
            return Task.FromResult(userLogins);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null) throw new ArgumentNullException("login");
            TUser usr = users.FirstOrDefault(u => u.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey));
            return Task.FromResult<TUser>(usr);
        }

        #endregion

        #region IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            IList<Claim> c = user.Claims.Where(u => u.UserId == user.Id).Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
            return Task.FromResult(c);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            CustomUserClaim<string> c = new CustomUserClaim<string>(claim.Type, claim.Value);
            c.UserId = user.Id;
            c.Id = 0; //store has to figure out this one itself
            user.Claims.Add(c);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");
            CustomUserClaim<string> c = user.Claims.FirstOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            if (c != null)
                user.Claims.Remove(c);
            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IQueryableUserStore

        public IQueryable<TUser> Users
        {
            get
            {
                return users.AsQueryable();
            }
        }

        #endregion

    }
}