using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using CinchExtended.Workspaces;

namespace Tester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            CinchBootStrapper.Initialise(new [] { Assembly.GetAssembly(typeof(App)) });
            base.OnStartup(e);
        }
    }
}
