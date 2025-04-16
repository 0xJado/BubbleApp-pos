using System.Collections.ObjectModel;
using System.Globalization;
using BubbleLib;

namespace BubbleAppNew.OrderConfirmationPage;

public partial class OrderConfirmationViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private bool _isWaitingForPayment;
    [ObservableProperty] private bool _paymentConfirmed;
    [ObservableProperty] private bool _paymentTimedOut;

    [ObservableProperty] private ObservableCollection<BubbleWaffle> _orderedItems;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _euroPrice;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _solanaRate;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ShortWalletAddress))]
    private string _storeWalletAddress;

    [ObservableProperty] private ImageSource _qrCodeImage;
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
            Console.WriteLine(
                $"Final QR Payload: solana:{StoreWalletAddress}?amount={TotalSol.ToString(CultureInfo.InvariantCulture)}&label={encodedLabel}&memo={encodedMemo}&message={encodedMsg}");

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
            string? txSignature =
                await tracker.WaitForPaymentAsync(StoreWalletAddress, TotalSol, OrderId, TimeSpan.FromMinutes(3));

            if (!string.IsNullOrEmpty(txSignature))
            {
                PaymentConfirmed = true;

                // Build the order to save
                var completedOrder = new CompletedOrder
                {
                    OrderId = OrderId,
                    ItemsSummary = string.Join(", ", OrderedItems.Select(x => $"{x.Quantity}x {x.WaffleName}")),
                    TotalEur = EuroPrice,
                    TotalSol = TotalSol,
                    Wallet = StoreWalletAddress,
                    TxSignature = txSignature,
                    Timestamp = DateTime.Now
                };

                // Save to DB IMMEDIATELY after order confirmation
                await _orderStorageService.SaveOrderAsync(completedOrder);

                // Prompt for email AFTER order is saved
                bool wantsEmail = await Shell.Current.DisplayAlert(
                    "Zahlung erfolgreich",
                    "Möchten Sie eine Quittung per E-Mail erhalten?",
                    "Ja", "Nein");

                if (wantsEmail)
                {
                    var email = await Shell.Current.DisplayPromptAsync("E-Mail eingeben",
                        "Bitte geben Sie Ihre E-Mail-Adresse ein:");
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await _emailService.SendReceiptAsync(email, completedOrder, OrderedItems.ToList());
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