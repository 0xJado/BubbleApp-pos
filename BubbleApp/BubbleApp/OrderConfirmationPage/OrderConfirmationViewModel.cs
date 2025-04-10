using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using BubbleLib;
using BubbleLib.Models;

namespace BubbleAppNew.OrderConfirmationPage;

public partial class OrderConfirmationViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] 
    private bool _isWaitingForPayment;
    [ObservableProperty] 
    private bool _paymentConfirmed;
    [ObservableProperty] 
    private bool _paymentTimedOut;
    
    [ObservableProperty] 
    private ObservableCollection<BubbleWaffle> _orderedItems;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _euroPrice;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _solanaRate;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShortWalletAddress))]
    private string _storeWalletAddress;
    
    [ObservableProperty] 
    private ImageSource _qrCodeImage;
    public decimal TotalSol => Math.Round(EuroPrice / SolanaRate, 3);

    public string ShortWalletAddress =>
        string.IsNullOrWhiteSpace(StoreWalletAddress)
            ? string.Empty
            : $"{StoreWalletAddress[..6]}...{StoreWalletAddress[^4..]}";

    public string OrderId { get; }

    public OrderConfirmationViewModel(
        ObservableCollection<BubbleWaffle> orderedItems,
        decimal euroPrice,
        decimal solanaRate,
        string storeWalletAddress)
    {
        OrderedItems = orderedItems;
        EuroPrice = euroPrice;
        SolanaRate = solanaRate;
        StoreWalletAddress = storeWalletAddress;
        OrderId = Guid.NewGuid().ToString().Split('-')[0];
    }

    public async Task GenerateQrCodeAsync(IQrCodeService qrCodeService)
    {
        if (string.IsNullOrWhiteSpace(StoreWalletAddress) || TotalSol <= 0)
            return;

        try
        {
            var encodedLabel = Uri.EscapeDataString("WaffleMoments");
            var encodedMemo = Uri.EscapeDataString($"Order-{OrderId}");
            var encodedMsg = Uri.EscapeDataString("Vielen Dank!");
            
            var solUri =
                $"solana:{StoreWalletAddress}?amount={TotalSol.ToString(CultureInfo.InvariantCulture)}&label={encodedLabel}&memo={encodedMemo}&message={encodedMsg}";
            
            QrCodeImage = await Task.Run(() => qrCodeService.GenerateQrCode(solUri));
            Console.WriteLine("QR Code generated.");
            Console.WriteLine($"Final QR Payload: solana:{StoreWalletAddress}?amount={TotalSol.ToString(CultureInfo.InvariantCulture)}&label={encodedLabel}&memo={encodedMemo}&message={encodedMsg}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating QR Code: {ex.Message}");
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Console.WriteLine("Applying query attributes.");
        IsWaitingForPayment = true;

        try
        {
            var tracker = new TxnTracker();
            var success = await tracker.WaitForPaymentAsync(StoreWalletAddress, TotalSol, OrderId,TimeSpan.FromMinutes(3));

            if (success)
                PaymentConfirmed = true;
            else
                PaymentTimedOut = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error tracking payment: {ex.Message}");
        }
        finally
        {
            IsWaitingForPayment = false;
        }
    }
}