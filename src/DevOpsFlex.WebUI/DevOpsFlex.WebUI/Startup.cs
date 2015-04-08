using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DevOpsFlex.WebUI.Startup))]
namespace DevOpsFlex.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
