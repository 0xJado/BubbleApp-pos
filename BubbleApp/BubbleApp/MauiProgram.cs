using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BubbleWaffleViewModel = BubbleAppNew.MainPage.BubbleWaffleViewModel;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib;

namespace BubbleAppNew;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<MainPage.MainPage>();
        builder.Services.AddTransient<BubbleWaffleViewModel>();
        builder.Services.AddSingleton<OrderConfirmationViewModel>();
        builder.Services.AddTransient<OrderPage>();
        builder.Services.AddSingleton<IWaffleService, WaffleService>();
        builder.Services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
        builder.Services.AddSingleton<IOrderStorageService>(s =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "wafflemoments_orders.db3");
            return new OrderStorageService(dbPath);
        });

        
        return builder.Build();
    }
    
}