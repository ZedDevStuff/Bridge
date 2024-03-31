using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.APIs
{
    abstract class BaseApi
    {
        public abstract string Name { get; }
        protected Dictionary<string, Action<Dictionary<string, object?>>> Functions = new Dictionary<string, Action<Dictionary<string, object?>>>();
        
        public BaseApi()
        {
            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var method in methods)
            {

                object? attribute = method.GetCustomAttributes(true)
                .Where(attr => attr.GetType() == typeof(BridgeFunction))
                .FirstOrDefault();
                if (attribute != null)
                {
                    Functions.Add(((BridgeFunction)attribute).Name ?? method.Name, parameters => method.Invoke(this, new object[] { parameters }));
                }
            }
        }
        public abstract void HandleRequest(string command, Dictionary<string, object?> parameters);
    }
    internal class BridgeFunction : Attribute
    {
        public string? Name { get; private set; }
        public BridgeFunction() { }
        public BridgeFunction(string name)
        {
            Name = name;
        }
    }
}
