using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IdentityUserLoginAttempts.Startup))]
namespace IdentityUserLoginAttempts
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
