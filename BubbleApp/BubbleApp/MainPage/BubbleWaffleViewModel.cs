using System.Collections.ObjectModel;
using BubbleAppNew.OrderConfirmationPage;
using BubbleLib;

namespace BubbleAppNew.MainPage;

/// <summary>
/// ViewModel for the main order interface. Handles the ordering process, 
/// price calculation, and transition to the confirmation view.
/// </summary>
public partial class BubbleWaffleViewModel : ObservableObject
{
    private readonly IWaffleService _waffleService;
    private readonly ICoinGeckoService _coinGeckoService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IOrderStorageService _orderStorageService;
    private readonly IEmailService _emailService;

    /// Reference to the available waffle menu.
    public ObservableCollection<BubbleWaffle> WaffleMenu => _waffleService.WaffleMenu;

    /// Current items added to the order.
    public ObservableCollection<BubbleWaffle> OrderedItems => _waffleService.OrderedItems;

    /// Current total price in EUR.
    [ObservableProperty] private decimal totalPrice;

    /// Calculated price in Solana.
    [ObservableProperty] private decimal solanaPrice;

    /// <summary>
    /// Initializes the viewmodel and its dependencies, and sets the initial prices.
    /// </summary>
    public BubbleWaffleViewModel(
        IWaffleService waffleService,
        ICoinGeckoService coinGeckoService,
        IQrCodeService qrCodeService,
        IOrderStorageService orderStorageService,
        IEmailService emailService)
    {
        _waffleService = waffleService ?? throw new ArgumentNullException(nameof(waffleService), "Der WaffleService darf nicht null sein.");
        _coinGeckoService = coinGeckoService ?? throw new ArgumentNullException(nameof(coinGeckoService));
        _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        _orderStorageService = orderStorageService ?? throw new ArgumentNullException(nameof(orderStorageService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        
        UpdatePricesAsync();
    }

    /// Adds a waffle item to the order and updates the total.
    [RelayCommand]
    public void AddWaffleToOrder(BubbleWaffle waffle)
    {
        _waffleService.AddWaffleToOrder(waffle);
        UpdatePricesAsync();
    }

    /// Removes a waffle item from the order and updates the total.
    [RelayCommand]
    private void RemoveWaffleFromOrder(BubbleWaffle? waffle)
    {
        if (waffle == null) return;
        _waffleService.RemoveWaffleFromOrder(waffle);
        UpdatePricesAsync();
    }
    
    /// Updates the total price and recalculates the equivalent Solana value.
    private async Task UpdatePricesAsync()
    {
        TotalPrice = _waffleService.GetTotalPrice();
        var solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        SolanaPrice = TotalPrice / solanaRate;
    }

    /// <summary>
    /// Navigates to the order confirmation page and passes the required order data.
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
            _emailService
        );
        
        await Shell.Current.GoToAsync($"{nameof(OrderPage)}", true,
            new Dictionary<string, object>
            {
                [nameof(OrderConfirmationViewModel)] = ocvm
            });

        Console.WriteLine("---> OrderPage Navigation Completed");
    }
}