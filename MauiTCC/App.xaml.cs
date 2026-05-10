namespace MauiTCC;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Isso permite navegar da Login para o Cadastro com o botão voltar
        MainPage = new NavigationPage(new MainPage());
    }
}