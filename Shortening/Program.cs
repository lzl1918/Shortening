using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Shortening
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost host = WebHost.CreateDefaultBuilder(args)
                                   .UseStartup<Startup>()
                                   .UseUrls("http://*:5000")
                                   .Build();
            host.Run();
        }

    }
}
