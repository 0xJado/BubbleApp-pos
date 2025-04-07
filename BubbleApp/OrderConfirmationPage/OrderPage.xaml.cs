namespace BubbleAppNew.OrderConfirmationPage;

public partial class OrderPage : ContentPage, IQueryAttributable
{
    public OrderPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("OrderConfirmationViewModel", out var ctx) && ctx is OrderConfirmationViewModel vm)
        {
            BindingContext = vm;
        }
        else
        {
            throw new ArgumentException("Invalid query parameter");
        }}
}