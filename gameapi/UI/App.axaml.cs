using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Infrastructure.ApiClients.Steam;
using Infrastructure.Database.Steam;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UI.ViewModels;

namespace UI;

public partial class App : Avalonia.Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddHttpClient<SteamPlayerClient>();
        services.AddHttpClient<SteamLibraryClient>();
        services.AddHttpClient<SteamBadgeClient>();

        var dbPath = config["Database:Path"] ?? "my_steam_data.db";
        services.AddSingleton(_ => new SteamDatabaseManager(dbPath));
        services.AddSingleton<MainWindowViewModel>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };

            desktop.ShutdownRequested += (_, _) =>
                Services.GetRequiredService<SteamDatabaseManager>().Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}