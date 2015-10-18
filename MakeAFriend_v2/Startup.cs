using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MakeAFriend_v2.Startup))]
namespace MakeAFriend_v2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
