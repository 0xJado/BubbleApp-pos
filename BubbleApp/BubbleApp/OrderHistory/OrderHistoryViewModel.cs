using System.Collections.ObjectModel;
using BubbleLib;

namespace BubbleAppNew.OrderHistory
{
    /// <summary>
    /// ViewModel for displaying and refreshing the historical list of completed orders.
    /// </summary>
    public partial class OrderHistoryViewModel : ObservableObject
    {
        private readonly IOrderStorageService _orderStorageService;

        /// Observable list of all completed orders shown in the UI.
        [ObservableProperty] private ObservableCollection<CompletedOrder> _orders;

        /// Indicates whether the UI is currently in a refreshing/loading state.
        [ObservableProperty] private bool _isRefreshing;

        /// <summary>
        /// Initializes the viewmodel and immediately loads order history from local storage.
        /// </summary>
        public OrderHistoryViewModel(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;
            Orders = new ObservableCollection<CompletedOrder>();
            LoadOrdersAsync();
        }

        /// <summary>
        /// Loads completed orders from the SQLite database and updates the Orders collection.
        /// </summary>
        private async Task LoadOrdersAsync()
        {
            try
            {
                IsRefreshing = true;
                var ordersFromDb = await _orderStorageService.GetOrdersAsync();

                Orders.Clear();
                foreach (var order in ordersFromDb.OrderByDescending(o => o.Timestamp))
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading orders: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// Public method that allows the UI to trigger a refresh of the orders list.
        /// </summary>
        public async Task RefreshFromViewAsync()
        {
            await LoadOrdersAsync();
        }
    }
}