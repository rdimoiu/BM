using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BuildingManagement.Startup))]
namespace BuildingManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
