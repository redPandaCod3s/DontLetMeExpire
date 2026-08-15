using DontLetMeExpire.Views;

namespace DontLetMeExpire;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Registrierung der Navigationsrouten
        Routing.RegisterRoute(nameof(ItemPage), typeof(ItemPage));
    }
}