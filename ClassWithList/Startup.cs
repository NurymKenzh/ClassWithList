using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassWithList.Startup))]
namespace ClassWithList
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
