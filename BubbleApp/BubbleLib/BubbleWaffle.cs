using CommunityToolkit.Mvvm.ComponentModel;

namespace BubbleLib.Models;

public partial class BubbleWaffle : ObservableObject
{
    [ObservableProperty]
    private int quantity = 1;

    [ObservableProperty]
    private decimal? customPrice;

    public string WaffleName { get; set; } = string.Empty;

    // Automatically fallback to default price if no custom price
    public decimal WafflePrice => CustomPrice ?? 8.00m;

    // Recalculate every time Quantity or Price changes
    public decimal TotalPrice => WafflePrice * Quantity;

    // Notify the UI that TotalPrice needs to update
    partial void OnQuantityChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }

    partial void OnCustomPriceChanged(decimal? oldValue, decimal? newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }
}