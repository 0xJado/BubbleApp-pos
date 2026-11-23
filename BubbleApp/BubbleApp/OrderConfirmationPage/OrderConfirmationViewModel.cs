using System.Collections.ObjectModel;
using System.Globalization;
using BubbleLib;
using Solnet.Rpc;

namespace BubbleAppNew.OrderConfirmationPage;

/// <summary>
/// ViewModel responsible for managing the order confirmation page, including
/// QR code generation, payment monitoring, and order persistence.
/// </summary>
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
    [ObservableProperty] private string _emailAddress;

    /// <summary>
    /// Gets the total price in Solana based on the Euro amount and the current SOL exchange rate.
    /// </summary>
    public decimal TotalSol => Math.Round(EuroPrice / SolanaRate, 3);

    /// <summary>
    /// Provides a shortened display version of the wallet address.
    /// </summary>
    public string ShortWalletAddress =>
        string.IsNullOrWhiteSpace(StoreWalletAddress)
            ? string.Empty
            : $"{StoreWalletAddress[..6]}...{StoreWalletAddress[^4..]}";

    /// <summary>
    /// Gets the unique identifier for the current order session.
    /// </summary>
    public string OrderId { get; }

    private readonly IOrderStorageService _orderStorageService;
    private readonly IWaffleService _waffleService;
    private readonly IEmailService _emailService;
    private readonly ITxnTracker _txnTracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderConfirmationViewModel"/> class.
    /// </summary>
    /// <param name="orderedItems">List of items included in the order.</param>
    /// <param name="euroPrice">Total price in euros.</param>
    /// <param name="solanaRate">Current SOL to EUR exchange rate.</param>
    /// <param name="storeWalletAddress">Destination wallet address for payment.</param>
    /// <param name="orderStorageService">Service for storing completed order data.</param>
    /// <param name="waffleService">Service for managing the order state.</param>
    /// <param name="emailService">Service for sending email receipts.</param>
    public OrderConfirmationViewModel(
        ObservableCollection<BubbleWaffle> orderedItems,
        decimal euroPrice,
        decimal solanaRate,
        string storeWalletAddress,
        IOrderStorageService orderStorageService,
        IWaffleService waffleService,
        IEmailService emailService,
        ITxnTracker txnTracker)
    {
        OrderedItems = orderedItems;
        EuroPrice = euroPrice;
        SolanaRate = solanaRate;
        StoreWalletAddress = storeWalletAddress;
        OrderId = Guid.NewGuid().ToString().Split('-')[0];

        _orderStorageService = orderStorageService;
        _waffleService = waffleService;
        _emailService = emailService;
        _txnTracker = txnTracker;
    }

    /// <summary>
    /// Clears the current order and resets the input for a new transaction session.
    /// </summary>
    [RelayCommand]
    private async Task NewTransactionAsync()
    {
        _waffleService.ClearOrder();
        EmailAddress = string.Empty;
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Generates a Solana-compatible QR code for the payment request.
    /// </summary>
    /// <param name="qrCodeService">Service used to generate the QR code image.</param>
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
    /// Handles parameter injection on navigation and monitors the transaction status on the Solana blockchain.
    /// </summary>
    /// <param name="query">Dictionary of query parameters from navigation.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Console.WriteLine("Applying query attributes.");
        IsWaitingForPayment = true;

        var pendingOrder = new CompletedOrder
        {
            OrderId = OrderId,
            ItemsSummary = string.Join(", ", OrderedItems.Select(x => $"{x.Quantity}x {x.WaffleName}")),
            TotalEur = EuroPrice,
            TotalSol = TotalSol,
            Wallet = StoreWalletAddress,
            Timestamp = DateTime.Now,
            Status = PaymentStatus.Pending
        };

        // Save the pending order as soon as the QR is shown
        await _orderStorageService.SaveOrderAsync(pendingOrder);

        try
        {
            string? txSignature = await _txnTracker.WaitForPaymentAsync(StoreWalletAddress, TotalSol, OrderId, TimeSpan.FromMinutes(3));

            if (!string.IsNullOrEmpty(txSignature))
            {
                PaymentConfirmed = true;

                // Update and overwrite the previous pending entry with confirmed status
                pendingOrder.TxSignature = txSignature;
                pendingOrder.Status = PaymentStatus.Confirmed;
                await _orderStorageService.UpdateOrderStatusAsync(OrderId, txSignature, PaymentStatus.Confirmed);

                bool wantsEmail = await Shell.Current.DisplayAlertAsync(
                    "Zahlung erfolgreich",
                    "Möchten Sie eine Quittung per E-Mail erhalten?",
                    "Ja", "Nein");

                if (wantsEmail)
                {
                    var email = await Shell.Current.DisplayPromptAsync("E-Mail eingeben",
                        "Bitte geben Sie Ihre E-Mail-Adresse ein:");
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await _emailService.SendReceiptAsync(email, pendingOrder, OrderedItems.ToList());
                    }
                }
            }
            else
            {
                PaymentTimedOut = true;

                // Update the pending order to Failed
                pendingOrder.Status = PaymentStatus.Failed;
                await _orderStorageService.UpdateOrderStatusAsync(OrderId, string.Empty, PaymentStatus.Failed);
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