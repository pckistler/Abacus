using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Abacus.Startup))]
namespace Abacus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
