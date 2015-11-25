using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DebugWebApplication.Startup))]
namespace DebugWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
