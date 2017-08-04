using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CustomUserStore.Startup))]
namespace CustomUserStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
