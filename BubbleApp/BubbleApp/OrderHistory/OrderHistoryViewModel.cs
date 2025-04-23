using System.Collections.ObjectModel;
using BubbleLib;

namespace BubbleAppNew.OrderHistory
{
    /// <summary>
    /// ViewModel for displaying and refreshing the historical list of completed orders.
    /// Interfaces with the order storage service to load order data from the local database.
    /// </summary>
    public partial class OrderHistoryViewModel : ObservableObject
    {
        private readonly IOrderStorageService _orderStorageService;

        /// <summary>
        /// Collection of completed orders that is bound to the UI.
        /// </summary>
        [ObservableProperty] 
        private ObservableCollection<CompletedOrder> _orders;

        /// <summary>
        /// Flag indicating whether the order data is currently being refreshed.
        /// Useful for showing loading indicators in the UI.
        /// </summary>
        [ObservableProperty] 
        private bool _isRefreshing;

        /// <summary>
        /// Constructor that initializes the viewmodel and triggers the loading of order data.
        /// </summary>
        /// <param name="orderStorageService">Service for retrieving stored order data.</param>
        public OrderHistoryViewModel(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;
            Orders = new ObservableCollection<CompletedOrder>();
            LoadOrdersAsync();
        }

        /// <summary>
        /// Loads the list of completed orders from the database and updates the observable collection.
        /// Orders are sorted by timestamp in descending order to show the latest orders first.
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
        /// Exposed method for the UI to manually trigger a refresh of the order history.
        /// </summary>
        public async Task RefreshFromViewAsync()
        {
            await LoadOrdersAsync();
        }
    }
}