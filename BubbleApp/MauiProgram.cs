using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BubbleLib;
using BubbleWaffleViewModel = BubbleAppNew.MainPage.BubbleWaffleViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using BubbleAppNew.BubbleLib;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib.Services;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Http;


namespace BubbleAppNew;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<MainPage.MainPage>();
        builder.Services.AddTransient<BubbleWaffleViewModel>();
        builder.Services.AddSingleton<OrderConfirmationViewModel>();
        builder.Services.AddSingleton<IWaffleService, WaffleService>();
        builder.Services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
        
        var app = builder.Build();
        return app;
    }
    
}