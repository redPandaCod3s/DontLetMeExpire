using DontLetMeExpire.Models;

namespace DontLetMeExpire.Services;

public static class DummyData
{
    public static List<StorageLocation> Locations { get; } =
    [
        new StorageLocation
        {
            Id = "f764f62e-d61c-48d4-ac2a-bcd8e1dce89e",
            Name = GetTranslation("Freezer", "Gefrierschrank"),
            Icon = "\uf166"
        },

        new StorageLocation
        {
            Id = "",
            Name = GetTranslation("Pantry", "Vorratsschrank"),
            Icon = "\uf86e"
        }
    ];

    public static List<Item> Items { get; } =
    [
        new Item
        {
            Id = "ea0b468a-220c-470f-aff6-3ebf7eadbfce",
            Name = GetTranslation("Leaf spinach 1lb (frozen)", "Blattspinat 500 g (TK)"),
            ExpirationDate = DateTime.Today,
            Amount = 1,
        },
        
        new Item
        {
            Id = "d1a7396a-9198-46ee-b541-f0e180911b7b",
            Name = GetTranslation("Rice","Reis"),
            ExpirationDate = DateTime.Today.AddDays(-25),
            Amount = 1,
        }
    ];

    private static string GetTranslation(string english, string german)
    {
        return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "de" ? german :  english;
    }

}