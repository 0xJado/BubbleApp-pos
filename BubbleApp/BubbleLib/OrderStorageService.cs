using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace BubbleLib;

/// <summary>
/// Interface for managing storage and retrieval of completed orders using SQLite.
/// </summary>
public interface IOrderStorageService
{
    /// <summary>
    /// Retrieves all completed orders from the database, sorted in descending order by timestamp.
    /// Depreciated: Kept in case we need to show all orders in a future build.
    /// </summary>
    /// <returns>A list of <see cref="CompletedOrder"/> instances representing all saved orders.</returns>
    Task<List<CompletedOrder>> GetOrdersAsync();

    /// <summary>
    /// Saves a completed order to the database.
    /// </summary>
    /// <param name="order">The order object to be saved.</param>
    /// <returns>An integer representing the number of rows affected.</returns>
    Task<int> SaveOrderAsync(CompletedOrder order);
    
    /// <summary>
    /// Retrieves orders that have either been confirmed or failed.
    /// </summary>
    /// <returns>A list of relevant <see cref="CompletedOrder"/> instances.</returns>
    Task<List<CompletedOrder>> GetRelevantOrdersAsync();
    
    Task<int> UpdateOrderStatusAsync(string orderId, string txSignature, PaymentStatus newStatus);

}

/// <summary>
/// Concrete implementation of <see cref="IOrderStorageService"/> using SQLite for persistent local storage.
/// </summary>
public class OrderStorageService : IOrderStorageService
{
    private readonly SQLiteAsyncConnection _database;

    /// <summary>
    /// Initializes the <see cref="OrderStorageService"/> with a database path and ensures the required table exists.
    /// </summary>
    /// <param name="dbPath">Path to the local SQLite database file.</param>
    public OrderStorageService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        // DROP and recreate the table (dev-only!)
        //_database.ExecuteAsync("DROP TABLE IF EXISTS CompletedOrder").Wait();
        _database.CreateTableAsync<CompletedOrder>().Wait();
    }

    /// <summary>
    /// Retrieves all orders from the database, sorted by descending timestamp (newest first).
    /// </summary>
    /// <returns>A task that represents the asynchronous fetch operation. The task result contains a list of orders.</returns>
    public async Task<List<CompletedOrder>> GetOrdersAsync()
    {
        return await _database.Table<CompletedOrder>()
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves only orders with confirmed or failed status, sorted by latest first.
    /// </summary>
    public async Task<List<CompletedOrder>> GetRelevantOrdersAsync()
    {
        return await _database.Table<CompletedOrder>()
            .Where(o => o.Status == PaymentStatus.Confirmed || o.Status == PaymentStatus.Failed)
            .OrderByDescending(o => o.Timestamp)
            .ToListAsync();
    }

    /// <summary>
    /// Saves a single <see cref="CompletedOrder"/> to the SQLite database.
    /// </summary>
    /// <param name="order">The order object to persist.</param>
    /// <returns>A task that represents the asynchronous insert operation. The result is the number of inserted rows.</returns>
    public Task<int> SaveOrderAsync(CompletedOrder order) =>
        _database.InsertAsync(order);
    
    public async Task<int> UpdateOrderStatusAsync(string orderId, string txSignature, PaymentStatus newStatus)
    {
        var order = await _database.Table<CompletedOrder>()
            .Where(o => o.OrderId == orderId)
            .FirstOrDefaultAsync();

        if (order is null) return 0;

        order.TxSignature = txSignature;
        order.Status = newStatus;
        return await _database.UpdateAsync(order);
    }
}