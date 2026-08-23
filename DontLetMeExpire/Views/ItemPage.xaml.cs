using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DontLetMeExpire.ViewModels;

namespace DontLetMeExpire.Views;
[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class ItemPage : ContentPage
{
    private ItemViewModel _viewModel;
    public ItemPage(ItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    
    public string? ItemId { get; set; }

    override protected async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await _viewModel.InitializeAsync(ItemId);
        base.OnNavigatedTo(args);
    }
}