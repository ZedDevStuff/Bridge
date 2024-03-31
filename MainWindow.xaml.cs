using Bridge.APIs;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bridge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            WebView.CoreWebView2InitializationCompleted += Init;
            WebView.Source = new Uri(EntryPoint.Url);
        }

        private void WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Dictionary<string, object?>? data = JsonSerializer.Deserialize<ExpandoObject>(e.WebMessageAsJson)?.ToDictionary();
            if(data != null) Handler.HandleRequest(data);
        }

        private void Init(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WebView.CoreWebView2.WebMessageReceived += WebMessageReceived;
            Handler.SetWebView(WebView.CoreWebView2);
        }
        public string WebSource
        {
            get
            {
                return EntryPoint.Url;
            }
        }
    }
}