using System.Collections.ObjectModel;
using System.Globalization;
using BubbleLib;

namespace BubbleAppNew.OrderConfirmationPage;

/// <summary>
/// ViewModel responsible for displaying the confirmation view,
/// managing QR code generation, payment tracking, and saving order details.
/// </summary>
public partial class OrderConfirmationViewModel : ObservableObject, IQueryAttributable
{
    /// Indicates whether the app is currently waiting for a blockchain payment.
    [ObservableProperty] private bool _isWaitingForPayment;

    /// Indicates if a payment was successfully confirmed.
    [ObservableProperty] private bool _paymentConfirmed;

    /// Indicates if the payment attempt timed out after a set duration.
    [ObservableProperty] private bool _paymentTimedOut;

    /// The list of items that were ordered.
    [ObservableProperty] private ObservableCollection<BubbleWaffle> _orderedItems;

    /// Total price in Euro.
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _euroPrice;

    /// Current Solana to Euro conversion rate.
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalSol))]
    private decimal _solanaRate;

    /// Solana wallet address of the store.
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ShortWalletAddress))]
    private string _storeWalletAddress;

    /// Image for the generated QR code.
    [ObservableProperty] private ImageSource _qrCodeImage;

    /// Displays the total price converted into Solana.
    public decimal TotalSol => Math.Round(EuroPrice / SolanaRate, 3);

    /// Truncated version of the wallet address for display purposes.
    public string ShortWalletAddress =>
        string.IsNullOrWhiteSpace(StoreWalletAddress)
            ? string.Empty
            : $"{StoreWalletAddress[..6]}...{StoreWalletAddress[^4..]}";

    /// Unique identifier for the current order.
    public string OrderId { get; }

    /// Optional email address provided by the user for receipt delivery.
    [ObservableProperty] private string _emailAddress;

    /// Clears the current order and resets the form for a new transaction.
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

    /// <summary>
    /// Constructor that injects all required services and sets the order context.
    /// </summary>
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

    /// <summary>
    /// Generates a Solana QR code based on the order details.
    /// </summary>
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
            Console.WriteLine($"Final QR Payload: {solUri}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating QR Code: {ex.Message}");
        }
    }

    /// <summary>
    /// Triggered when the page is navigated to. Starts the transaction watcher and handles order saving and receipt.
    /// </summary>
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
                
                // Builds the order details for local storage
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

                // Saves to DB IMMEDIATELY after the order is confirmed
                await _orderStorageService.SaveOrderAsync(completedOrder);

                //Prompts for an email address to send the receipt if the user wants it
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