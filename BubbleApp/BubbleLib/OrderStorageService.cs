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
    /// </summary>
    /// <returns>A list of <see cref="CompletedOrder"/> instances representing all saved orders.</returns>
    Task<List<CompletedOrder>> GetOrdersAsync();

    /// <summary>
    /// Saves a completed order to the database.
    /// </summary>
    /// <param name="order">The order object to be saved.</param>
    /// <returns>An integer representing the number of rows affected.</returns>
    Task<int> SaveOrderAsync(CompletedOrder order);
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
    /// Saves a single <see cref="CompletedOrder"/> to the SQLite database.
    /// </summary>
    /// <param name="order">The order object to persist.</param>
    /// <returns>A task that represents the asynchronous insert operation. The result is the number of inserted rows.</returns>
    public Task<int> SaveOrderAsync(CompletedOrder order) =>
        _database.InsertAsync(order);
}