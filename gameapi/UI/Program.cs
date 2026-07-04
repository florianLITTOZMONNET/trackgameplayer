using Avalonia;
using System;
using Avalonia.ReactiveUI;

namespace UI;

sealed class Program
{

    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI() 
            .LogToTrace()
            .UsePlatformDetect()
            .LogToTrace();
}
