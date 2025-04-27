using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using BubbleWaffleViewModel = BubbleAppNew.MainPage.BubbleWaffleViewModel;
using BubbleAppNew.OrderConfirmationPage;
using BubbleAppNew.OrderHistory;
using BubbleLib;
using Solnet.Rpc;

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
        builder.Services.AddTransient<OrderHistoryViewModel>();
        builder.Services.AddTransient<OrderHistoryPage>();
        builder.Services.AddSingleton<IWaffleService, WaffleService>();
        builder.Services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
        builder.Services.AddSingleton<IOrderStorageService>(s =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "wafflemoments_orders.db3");
            return new OrderStorageService(dbPath);
        });
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();
        builder.Services.AddSingleton<IRpcClient>(s =>
        {
            // Uses DevNet for testing; for production, switch to MainNet.
            var rpcClient = ClientFactory.GetClient(Cluster.DevNet);
            return rpcClient;
        });builder.Services.AddSingleton<ITxnTracker, TxnTracker>();

        
        return builder.Build();
    }
}