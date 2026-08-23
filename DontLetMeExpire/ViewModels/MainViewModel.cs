using System.Collections.ObjectModel;
using DontLetMeExpire.Services;
using DontLetMeExpire.Views;
using System.Windows.Input;
using DontLetMeExpire.Models;

namespace DontLetMeExpire.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IItemService _itemService;
    private readonly INavigationService _navigationService;
    private readonly IStorageLocationService _storageLocationService;
    
    private int _stockCount;
    private int _expiringSoonCount;
    private int _expiresTodayCount;
    private int _expiredCount;
    
    public ICommand NavigateToAddItemCommand { get; }
    public ICommand NavigateToStockCommand { get; }
    public ICommand NavigateToExpiringSoonCommand { get; }
    public ICommand NavigateToExpiresTodayCommand { get; }
    public ICommand NavigateToExpiredCommand { get; }
    public ICommand NavigateToLocationCommand { get; }
    
    public ICommand CreateSuggestedLocationsCommand { get; }
    public ICommand CreateSuggestedLocationsAndStockCommand { get; }
    
    public MainViewModel(INavigationService navigationService, IItemService itemService,
        IStorageLocationService storageLocationService)
    {
        _itemService = itemService;
        _storageLocationService = storageLocationService;
        _navigationService = navigationService;

        CreateSuggestedLocationsCommand = new Command(async () => await CreateSuggestedLocations());
        CreateSuggestedLocationsAndStockCommand = new Command(async () => await CreateSuggestedLocationsAndStock());
        
        // Initialisierung der neuen Navigations-Commands
        NavigateToAddItemCommand = new Command(async () => await NavigateToAddItem(), CanAddNewItem);
        NavigateToStockCommand = new Command(async () => await NavigateToStock());
        NavigateToExpiringSoonCommand = new Command(async () => await NavigateToExpiringSoon());
        NavigateToExpiresTodayCommand = new Command(async () => await NavigateToExpiresToday());
        NavigateToExpiredCommand = new Command(async () => await NavigateToExpired());
        NavigateToLocationCommand = new Command<string>(async (locationId) => await NavigateToLocation(locationId));
    }

    private async Task NavigateToAddItem()
    {
        await _navigationService.GoToAsync(nameof(ItemPage));
    }

    public ObservableCollection<StorageLocationWithItemCount> StorageLocations { get; } = [];

    /// <summary>
    /// Die Anzahl aller gelagerten Artikel
    /// </summary>
    public int StockCount
    {
        get => _stockCount;
        set => SetProperty(ref _stockCount, value);
    }

    /// <summary>
    /// Die Anzahl der Artikel, die bald ablaufen
    /// </summary>
    public int ExpiringSoonCount
    {
        get => _expiringSoonCount;
        set => SetProperty(ref _expiringSoonCount, value);
    }

    /// <summary>
    ///Die Anzahl Artikel, die heute ablaufen
    /// </summary>
    public int ExpiresTodayCount
    {
        get => _expiresTodayCount;
        set => SetProperty(ref _expiresTodayCount, value);
    }
    
    /// <summary>
    ///
    /// </summary>
    public int ExpiredCount
    {
        get => _expiredCount;
        set => SetProperty(ref _expiredCount, value);
    }

    ///<summary>
    ///
    /// </summary>
    public async Task InitializeAsync()
    {
        var locations = await _storageLocationService.GetWithItemCountAsync();
        
        StorageLocations.Clear();
        foreach (var location in locations)
        {
            StorageLocations.Add(location);
        }
        
        StockCount = (await _itemService.GetAsync()).Count();
        ExpiringSoonCount = (await _itemService.GetExpiresSoonAsync()).Count();
        ExpiresTodayCount = (await _itemService.GetExpiresTodayAsync()).Count();
        ExpiredCount = (await _itemService.GetExpiredAsync()).Count();
        
        // Überprüfen, ob das NavigateToAddItemCommand mit den neuen Daten ausgeführt werden kann
        ((Command) NavigateToAddItemCommand).ChangeCanExecute();
    }
    
    // Prüfen, ob neuer Vorratseintrag angelegt werden kann
    private bool CanAddNewItem()
    {
        // Neue Vorratseinträge können nur angelegt werden, wenn es auch Lagerorte gibt
        return StorageLocations.Any();
    }
    
    private async Task NavigateToStock()
    {
        await NavigateToItemsPage("Stock");
    }
    
    private async Task NavigateToLocation(string locationId)
    {
        await NavigateToItemsPage("Location", locationId);
    }

    private async Task NavigateToExpired()
    {
        await NavigateToItemsPage("Expired");
    }

    private async Task NavigateToExpiresToday()
    {
        await NavigateToItemsPage("ExpiresToday");
    }

    private async Task NavigateToExpiringSoon()
    {
        await NavigateToItemsPage("ExpiringSoon");
    }

    private async Task NavigateToItemsPage(string displayMode, string? locationId = null)
    {
        var navigationParams = new Dictionary<string, object>
        {
            {"DisplayMode", displayMode},
            {"LocationId", locationId}
        };
        
        await _navigationService.GoToAsync(nameof(ItemsPage), navigationParams);
    }

    // Beispiellagerorte anlegen
    private async Task CreateSuggestedLocations()
    {
        foreach (var location in DummyData.Locations)
        {
            await _storageLocationService.SaveAsync(location);
        }

        await InitializeAsync();
    }
    
    // Beispiellagerorte und -vorräte anlegen
    private async Task CreateSuggestedLocationsAndStock()
    {
        foreach (var location in DummyData.Locations)
        {
            await _storageLocationService.SaveAsync(location);
        }
        
        foreach( var item in DummyData.Items)
        {
            await _itemService.SaveAsync(item);
        }
        
        await InitializeAsync();
    }
    
}