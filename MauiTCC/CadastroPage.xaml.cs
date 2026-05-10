using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC;

public partial class CadastroPage : ContentPage
{
    private readonly DatabaseService _dbService;

    public CadastroPage()
    {
        InitializeComponent();
        _dbService = new DatabaseService();
    }

    private async void OnFinalizarCadastroClicked(object sender, EventArgs e)
    {
        try
        {
            // 1. Criar o objeto com os dados dos campos
            var novoPaciente = new Paciente
            {
                Nome = txtNome.Text,
                CPF = txtCPF.Text,
                Telefone = txtTelefone.Text,
                Convenio = txtConvenio.Text,
                Senha = txtSenha.Text
            };

            // 2. Salvar no SQLite usando o serviço da classe
            await _dbService.SalvarPacienteAsync(novoPaciente);

            // 3. (Opcional) Buscar todos para conferir no Console (Log)
            var lista = await _dbService.GetPacientesAsync();

            foreach (var p in lista)
            {
                Console.WriteLine($"🔍 PACIENTE NO BANCO: {p.Nome} - CPF: {p.CPF}");
            }

            // 4. Feedback para o usuário
            await DisplayAlert("Sucesso", "Paciente cadastrado com sucesso!", "OK");

            // 5. Voltar para a tela anterior
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Falha ao salvar: " + ex.Message, "OK");
        }
    }
}