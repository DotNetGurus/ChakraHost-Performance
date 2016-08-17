using JSRTManaged;
using JSRTNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JsrtPerformanceApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            RunManagedAdd();
            RunNativeAdd();
        }

        private void RunManagedAdd()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            JSRTManagedExecutor executor = new JSRTManagedExecutor();

            for (var i = 0; i < 100000; i++)
            {
                int result = executor.AddNumbers(i, i + 1);
            }

            executor.Dispose();

            watch.Stop();
            Debug.WriteLine($"Managed elapsed ticks {watch.ElapsedTicks}");
        }

        private void RunNativeAdd()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            JSRTNativeExecutor executor = new JSRTNativeExecutor();
            executor.InitializeHost();

            for (var i = 0; i < 100000; i++)
            {
                int result = executor.AddNumbers(i, i + 1);
            }

            executor.DisposeHost();

            watch.Stop();
            Debug.WriteLine($"Native elapsed ticks {watch.ElapsedTicks}");
        }
    }
}
