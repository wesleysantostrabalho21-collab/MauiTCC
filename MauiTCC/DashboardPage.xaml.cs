namespace MauiTCC;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    private async void OnSairClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Volta para a tela de Login
    }
}