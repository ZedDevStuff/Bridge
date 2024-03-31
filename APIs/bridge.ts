class BridgeManager {
    private bridgePresent: boolean = false;
    ensureBridgePresence() {
        try
        {
            (window as any).chrome.webview.postMessage("ping");
            (window as any)._bridgeMessageReceived = this.messageReceivedHandler.bind(this);
            this.bridgePresent = true;
        }
        catch
        {
            this.bridgePresent = false;
        }
    }
    private receivedMessages: Map<number, any> = new Map();
    private messagesListeners: Map<number, (data: any) => void> = new Map();
    private permanentListeners: Map<number, (data: any) => void> = new Map();
    private async messageReceivedHandler(id: number, base64json: string) {
        setTimeout(() => {
            let json = atob(base64json);
            let response: { id: number, response: string, convert: boolean };
            try {
                response = JSON.parse(json);
                if(this.permanentListeners.has(id)) {
                    this.permanentListeners.get(id)!(response.convert ? JSON.parse(response.response) : response.response);
                }
                else if(this.messagesListeners.has(id)){
                    this.messagesListeners.get(id)!(response.convert ? JSON.parse(response.response) : response.response);
                    this.messagesListeners.delete(id);
                }
            }
            catch(e) {
                console.error("Failed to parse response", e);
                return;
            }
            
        }, 0);
    }
    send(object: any) {
        if(this.bridgePresent) {
            (window as any).chrome.webview.postMessage(object);
        }
    }
    sendAndWaitForResponse(object: any): Promise<any> {
        return new Promise((resolve, reject) => {
            let id = this.getAvailableId();
            object.id = id;
            this.messagesListeners.set(id, (data) => {
                resolve(data);
            });
            this.send(object);
        });
    }
    addListener(type: string, data: any, callback: (data: any) => void) {
        let id = this.getAvailableId();
        this.permanentListeners.set(id, callback);
        this.send({type: type, id: id, data: data});
    }
    getAvailableId() {
        let id = Math.floor(Math.random() * 1000000);
        while(this.receivedMessages.has(id)) {
            id = Math.floor(Math.random() * 1000000);
        }
        return id;
    }
}
export const Bridge = new BridgeManager();

export const FileSystemApi = {
    open(path: string) {
        Bridge.send({type: "File.Open", path: path});
    },

    fileExists(path: string): Promise<boolean> {
        return Bridge.sendAndWaitForResponse({type: "File.FileExists", path: path}) as Promise<boolean>;
    },
    directoryExists(path: string): Promise<boolean> {
        return Bridge.sendAndWaitForResponse({type: "File.DirectoryExists", path: path}) as Promise<boolean>;
    }, 
    
    directoryGetFiles(path: string, searchPattern: string = ""): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.GetFiles", path: path, searchPattern: searchPattern}) as Promise<string[]>;
    },
    directoryGetFileSystemEntries(path: string, searchPattern: string = ""): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.GetFileSystemEntries", path: path, searchPattern: searchPattern}) as Promise<string[]>;
    },
    directoryCreateDirectory(path: string) {
        Bridge.send({type: "File.CreateDirectory", path: path});
    },
    directoryGetDirectories(path: string): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.GetDirectories", path: path}) as Promise<string[]>;
    },

    openFileDialog(title: string, baseDir: string = "", filter: string = "", multiselect: boolean = false): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.OpenFileDialog", title: title, baseDir: baseDir, filter: filter, multiselect: multiselect}) as Promise<string[]>;
    },
    openFolderDialog(title: string, baseDir: string = "", multiselect: boolean = false): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.OpenFolderDialog", title: title, baseDir: baseDir, multiselect: multiselect}) as Promise<string[]>;
    },

    // Read 

    fileReadAllText(path: string): Promise<string> {
        return Bridge.sendAndWaitForResponse({type: "File.ReadAllText", path: path}) as Promise<string>;
    },
    fileReadAllLines(path: string): Promise<string[]> {
        return Bridge.sendAndWaitForResponse({type: "File.ReadAllLines", path: path}) as Promise<string[]>;
    },
    fileReadAllBytes(path: string): Promise<string> {
        return new Promise(async (resolve, reject) => {
            let response = await (Bridge.sendAndWaitForResponse({type: "File.ReadAllBytes", path: path}) as Promise<string>);
            let bytes = atob(response);
            resolve(bytes);
        });
    },

    // Write

    fileWriteAllText(path: string, contents: string) {
        Bridge.send({type: "File.WriteAllText" , path: path, contents: contents });
    },
    fileWriteAllLines(path: string, lines: string[]) {
        Bridge.send({type: "File.WriteAllLines" , path: path, lines: lines });
    },
    fileWriteAllBytes(path: string, bytes: Uint8Array) {
        let base64 = btoa(String.fromCharCode(...bytes));
        Bridge.send({type: "File.WriteAllBytes" , path: path, bytes: base64 });
    },
    fileWriteAllBytesBase64(path: string, base64: string) {
        Bridge.send({type: "File.WriteAllBytes" , path: path, bytes: base64 });
    },

    // Untested
    /*fileSystemWatcherCreate(path: string, callback: (data: any) => void) {
        Bridge.addListener("FileSystemWatcher.Create", path, callback);
    }*/
}

export const WindowApi = {
    setTitle(title: string) {
        Bridge.send({type: "Window.SetTitle", title: title});
    },
    setIcon(path: string) {
        Bridge.send({type: "Window.SetIcon", path: path});
    },
    setSize(width: number, height: number) {
        Bridge.send({type: "Window.SetSize", width: width, height: height});
    },

    maximize() {
        Bridge.send({type: "Window.Maximize"});
    },
    minimize() {
        Bridge.send({type: "Window.Minimize"});
    },
    restore() {
        Bridge.send({type: "Window.Restore"});
    },
    close() {
        Bridge.send({type: "Window.Close"});
    },
    setResizable(resizable: boolean) {
        Bridge.send({type: "Window.SetResizable", resizable: resizable});
    },
}