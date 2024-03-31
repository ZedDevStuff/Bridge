using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bridge.APIs
{
    internal class WindowApi : BaseApi
    {
        public override string Name => "Window";
        public override void HandleRequest(string command, Dictionary<string, object?> parameters)
        {
            if(Functions.TryGetValue(command, out Action<Dictionary<string, object?>>? function))
            {
                function(parameters);
            }
        }

        [BridgeFunction]
        private void SetTitle(Dictionary<string, object?> parameters)
        {
            SetTitleRequest? request = JsonSerializer.Deserialize<SetTitleRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                MainWindow.Instance?.Dispatcher.Invoke(() =>
                {
                    MainWindow.Instance.Title = request.title;
                });
            }
        }
        [BridgeFunction]
        private void SetIcon(Dictionary<string, object?> parameters)
        {
            SetIconRequest? request = JsonSerializer.Deserialize<SetIconRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                MainWindow.Instance?.Dispatcher.Invoke(() =>
                {
                    MainWindow.Instance.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri(request.path));
                });
            }
        }
        [BridgeFunction]
        private void SetSize(Dictionary<string, object?> parameters)
        {
            SetSizeRequest? request = JsonSerializer.Deserialize<SetSizeRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                MainWindow.Instance?.Dispatcher.Invoke(() =>
                {
                    MainWindow.Instance.Width = request.width;
                    MainWindow.Instance.Height = request.height;
                });
            }
        }

        [BridgeFunction]
        private void Maximize(Dictionary<string, object?> parameters)
        {
            MainWindow.Instance?.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.WindowState = System.Windows.WindowState.Maximized;
            });
        }
        [BridgeFunction]
        private void Minimize(Dictionary<string, object?> parameters)
        {
            MainWindow.Instance?.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.WindowState = System.Windows.WindowState.Minimized;
            });
        }
        [BridgeFunction]
        private void Restore(Dictionary<string, object?> parameters)
        {
            MainWindow.Instance?.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.WindowState = System.Windows.WindowState.Normal;
            });
        }
        [BridgeFunction]
        private void Close(Dictionary<string, object?> parameters)
        {
            MainWindow.Instance?.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance.Close();
            });
        }
        [BridgeFunction]
        private void SetResizable(Dictionary<string, object?> parameters)
        {
            SetResizableRequest? request = JsonSerializer.Deserialize<SetResizableRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                MainWindow.Instance?.Dispatcher.Invoke(() =>
                {
                    MainWindow.Instance.ResizeMode = request.resizable ? System.Windows.ResizeMode.CanResize : System.Windows.ResizeMode.NoResize;
                });
            }
        }

        private record SetTitleRequest(string title);
        private record SetIconRequest(string path);
        private record SetSizeRequest(double width, double height);
        private record SetResizableRequest(bool resizable);
    }
}
