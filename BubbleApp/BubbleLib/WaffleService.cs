using System.Collections.ObjectModel;
using System.Linq;

namespace BubbleLib;

/// <summary>
/// Interface for managing waffle orders and menu items.
/// Provides methods for adding, removing, clearing, and calculating waffle order totals.
/// </summary>
public interface IWaffleService
{
    /// <summary>
    /// Gets the predefined list of available waffle menu items.
    /// </summary>
    ObservableCollection<BubbleWaffle> WaffleMenu { get; }

    /// <summary>
    /// Gets the current list of waffles added to the active order.
    /// </summary>
    ObservableCollection<BubbleWaffle> OrderedItems { get; }
    
    /// <summary>
    /// Adds a selected waffle to the order. Increments quantity if it already exists.
    /// </summary>
    /// <param name="waffle">The waffle item to be added to the order.</param>
    void AddWaffleToOrder(BubbleWaffle waffle);
    
    /// <summary>
    /// Removes a waffle from the order. Decreases quantity or removes it completely if only one remains.
    /// </summary>
    /// <param name="waffle">The waffle item to be removed from the order.</param>
    void RemoveWaffleFromOrder(BubbleWaffle waffle);
    
    /// <summary>
    /// Calculates the total price of all items in the current order.
    /// </summary>
    /// <returns>The total price of the current order in euros.</returns>
    decimal GetTotalPrice();

    /// <summary>
    /// Clears all items from the current order.
    /// </summary>
    void ClearOrder();
}

/// <summary>
/// Service for managing the waffle ordering process.
/// Handles adding, removing, and pricing logic for ordered items.
/// </summary>
public class WaffleService : IWaffleService
{
    /// <summary>
    /// Gets the list of predefined waffle menu items.
    /// </summary>
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

    /// <summary>
    /// Gets the collection of items currently added to the order.
    /// </summary>
    public ObservableCollection<BubbleWaffle> OrderedItems { get; } = new();

    /// <summary>
    /// Adds a waffle to the current order. If the item exists, its quantity is increased.
    /// </summary>
    /// <param name="waffle">The waffle to add to the order.</param>
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

    /// <summary>
    /// Removes one quantity of the specified waffle from the order.
    /// Removes the waffle entirely if only one exists.
    /// </summary>
    /// <param name="waffle">The waffle to remove from the order.</param>
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

    /// <summary>
    /// Calculates the total cost of all waffles in the current order.
    /// </summary>
    /// <returns>The total price of all items in the order.</returns>
    public decimal GetTotalPrice()
    {
        return OrderedItems.Sum(w => w.Quantity * w.WafflePrice);
    }

    /// <summary>
    /// Clears the current order, removing all items.
    /// </summary>
    public void ClearOrder()
    {
        OrderedItems.Clear();
    }
}