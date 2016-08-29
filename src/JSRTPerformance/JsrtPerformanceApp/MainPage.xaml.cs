using JSRTManaged;
using JSRTNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        const int LIMIT = 10;
        private string _fileLocation;

        public MainPage()
        {
            this.InitializeComponent();
            LoadFile();
            _fileLocation = Path.Combine(ApplicationData.Current.LocalFolder.Path, "lodash.js");
            RunNativeAdd();
            //RunManagedAdd();
        }

        private void LoadFile()
        {
            HttpClient client = new HttpClient();
            var lodash = client.GetStringAsync("https://raw.githubusercontent.com/lodash/lodash/4.15.0/dist/lodash.js").Result;
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.CreateFileAsync("lodash.js", CreationCollisionOption.ReplaceExisting).AsTask().Result;
            using (var stream = file.OpenStreamForWriteAsync().Result)
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(lodash);
                }
            }
            
        }

        private void RunManagedAdd()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            JSRTManagedExecutor executor = new JSRTManagedExecutor();

            for (var i = 0; i < LIMIT; i++)
            {
                executor.RunScript(_fileLocation, "");
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

            for (var i = 0; i < LIMIT; i++)
            {
                executor.RunScriptFromFile(_fileLocation, "");
            }

            executor.DisposeHost();

            watch.Stop();
            Debug.WriteLine($"Native elapsed ticks {watch.ElapsedTicks}");
        }
    }
}
