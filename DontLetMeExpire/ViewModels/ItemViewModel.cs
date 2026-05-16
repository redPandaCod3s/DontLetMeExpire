using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DontLetMeExpire.Models;
using DontLetMeExpire.Services;

namespace DontLetMeExpire.ViewModels;

public class ItemViewModel : ValidationViewModelBase
{
    private string _name;
    private DateTime _expirationDate;
    private StorageLocation _location;
    private decimal _amount;
    private string _image;
    
    private readonly IStorageLocationService _storageLocationService;
    private readonly IItemService _itemService;

    public ItemViewModel(IStorageLocationService storageLocationService, IItemService itemService)
    {
            SaveCommand = new Command (async () => await SaveAsync());
            _storageLocationService = storageLocationService;
            _itemService = itemService;
    }
    
    public ObservableCollection<StorageLocation> StorageLocations { get; set; } = [];
    
    /// <summary>
    ///
    /// </summary>
    public Command  SaveCommand { get; private set; }

    /// <summary>
    /// Name des Elements
    /// </summary>
    [Required]
    [Length(1,50)]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value, SaveCommand.ChangeCanExecute);
    }
    
    /// <summary>
    /// Ablaufdatum des Elements
    /// </summary>
    public DateTime ExpirationDate
    {
        get => _expirationDate;
        set => SetProperty(ref _expirationDate, value);
    }

    /// <summary>
    /// ausgewählter Speicherort des Elements
    /// </summary>
    public StorageLocation SelectedLocation
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }
    
    /// <summary>
    /// Menge des Elements
    /// </summary>
    [Range(1,10000)]
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value, SaveCommand.ChangeCanExecute);
    }

    /// <summary>
    /// Bild des Elements
    /// </summary>
    public string Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    /// <summary>
    /// Initialisiert das ViewModel asynchron
    /// </summary>
    public async Task InitializeAsync()
    {
        //Speicherort laden
        var locations = await _storageLocationService.GetAsync();
        
        // die Liste der Speicherorte aktualisieren
        StorageLocations.Clear();
        foreach (var location in locations)
        {
            StorageLocations.Add(location);
        }
        SelectedLocation = StorageLocations.FirstOrDefault();
    }

    /// <summary>
    /// speichert das Element asynchron
    /// </summary>
    private async Task SaveAsync()
    {
        if(!Validate())
            return;
        
        // neues Element mit den Daten des ViewModels erstellen
        var item = new Item
        {
            Name = Name,
            ExpirationDate = ExpirationDate,
            Amount = Amount,
            Image = Image
        };
        
        // Element speichern
        await _itemService.SaveAsync(item);
        
        // Daten für die Anzeige zurücksetzen
        Name = string.Empty;
        ExpirationDate = DateTime.Today;
        Amount = 0;
        SelectedLocation = StorageLocations.First();
    }

    private bool CanSave()
    {
        return !string.IsNullOrEmpty(Name) && Amount > 0;
    }
}