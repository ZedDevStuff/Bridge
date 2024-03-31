# Bridge

Bridge is a simple way to bridge a web app and Windows

# Why

Why not? I was working on a web app (prototype, i'll probably never finish it but it's fun to work on) and noticed how annoying the PWA development experience was. Tauri would have required me to learn Rust to extend it on top of only building installers and Flutter doesn't have a desktop WebView plugin as good as the mobile ones, so i made my own thing instead.

# How

To get started quickly (if you feel like using this thing), build Bridge with dotnet and run it with the address of your web app as the first and only argument. (e.g. `Bridge.exe http://localhost:5173`) Without any arguments, Bridge will open a window with a WebView that navigates to `https://google.com`. Now, your app runs in bridge but isn't doing much more than before. Intergrate bridge.ts into your app then you can call its functions to interact with the host.

# Next

In the future (if i don't abandon this), i'll probably add a http server to Bridge so you can specify a folder to use as the web app.
