using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bridge.APIs
{
    internal class Handler
    {
        private static Dictionary<string, BaseApi> APIs = new Dictionary<string, BaseApi>();
        private static CoreWebView2 WebView;
        public static void Init()
        {
            Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseApi)))
            .ToList()
            .ForEach(api =>
            {
                BaseApi b = (BaseApi)Activator.CreateInstance(api);
                APIs.Add(b.Name, b);
            });
        }
        public static void SetWebView(CoreWebView2 webView)
        {
            WebView = webView;
        }
        public static void HandleRequest(Dictionary<string, object?> request)
        {
            if(request.TryGetValue("type", out object? type))
            {
                string[] typeComponents = type.ToString().Split('.');
                if (APIs.TryGetValue(typeComponents[0], out BaseApi api))
                {
                    api.HandleRequest(typeComponents[1], request);
                }
            }
        }
        public static void ReturnResponse(int id, object response, bool convert = false)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default,
            };
            string json = JsonSerializer.Serialize(response);
            Response r = new Response(id, json, convert);
            json = JsonSerializer.Serialize(r);
            /*if(json.StartsWith("\"") && json.EndsWith("\""))
            {
                json = "'" + json.Substring(1, json.Length - 2) + "'";
            }
            else json = "'" + json + "'";*/
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            Debug.WriteLine(base64);
            string script = $"window._bridgeMessageReceived({id}, '{base64}')";
            WebView.ExecuteScriptAsync(script);
        }
        private record Response(int id, string response, bool convert);
    }
}
