using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BubbleLib.Models;

namespace BubbleAppNew.OrderSummaryPage;

public class OrderViewModel : INotifyPropertyChanged
{
    public ObservableCollection<BubbleWaffle> OrderedItems { get; set; }

    private decimal _totalEur;

    public decimal TotalEur
    {
        get => _totalEur;
        set
        {
            _totalEur = value;
            OnPropertyChanged();
        }
    }

    private decimal _solanaRate;

    public decimal SolanaRate
    {
        get => _solanaRate;
        set
        {
            _solanaRate = value;
            OnPropertyChanged();
        }
    }

    private decimal _totalSol;

    public decimal TotalSol
    {
        get => _totalSol;
        set
        {
            _totalSol = value;
            OnPropertyChanged();
        }
    }

    public OrderViewModel()
    {
        OrderedItems = new ObservableCollection<BubbleWaffle>();
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        TotalEur = 0;
        foreach (var item in OrderedItems)
        {
            TotalEur += item.WafflePrice;
        }

        SolanaRate = 20.0m;
        TotalSol = TotalEur / SolanaRate;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}