using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CustomUserStore.UserManagement
{

    public class CustomStringUser: CustomUser<string>
    {
        public CustomStringUser()
        {
            Claims = new List<CustomUserClaim<string>>();
            Logins = new List<CustomUserLogin>();
        }

        public CustomStringUser(string userName):this()
        {
            Id = Guid.NewGuid().ToString();
            if (userName == null)
                throw new ArgumentException("userName");
            UserName = userName;
        }

        public CustomStringUser(string userName, string email): this(userName)
        {
            Email = email;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CustomStringUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            foreach (CustomUserClaim<string> claim in Claims)
                userIdentity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class CustomGuidUser: CustomUser<Guid>
    {
        public CustomGuidUser()
        {
            Claims = new List<CustomUserClaim<Guid>>();
            Logins = new List<CustomUserLogin>();
        }

        public CustomGuidUser(string userName):this()
        {
            Id = Guid.NewGuid();
            if (userName == null)
                throw new ArgumentException("userName");
            UserName = userName;
        }

        public CustomGuidUser(string userName, string email): this(userName)
        {
            Email = email;
        }

        //internal async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CustomGuidUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    foreach (CustomUserClaim<Guid> claim in Claims)
        //        userIdentity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
        //    // Add custom user claims here
        //    return userIdentity;
        //}
    }


    public abstract class CustomUser<T> : IUser<T>
    {

        #region constructors

        public CustomUser()
        {
            Claims = new List<CustomUserClaim<T>>();
            Logins = new List<CustomUserLogin>();
        }

        public CustomUser(string userName):this()
        {
            
            if (userName == null)
                throw new ArgumentException("userName");
            UserName = userName;
        }

        public CustomUser(string userName, string email): this(userName)
        {
            Email = email;
        }

        #endregion


        public List<CustomUserClaim<T>> Claims { get; set; }

        public List<CustomUserLogin> Logins { get; set; }



        public T Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public bool IsTwoFactorEnabled { get; set; }

        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CustomUser<T>> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        //    return userIdentity;
        //}

        internal static string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }

    }

    public class CustomUserLogin
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string UserId { get; set; }
    }

    public class CustomUserClaim<T>
    {

        public CustomUserClaim(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public int Id { get; set; }

        public T UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

}