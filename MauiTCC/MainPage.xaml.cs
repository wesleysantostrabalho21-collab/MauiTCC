using MauiTCC.Services; // Adicione este using para enxergar o DatabaseService

namespace MauiTCC;

public partial class MainPage : ContentPage
{
    // 1. Declarar o serviço de banco de dados
    private readonly DatabaseService _dbService;

    public MainPage()
    {
        InitializeComponent();

        // 2. Inicializar o serviço
        _dbService = new DatabaseService();
    }

    private async void OnCadastroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPage());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Obtém o que foi digitado nos campos que nomeamos no XAML
        string cpfDigitado = txtUsuarioLogin.Text;
        string senhaDigitada = txtSenhaLogin.Text;

        if (string.IsNullOrEmpty(cpfDigitado) || string.IsNullOrEmpty(senhaDigitada))
        {
            await DisplayAlert("Aviso", "Preencha todos os campos!", "OK");
            return;
        }

        try
        {
            // 3. Chama o banco para validar
            // Importante: Certifique-se que o método ValidarLoginAsync no seu DatabaseService 
            // está comparando o CPF (veja o ajuste abaixo)
            bool loginSucesso = await _dbService.ValidarLoginAsync(cpfDigitado, senhaDigitada);

            if (loginSucesso)
            {
                await Navigation.PushAsync(new DashboardPage());
            }
            else
            {
                await DisplayAlert("Erro", "CPF ou senha incorretos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Erro ao acessar banco: " + ex.Message, "OK");
        }
    }
}