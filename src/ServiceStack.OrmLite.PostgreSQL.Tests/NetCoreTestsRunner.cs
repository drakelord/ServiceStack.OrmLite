//NUnitLite isn't recognized in VS2017 - shouldn't need NUnitLite with NUnit 3.5+ https://github.com/nunit/dotnet-test-nunit
#if false
using NUnitLite;
using NUnit.Common;
using System.Reflection;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Globalization;
using System.Threading;
using ServiceStack.OrmLite.Tests;

namespace ServiceStack.OrmLite.PostgreSQL.Tests
{
    public class NetCoreTestsRunner
    {
        /// <summary>
        /// The main program executes the tests. Output may be routed to
        /// various locations, depending on the arguments passed.
        /// </summary>
        /// <remarks>Run with --help for a full list of arguments supported</remarks>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            var licenseKey = Environment.GetEnvironmentVariable("SERVICESTACK_LICENSE");
            if (licenseKey.IsNullOrEmpty())
                throw new ArgumentNullException("SERVICESTACK_LICENSE", "Add Environment variable for SERVICESTACK_LICENSE");

            Licensing.RegisterLicense(licenseKey);
            //"ActivatedLicenseFeatures: ".Print(LicenseUtils.ActivatedLicenseFeatures());

            var postgreSqlDb = Environment.GetEnvironmentVariable("POSTGRESQL_DB");

            if (!String.IsNullOrEmpty(postgreSqlDb))
            {
                Config.PostgreSqlDb = postgreSqlDb;
            }

    	    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            JsConfig.InitStatics();

            //JsonServiceClient client = new JsonServiceClient();
            var writer = new ExtendedTextWrapper(Console.Out);
            return new AutoRun(((IReflectableType)typeof(NetCoreTestsRunner)).GetTypeInfo().Assembly).Execute(args, writer, Console.In);
        }
    }
}
#endif