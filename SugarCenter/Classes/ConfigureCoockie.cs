using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace SugarCenter.Classes
{
    internal class ConfigureMyCookie: IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public void Configure(string name, CookieAuthenticationOptions options)
        {
            // Only configure the schemes you want
            if (name == Startup.CookieScheme)
            {
                // options.LoginPath = "/someotherpath";
            }
        }

        public void Configure(CookieAuthenticationOptions options)
            => Configure(Options.DefaultName, options);
    }
}