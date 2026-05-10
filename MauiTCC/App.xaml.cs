using MauiTCC.Services;

namespace MauiTCC;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    private async void IniciarBanco()
    {
        var dbService = new MauiTCC.Services.DatabaseService();
        // Isso apenas cria o arquivo .db3 e as tabelas
        await dbService.GetAgendamentosAsync();
    }
}
    
     
    
