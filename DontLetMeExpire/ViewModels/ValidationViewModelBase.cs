using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DontLetMeExpire.ViewModels;

//Basisklasse für alle ViewModels die eine Validierung benötigen
public abstract class ValidationViewModelBase : ViewModelBase
{
    // Dicitionary welches alle Fehler enthält
    private readonly Dictionary<string, List<ValidationResult>> _errors = [];
    private readonly ValidationContext _validationContext;

    protected ValidationViewModelBase()
    {
        _validationContext = new ValidationContext(this);
    }
    
    // gibt alle Fehler zurück
    public IEnumerable<string> ErrorMessages =>
        _errors.Values.SelectMany(errors => errors.Select(error => error.ErrorMessage));
    
    // gibt zurück, ob es Validierungsfehler gibt
    public bool HasErrors => _errors.Any();
    
    // liefert ein Dictionary über alle fehlerhaften Eigenschaften
    public Dictionary<string, IEnumerable<string>> PropertyErrorMessages
    {
        get
        {
            return _errors.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.Select(error => error.ErrorMessage).AsEnumerable()
            );
        }
    }
    
    // Ersetzt die alte PropertyErrorMessages-Property
    public List<string> this[string propertyName] =>
        _errors.TryGetValue(propertyName, out var errors)
            ? errors.Select(e => e.ErrorMessage).ToList()
            : [];
    
    // Fügt der Liste von Fehlern alle Fehler einer Eigenschaft hinzu
    protected virtual void AddErrors(string propertyName, 
                                    List<ValidationResult> validationResults)
    {
        // Falls es die gleichen Fehler bereits in der Liste gibt,
        // springen wir direkt aus dem Code heraus
        if (_errors.TryGetValue(propertyName, out var existingErrors) && 
            existingErrors.Select(e => e.ErrorMessage)
                .SequenceEqual(validationResults.Select(e => e.ErrorMessage)))
        {
            return;
        }
        
        _errors[propertyName] = validationResults;
        
        // UI über die Änderung der Fehler informieren
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(ErrorMessages));
        OnPropertyChanged(nameof(PropertyErrorMessages));
        OnPropertyChanged("Item[]");
    }
    
    // Entfernt alle Fehler einer Eigenschaft aus der Liste der Fehler
    protected virtual void RemoveErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName) && _errors[propertyName].Count > 0)
        {
            _errors.Remove(propertyName);
            
            // UI über die Änderung der Fehler informieren
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(PropertyErrorMessages));
            OnPropertyChanged("Item[]");
        }
    }
    
    // Validiert alle Eigenschaften des ViewModel
    protected virtual bool ValidateAllProperties()
    {
        var isValid = true;
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            isValid = isValid && ValidateProperty(value, property.Name);
        }
        return isValid;
    }
    
    // Validiert eine EIgenschaft
    protected virtual bool ValidateProperty(object? value,
        [CallerMemberName] string propertyName = null!)
    {
        var validationResults = new List<ValidationResult>();
        _validationContext.MemberName = propertyName;
        
        // Eigenschaft validieren
        var isValid = Validator.TryValidateProperty(value,
            _validationContext, validationResults);

        if (isValid)
        {
            RemoveErrors(propertyName);
        }
        else
        {
            AddErrors(propertyName, validationResults);
        }
        return isValid;
    }
    
    // Validiert das ganze ViewModel
    public bool Validate()
    {
        var validationResults = new List<ValidationResult>();
        
        // ValidationContext für das aktuelle Onjekt erstellen
        var context = new ValidationContext(this);
        
        // Validierung über alle Eigenschaften des Objekts durchführen
        bool isValid = Validator.TryValidateObject(this, context, 
                                    validationResults, true);
        
        _errors.Clear();
        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    AddErrors(memberName, [validationResult]);
                }
            }
        }
        
        // UI über die Änderung der Fehler informieren
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(ErrorMessages));
        OnPropertyChanged(nameof(PropertyErrorMessages));
        OnPropertyChanged("Item[]");
        return isValid;
    }
}