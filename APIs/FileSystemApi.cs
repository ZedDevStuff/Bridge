using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bridge.APIs
{
    internal class FileSystemApi : BaseApi
    {
        public override string Name => "File";
        public override void HandleRequest(string command, Dictionary<string, object?> parameters)
        {
            if(Functions.TryGetValue(command, out Action<Dictionary<string, object?>>? function))
            {
                function(parameters);
            }
        }
        [BridgeFunction]
        private void Open(Dictionary<string, object?> parameters)
        {
            if(parameters.TryGetValue("data", out object? data))
            {
                OpenRequest? request = JsonSerializer.Deserialize<OpenRequest>(JsonSerializer.Serialize(data));
                if(request != null)
                {
                    try
                    {
                        Process.Start(request.path);
                    }
                    catch { }
                }
            }
        }

        [BridgeFunction]
        private void FileExists(Dictionary<string, object?> parameters)
        {
            FileExistsRequest? request = JsonSerializer.Deserialize<FileExistsRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                Handler.ReturnResponse(request.id, File.Exists(request.path), true);
            }
        }
        [BridgeFunction]
        private void DirectoryExists(Dictionary<string, object?> parameters)
        {
            DirectoryExistsRequest? request = JsonSerializer.Deserialize<DirectoryExistsRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                Handler.ReturnResponse(request.id, Directory.Exists(request.path), true);
            }
        }

        [BridgeFunction]
        private void GetFiles(Dictionary<string, object?> parameters)
        {
            GetFilesRequest? request = JsonSerializer.Deserialize<GetFilesRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    string[] files = Directory.GetFiles(request.path, request.searchPattern).Select(p => p.Replace("\\", "/")).ToArray();
                    Handler.ReturnResponse(request.id, files);
                }
                catch
                {
                    Handler.ReturnResponse(request.id, new string[] { "" });
                }
            }
        }
        [BridgeFunction]
        private void GetFileSystemEntries(Dictionary<string, object?> parameters)
        {
            GetFilesRequest? request = JsonSerializer.Deserialize<GetFilesRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    string[] files = Directory.GetFiles(request.path, request.searchPattern).Select(p => p.Replace("\\", "/")).ToArray();
                    string[] directories = Directory.GetDirectories(request.path, request.searchPattern).Select(p => p.Replace("\\", "/")+"/").ToArray();
                    Handler.ReturnResponse(request.id, directories.Concat(files).ToArray(), true);
                }
                catch
                {
                    Handler.ReturnResponse(request.id, new string[] { "" });
                }
            }
        }
        [BridgeFunction]
        private void CreateDirectory(Dictionary<string, object?> parameters)
        {
            CreateDirectoryRequest? request = JsonSerializer.Deserialize<CreateDirectoryRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                Debug.WriteLine(request.path);
                try
                {
                    Directory.CreateDirectory(request.path);
                }
                catch { }
            }
        }
        [BridgeFunction]
        private void GetDirectories(Dictionary<string, object?> parameters)
        {
            GetDirectoriesRequest? request = JsonSerializer.Deserialize<GetDirectoriesRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    string[] directories = Directory.GetDirectories(request.path, request.searchPattern).Select(p => p.Replace("\\", "/")).ToArray();
                    Handler.ReturnResponse(request.id, directories);
                }
                catch
                {
                    Handler.ReturnResponse(request.id, new string[] { "" });
                }
            }
        }

        [BridgeFunction]
        private void OpenFileDialog(Dictionary<string, object?> parameters)
        {
            OpenFileDialogRequest? request = JsonSerializer.Deserialize<OpenFileDialogRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                    dialog.InitialDirectory = request.baseDir;
                    dialog.Title = request.title;
                    dialog.Filter = request.filter;
                    dialog.Multiselect = request.multiselect;
                    if(dialog.ShowDialog() == true)
                    {
                        Handler.ReturnResponse(request.id, dialog.SafeFileNames.Select(p => p.Replace("\\", "/")).ToArray());
                    }
                    else Handler.ReturnResponse(request.id, new string[] { "" });
                }
                catch { }
            }
        }
        [BridgeFunction]
        private void OpenFolderDialog(Dictionary<string, object?> parameters)
        {
            OpenFolderDialogRequest? request = JsonSerializer.Deserialize<OpenFolderDialogRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    Microsoft.Win32.OpenFolderDialog dialog = new Microsoft.Win32.OpenFolderDialog();
                    dialog.InitialDirectory = request.baseDir;
                    dialog.Title = request.title;
                    dialog.Multiselect = request.multiselect;
                    if(dialog.ShowDialog() == true)
                    {
                        Handler.ReturnResponse(request.id, dialog.FolderNames.Select(p => p.Replace("\\", "/")).ToArray(), true);
                    }
                    else Handler.ReturnResponse(request.id, new string[] {""});
                }
                catch { }
            }
        }

        [BridgeFunction]
        private void WriteAllText(Dictionary<string, object?> parameters)
        {
            WriteAllTextRequest? potentialRequest = JsonSerializer.Deserialize<WriteAllTextRequest>(JsonSerializer.Serialize(parameters));
            if(potentialRequest is WriteAllTextRequest request)
            {
                try
                {
                    File.WriteAllText(request.path, request.contents);
                }
                catch { }
            }
        }
        [BridgeFunction]
        private void WriteAllLines(Dictionary<string, object?> parameters)
        {
            WriteAllLinesRequest? request = JsonSerializer.Deserialize<WriteAllLinesRequest>(JsonSerializer.Serialize(parameters));
            if(request != null)
            {
                try
                {
                    File.WriteAllLines(request.path, request.lines);
                }
                catch { }
            }
        }
        [BridgeFunction]
        private void WriteAllBytes(Dictionary<string, object?> parameters)
        {
            WriteAllBytesRequest? request = JsonSerializer.Deserialize<WriteAllBytesRequest>(JsonSerializer.Serialize(parameters));
            request!.GetBytes();
            if(request != null)
            {
                try
                {
                    File.WriteAllBytes(request.path, request.GetBytes());
                }
                catch { }
            }
        }

        [BridgeFunction]
        private void ReadAllText(Dictionary<string, object?> parameters)
        {
            ReadAllTextRequest? request = JsonSerializer.Deserialize<ReadAllTextRequest>(JsonSerializer.Serialize(parameters));
            if (request != null)
            {
                try
                {
                    string contents = File.ReadAllText(request.path);
                    Handler.ReturnResponse(request.id, contents, true);
                }
                catch
                {
                    Handler.ReturnResponse(request.id, "");
                }
            }
        }
        [BridgeFunction]
        private void ReadAllLines(Dictionary<string, object?> parameters)
        {
            ReadAllLinesRequest? request = JsonSerializer.Deserialize<ReadAllLinesRequest>(JsonSerializer.Serialize(parameters));
            if (request != null)
            {
                try
                {
                    string[] lines = File.ReadAllLines(request.path);
                    Handler.ReturnResponse(request.id, lines, true);
                }
                catch
                {
                    Handler.ReturnResponse(request.id, "");
                }
            }
        }
        [BridgeFunction]
        private void ReadAllBytes(Dictionary<string, object?> parameters)
        {
            ReadAllBytesRequest? request = JsonSerializer.Deserialize<ReadAllBytesRequest>(JsonSerializer.Serialize(parameters));
            if (request != null)
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(request.path);
                    Handler.ReturnResponse(request.id, bytes, true);
                    //Handler.ReturnResponse(request.id, Convert.ToBase64String(bytes));
                }
                catch
                {
                    Handler.ReturnResponse(request.id, "");
                }
            }
        }
        
        [BridgeFunction]
        private void WatchFilesystem(Dictionary<string, object?> parameters)
        {
            if(parameters.TryGetValue("data", out object? data))
            {
                WatchFilesystemRequest? request = JsonSerializer.Deserialize<WatchFilesystemRequest>(JsonSerializer.Serialize(data));
                if(request != null)
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(request.path);
                    watcher.EnableRaisingEvents = true;
                    watcher.Changed += (s, e) => Handler.ReturnResponse(request.id, new { type = "changed", path = e.FullPath });
                    watcher.Created += (s, e) => Handler.ReturnResponse(request.id, new { type = "created", path = e.FullPath });
                    watcher.Deleted += (s, e) => Handler.ReturnResponse(request.id, new { type = "deleted", path = e.FullPath });
                    watcher.Renamed += (s, e) => Handler.ReturnResponse(request.id, new { type = "renamed", path = e.FullPath });
                }
            }
        }

        private record OpenRequest(string path);

        private record FileExistsRequest(string path, int id);
        private record DirectoryExistsRequest(string path, int id);

        private record GetFilesRequest(string path, string searchPattern, int id);
        private record CreateDirectoryRequest(string path);
        private record GetDirectoriesRequest(string path, string searchPattern, int id);

        private record OpenFileDialogRequest(string title, string baseDir, string filter, bool multiselect, int id);
        private record OpenFolderDialogRequest(string title, string baseDir, bool multiselect, int id);

        private record ReadAllTextRequest(string path, int id);
        private record ReadAllLinesRequest(string path, int id);
        private record ReadAllBytesRequest(string path, int id);

        private record WriteAllTextRequest(string path, string contents);
        private record WriteAllLinesRequest(string path, string[] lines);
        private record WriteAllBytesRequest(string path, string bytes)
        {
            public byte[] GetBytes() => Convert.FromBase64String(bytes);
        }

        private record WatchFilesystemRequest(string path, int id);
        
    }
}
