using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using BubbleLib;

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

    [ObservableProperty] private string _emailAddress;
    
    [RelayCommand]
    private async Task NewTransactionAsync()
    {
        _waffleService.ClearOrder();
        EmailAddress = string.Empty;

        await Shell.Current.GoToAsync(".."); // Navigates back to MainPage
    }

    
    private readonly IOrderStorageService _orderStorageService;
    private readonly IWaffleService _waffleService;
    private readonly IEmailService _emailService;

    public OrderConfirmationViewModel(
        ObservableCollection<BubbleWaffle> orderedItems,
        decimal euroPrice,
        decimal solanaRate,
        string storeWalletAddress,
        IOrderStorageService orderStorageService,
        IWaffleService waffleService,
        IEmailService emailService
        )
    {
        OrderedItems = orderedItems;
        EuroPrice = euroPrice;
        SolanaRate = solanaRate;
        StoreWalletAddress = storeWalletAddress;
        OrderId = Guid.NewGuid().ToString().Split('-')[0];
        
        _orderStorageService = orderStorageService;
        _waffleService = waffleService;
        _emailService = emailService;
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
            var success = 
                await tracker.WaitForPaymentAsync(StoreWalletAddress, TotalSol, OrderId, TimeSpan.FromMinutes(3));

            if (success)
            {
                PaymentConfirmed = true;

                string userEmail = null;

                bool wantsEmail = await Shell.Current.DisplayAlert(
                    "Zahlung erfolgreich",
                    "Möchten Sie eine Quittung per E-Mail erhalten?",
                    "Ja", "Nein");

                if (wantsEmail)
                {
                    userEmail = await Shell.Current.DisplayPromptAsync(
                        "E-Mail eingeben",
                        "Bitte geben Sie Ihre E-Mail-Adresse ein:");

                    Console.WriteLine($"E-Mail-Adresse für Quittung: {userEmail}");
                }

                var completedOrder = new CompletedOrder
                {
                    OrderId = OrderId,
                    ItemsSummary = string.Join(", ", OrderedItems.Select(x => $"{x.Quantity}x {x.WaffleName}")),
                    TotalEur = EuroPrice,
                    TotalSol = TotalSol,
                    Wallet = StoreWalletAddress,
                    Timestamp = DateTime.Now
                };

                await _orderStorageService.SaveOrderAsync(completedOrder);

                // Send Email AFTER storing
                if (!string.IsNullOrWhiteSpace(userEmail))
                
                {
                    try
                    {
                        await _emailService.SendReceiptAsync(userEmail, completedOrder, OrderedItems.ToList());
                        Console.WriteLine($"📬 Quittung an {userEmail} gesendet");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim E-Mail-Versand: {ex.Message}");
                        await Shell.Current.DisplayAlert("Fehler", "Die Quittung konnte nicht gesendet werden.", "OK");
                    }
                }
            }
            else
            {
                PaymentTimedOut = true;
            }
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