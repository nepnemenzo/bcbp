using System;
using Microsoft.Owin;
using Owin;
using bcbp_101;

[assembly: OwinStartupAttribute(typeof(bcbp_101.Startup))]
namespace bcbp_101
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        //private void ConfigureAuth(IAppBuilder app)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
