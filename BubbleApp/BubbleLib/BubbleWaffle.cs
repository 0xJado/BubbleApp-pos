using CommunityToolkit.Mvvm.ComponentModel;

namespace BubbleLib;

/// Represents a bubble waffle item in the menu or order.
public partial class BubbleWaffle : ObservableObject
{
    /// Number of units ordered (default: 1)
    [ObservableProperty]
    private int quantity = 1;

    /// Custom price per unit when ordering a custom item 
    [ObservableProperty]
    private decimal? customPrice;

    /// Name of the waffle item
    public string WaffleName { get; set; } = string.Empty;

    /// Automatically fallback to default price if no custom price
    public decimal WafflePrice => CustomPrice ?? 8.00m;

    /// Recalculate every time Quantity or Price changes
    public decimal TotalPrice => WafflePrice * Quantity;

    /// Notify the UI that TotalPrice needs to update
    partial void OnQuantityChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }

    partial void OnCustomPriceChanged(decimal? oldValue, decimal? newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }
}