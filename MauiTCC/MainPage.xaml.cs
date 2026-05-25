using System;
using Microsoft.Maui.Controls;

namespace MauiTCC
{
    public partial class MainPage : ContentPage
    {
        // Conexão com o serviço de banco de dados usando o caminho completo
        private readonly MauiTCC.Services.DatabaseService _dbService;

        public MainPage()
        {
            InitializeComponent();
            _dbService = new MauiTCC.Services.DatabaseService();
        }

        // Abre a tela de cadastro se o usuário clicar no link
        private async void OnCadastroClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroPage());
        }

        // Lógica principal do Botão de Login com Níveis de Acesso
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // 1. Captura as informações digitadas limpando espaços extras
            string cpfDigitado = (txtUsuarioLogin.Text ?? "").Trim();
            string senhaDigitada = txtSenhaLogin.Text ?? "";

            // 2. Validação visual imediata (impede campos vazios)
            if (string.IsNullOrEmpty(cpfDigitado) || string.IsNullOrEmpty(senhaDigitada))
            {
                await DisplayAlert("Campos Obrigatórios", "Por favor, digite seu CPF e senha.", "OK");
                return;
            }

            try
            {
                // 3. Consulta a tabela de Usuários no SQLite através do serviço
                var usuarioLogado = await _dbService.ValidarLoginComNivelAsync(cpfDigitado, senhaDigitada);

                // 4. Se encontrou o usuário, avalia o nível de acesso (Texto Simples)
                if (usuarioLogado != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🟢 LOGIN RECONHECIDO: {usuarioLogado.Nome} é {usuarioLogado.Tipo}");

                    switch (usuarioLogado.Tipo)
                    {
                        case "Administrador":
                            await DisplayAlert("Acesso Autorizado", $"Bem-vindo, Administrador: {usuarioLogado.Nome}", "OK");
                            // Alinhado com a DashboardPage que já existe no seu projeto!
                            await Navigation.PushAsync(new DashboardPage());
                            break;

                        case "Dentista":
                            await DisplayAlert("Acesso Autorizado", $"Olá, Dr(a). {usuarioLogado.Nome}", "OK");
                            // Direciona para a tela de prontuários, consultas e agenda médica
                            await Navigation.PushAsync(new DentistaDashboardPage());
                            break;

                        case "Recepcionista":
                            await DisplayAlert("Acesso Autorizado", $"Bem-vinda, Recepção: {usuarioLogado.Nome}", "OK");
                            // Direciona para o painel de marcação de consultas e fluxo de pacientes
                            await Navigation.PushAsync(new RecepcionistaDashboardPage());
                            break;

                        case "Paciente":
                            await DisplayAlert("Bem-vindo", usuarioLogado.Nome, "OK");
                             await Navigation.PushAsync(new PainelPacientePage());
                            break;

                        default:
                            // Caso exista um tipo gravado incorretamente no banco
                            await DisplayAlert("Erro de Permissão", "Seu nível de acesso não possui uma tela definida. Contate o suporte.", "OK");
                            break;
                    }
                }
                else
                {
                    // Se o banco retornar nulo (usuário ou senha errados)
                    await DisplayAlert("Falha no Login", "CPF ou senha incorretos. Tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Proteção contra falhas inesperadas de conexão com o SQLite
                await DisplayAlert("Erro no Sistema", "Não foi possível processar o login: " + ex.Message, "OK");
            }
        }
    }
}