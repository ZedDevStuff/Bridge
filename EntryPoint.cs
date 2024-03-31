using Bridge.APIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge
{
    internal class EntryPoint
    {
        public static string Url { get; set; } = "https://google.com";
        [STAThread]
        public static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                DirectoryInfo info = new DirectoryInfo(args[0]);
                if(info.Exists)
                {
                    // Start the web server on that folder
                }
                else
                {
                    Url = args[0];
                }
            }
            Handler.Init();
            Bridge.App app = new Bridge.App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
