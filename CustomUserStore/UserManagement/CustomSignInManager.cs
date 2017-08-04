using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomUserStore.UserManagement
{
    public class CustomSignInManager : SignInManager<CustomStringUser, string>
    {
        public CustomSignInManager(UserManager<CustomStringUser, string> userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(CustomStringUser user)
        {
            return user.GenerateUserIdentityAsync((CustomUserManager)UserManager);
        }

        public static CustomSignInManager Create(IdentityFactoryOptions<CustomSignInManager> options, IOwinContext context)
        {
            return new CustomSignInManager(context.GetUserManager<CustomUserManager>(), context.Authentication);
        }

    }

    //public class CustomGuidSignInManager : SignInManager<CustomGuidUser, Guid>
    //{
    //    public CustomGuidSignInManager(UserManager<CustomGuidUser, Guid> userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
    //    {
    //    }

    //    public override Task<ClaimsIdentity> CreateUserIdentityAsync(CustomGuidUser user)
    //    {
    //        return user.GenerateUserIdentityAsync((CustomGuidSignInManager)UserManager);
    //    }

    //    public static CustomGuidSignInManager Create(IdentityFactoryOptions<CustomGuidSignInManager> options, IOwinContext context)
    //    {
    //        return new CustomGuidSignInManager(context.GetUserManager<CustomUserManager>(), context.Authentication);
    //    }

    //}
}