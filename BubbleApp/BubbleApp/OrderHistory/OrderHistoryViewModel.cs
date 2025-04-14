using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using BubbleLib;

namespace BubbleAppNew.OrderHistory
{
    public partial class OrderHistoryViewModel : ObservableObject
    {
        private readonly IOrderStorageService _orderStorageService;

        [ObservableProperty] private ObservableCollection<CompletedOrder> _orders;
        [ObservableProperty] private bool _isRefreshing;

        public OrderHistoryViewModel(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;
            Orders = new ObservableCollection<CompletedOrder>();
            LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsRefreshing = true;
                var ordersFromDb = await _orderStorageService.GetOrdersAsync();
                Orders = new ObservableCollection<CompletedOrder>(ordersFromDb.OrderByDescending(o => o.Timestamp));
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

        public async Task RefreshFromViewAsync()
        {
            await LoadOrdersAsync();        }
    }
}