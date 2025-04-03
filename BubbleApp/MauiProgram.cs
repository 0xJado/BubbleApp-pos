using System.Collections.Immutable;
using BubbleAppNew.OrderSummaryPage;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BubbleLib;
using BubbleWaffleViewModel = BubbleAppNew.MainPage.BubbleWaffleViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using BubbleAppNew.BubbleLib;
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
        builder.Services.AddSingleton<IWaffleService, WaffleService>();
        builder.Services.AddSingleton<MainPage.MainPage>();
        builder.Services.AddTransient<BubbleWaffleViewModel>();
        builder.Services.AddSingleton<OrderViewModel>();
        builder.Services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
        
        return builder.Build();
    }
    
}