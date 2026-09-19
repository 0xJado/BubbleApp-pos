using CommunityToolkit.Mvvm.ComponentModel;

namespace BubbleLib;

/// <summary>
/// Represents a single bubble waffle item, either from the menu or as part of an order.
/// Includes pricing logic, quantity tracking, and data binding support.
/// </summary>
public partial class BubbleWaffle : ObservableObject
{
    /// <summary>
    /// The number of waffle units ordered. Default is 1.
    /// </summary>
    [ObservableProperty]
    private int quantity = 1;

    /// <summary>
    /// Optional custom price per unit. If set, overrides the default price.
    /// </summary>
    [ObservableProperty]
    private decimal? customPrice;

    /// <summary>
    /// The name of the waffle item.
    /// </summary>
    public string WaffleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the price per unit. Falls back to a default price if no custom price is set.
    /// </summary>
    public decimal WafflePrice => CustomPrice ?? 8.00m;

    /// <summary>
    /// Gets the total price for this item based on quantity and unit price.
    /// </summary>
    public decimal TotalPrice => WafflePrice * Quantity;

    /// <summary>
    /// Triggered when the quantity changes to update the total price.
    /// </summary>
    /// <param name="oldValue">The old quantity value.</param>
    /// <param name="newValue">The new quantity value.</param>
    partial void OnQuantityChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }

    /// <summary>
    /// Triggered when the custom price changes to update the total price.
    /// </summary>
    /// <param name="oldValue">The previous custom price.</param>
    /// <param name="newValue">The new custom price.</param>
    partial void OnCustomPriceChanged(decimal? oldValue, decimal? newValue)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }
}