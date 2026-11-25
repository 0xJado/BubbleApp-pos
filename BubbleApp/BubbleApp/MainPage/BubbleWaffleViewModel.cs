using System.Collections.ObjectModel;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib;

namespace BubbleAppNew.MainPage;

/// <summary>
/// ViewModel for the main order interface. Handles the ordering process,
/// price calculation, and navigation to the order confirmation view.
/// </summary>
public partial class BubbleWaffleViewModel : ObservableObject
{
    private readonly IWaffleService _waffleService;
    private readonly ICoinGeckoService _coinGeckoService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IOrderStorageService _orderStorageService;
    private readonly IEmailService _emailService;
    private readonly ITxnTracker _txnTracker;

    /// <summary>
    /// Gets the list of available waffle items from the menu.
    /// </summary>
    public ObservableCollection<BubbleWaffle> WaffleMenu => _waffleService.WaffleMenu;

    /// <summary>
    /// Gets the collection of waffle items currently added to the order.
    /// </summary>
    public ObservableCollection<BubbleWaffle> OrderedItems => _waffleService.OrderedItems;

    /// <summary>
    /// The total price of the current order in euros.
    /// </summary>
    [ObservableProperty] private decimal totalPrice;

    /// <summary>
    /// The converted price of the order in Solana.
    /// </summary>
    [ObservableProperty] private decimal solanaPrice;

    /// <summary>
    /// Initializes a new instance of the <see cref="BubbleWaffleViewModel"/> class.
    /// </summary>
    /// <param name="waffleService">Service for managing waffle orders and menu items.</param>
    /// <param name="coinGeckoService">Service for retrieving SOL to EUR exchange rate.</param>
    /// <param name="qrCodeService">Service for generating QR codes for payments.</param>
    /// <param name="orderStorageService">Service for storing completed orders.</param>
    /// <param name="emailService">Service for sending order receipts via email.</param>
    public BubbleWaffleViewModel(
        IWaffleService waffleService,
        ICoinGeckoService coinGeckoService,
        IQrCodeService qrCodeService,
        IOrderStorageService orderStorageService,
        IEmailService emailService,
        ITxnTracker txnTracker)
    {
        _waffleService = waffleService ?? throw new ArgumentNullException(nameof(waffleService), "Der WaffleService darf nicht null sein.");
        _coinGeckoService = coinGeckoService ?? throw new ArgumentNullException(nameof(coinGeckoService));
        _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        _orderStorageService = orderStorageService ?? throw new ArgumentNullException(nameof(orderStorageService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _txnTracker = txnTracker ?? throw new ArgumentNullException(nameof(txnTracker));

    }

    /// <summary>
    /// Adds a waffle item to the current order and updates the displayed prices.
    /// </summary>
    /// <param name="waffle">The waffle item to add.</param>
    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task AddWaffleToOrderAsync(BubbleWaffle waffle)
    {
        _waffleService.AddWaffleToOrder(waffle);
        await UpdatePricesAsync();
    }

    /// <summary>
    /// Removes a waffle item from the current order and updates the displayed prices.
    /// </summary>
    /// <param name="waffle">The waffle item to remove.</param>
    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task RemoveWaffleFromOrderAsync(BubbleWaffle? waffle)
    {
        if (waffle == null) return;
        _waffleService.RemoveWaffleFromOrder(waffle);
        await UpdatePricesAsync();
    }

    /// <summary>
    /// Recalculates the total price in euros and converts it to Solana.
    /// </summary>
    private async Task UpdatePricesAsync()
    {
        TotalPrice = _waffleService.GetTotalPrice();
        if (TotalPrice == 0)
        {
            SolanaPrice = 0;
            return;
        }

        try
        {
            var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
            SolanaPrice = solanaRate > 0 ? TotalPrice / solanaRate : 0;
        }
        catch (InvalidOperationException ex)
        {
            SolanaPrice = 0;
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Navigates to the order confirmation page with all relevant data for finalizing the order.
    /// </summary>
    [RelayCommand]
    private async Task GenerateQrCodeAsync()
    {
        Console.WriteLine("---> QR Code Generation Clicked");
        var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        Console.WriteLine($"---> Solana Rate: {solanaRate}");

        var orderedItems = new ObservableCollection<BubbleWaffle>(_waffleService.OrderedItems);
        Console.WriteLine($"---> Items in Order: {orderedItems.Count}");

        var euroPrice = _waffleService.GetTotalPrice();
        Console.WriteLine($"---> Euro Price: {euroPrice}");

        var storeWalletAddress = "AvgjRNHhKgZ6pb5AxeqBdCDiZY1Rh9yUnCTh464oA4Cn";

        var ocvm = new OrderConfirmationViewModel(
            orderedItems,
            euroPrice,
            solanaRate,
            storeWalletAddress,
            _orderStorageService,
            _waffleService,
            _emailService,
            _txnTracker
        );
        
        await Shell.Current.GoToAsync($"{nameof(OrderPage)}", true,
            new Dictionary<string, object>
            {
                [nameof(OrderConfirmationViewModel)] = ocvm
            });

        Console.WriteLine("---> OrderPage Navigation Completed");
    }
}