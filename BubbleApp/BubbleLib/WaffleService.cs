using System.Collections.ObjectModel;
using System.Linq;

namespace BubbleLib;

/// <summary>
/// Interface for managing waffle orders and menu items.
/// Provides methods for adding, removing, clearing, and calculating waffle order totals.
/// </summary>
public interface IWaffleService
{
    /// Gets the predefined list of available waffle menu items.
    ObservableCollection<BubbleWaffle> WaffleMenu { get; }

    /// Gets the current list of waffles added to the active order.
    ObservableCollection<BubbleWaffle> OrderedItems { get; }
    
    /// Adds a selected waffle to the order. Increments quantity if it already exists.
    void AddWaffleToOrder(BubbleWaffle waffle);
    
    /// Removes a waffle from the order. Decreases quantity or removes it completely if only one remains.
    void RemoveWaffleFromOrder(BubbleWaffle waffle);
    
    /// Calculates the total price of all items in the current order.
    decimal GetTotalPrice();

    /// Clears all items from the current order.
    void ClearOrder();
}
/// <summary>
/// Service for managing the waffle ordering process.
/// Handles adding, removing, and pricing logic for ordered items.
/// </summary>
public class WaffleService : IWaffleService
{
    public ObservableCollection<BubbleWaffle> WaffleMenu { get; } = new()
    {
        new BubbleWaffle { WaffleName = "Cookie Moment" },
        new BubbleWaffle { WaffleName = "Latte Macchiato Moment" },
        new BubbleWaffle { WaffleName = "Caramel Moment" },
        new BubbleWaffle { WaffleName = "Berry Moment" },
        new BubbleWaffle { WaffleName = "Flamingo Moment" },
        new BubbleWaffle { WaffleName = "Banana Split Moment" },
        new BubbleWaffle { WaffleName = "Schlumpf Moment" },
        new BubbleWaffle { WaffleName = "Kids Moment" }
    };
    
    public void ClearOrder()
    {
        OrderedItems.Clear();
    }
    
    public ObservableCollection<BubbleWaffle> OrderedItems { get; } = new();
    
    public void AddWaffleToOrder(BubbleWaffle waffle)
    {
        var existingWaffle = OrderedItems.FirstOrDefault(w => w.WaffleName == waffle.WaffleName);
        if (existingWaffle != null)
        {
            existingWaffle.Quantity++;
        }
        else
        {
            OrderedItems.Add(new BubbleWaffle 
            { 
                WaffleName = waffle.WaffleName, 
                Quantity = 1,
                CustomPrice = waffle.CustomPrice
            });
        }
    }
    public void RemoveWaffleFromOrder(BubbleWaffle waffle)
    {
        var existingWaffle = OrderedItems.FirstOrDefault(w => w.WaffleName == waffle.WaffleName);
        if (existingWaffle != null)
        {
            if (existingWaffle.Quantity > 1)
            {
                existingWaffle.Quantity--;
            }
            else
            {
                OrderedItems.Remove(existingWaffle);
            }
        }
    }
    
    public decimal GetTotalPrice()
    {
        return OrderedItems.Sum(w => w.Quantity * w.WafflePrice);
    }
}