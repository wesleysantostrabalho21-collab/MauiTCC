using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class CadastroDentistaPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public CadastroDentistaPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            LimparCampos();
        }

        private void LimparCampos()
        {
            txtNome.Text = string.Empty;
            txtCPF.Text = string.Empty;
            txtSenha.Text = string.Empty;
            txtCro.Text = string.Empty;
            txtEspecialidade.Text = string.Empty;
        }

        private async void OnSalvarDentistaClicked(object sender, EventArgs e)
        {
            if (!await ValidarFormularioAsync())
            {
                return; // Para a execução se houver erros de validação
            }

            try
            {
                // Criar o objeto Usuario (Geral para Login)
                var usuario = new Usuario
                {
                    Nome = txtNome.Text.Trim(),
                    CPF = txtCPF.Text.Trim(),
                    Senha = txtSenha.Text,
                    Tipo = "Dentista" // Definido por segurança
                };

                // Criar o objeto Dentista (Dados clínicos)
                var dentista = new Dentista
                {
                    Cro = txtCro.Text.Trim(),
                    Especialidade = txtEspecialidade.Text.Trim()
                };

                // Salva os dois de forma encadeada no SQLite
                bool sucesso = await _dbService.SalvarDentistaCompletoAsync(usuario, dentista);

                if (sucesso)
                {
                    await DisplayAlert("Sucesso", "Dentista e usuário criados com sucesso!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível salvar os dados. Verifique se o CRO já existe.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha no sistema: " + ex.Message, "OK");
            }
        }

        private async Task<bool> ValidarFormularioAsync()
        {
            string nome = (txtNome.Text ?? "").Trim();
            string cpf = (txtCPF.Text ?? "").Trim();
            string senha = (txtSenha.Text ?? "");
            string cro = (txtCro.Text ?? "").Trim();
            string especialidade = (txtEspecialidade.Text ?? "").Trim();

            // 1. Campos vazios
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cpf) ||
                string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(cro) ||
                string.IsNullOrEmpty(especialidade))
            {
                await DisplayAlert("Campos Obrigatórios", "Por favor, preencha todas as informações do Dentista.", "OK");
                return false;
            }

            // 2. Validação do CPF
            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            {
                await DisplayAlert("CPF Inválido", "O CPF deve conter 11 dígitos numéricos.", "OK");
                return false;
            }

            // 3. Validação de Senha
            if (senha.Length < 6)
            {
                await DisplayAlert("Senha Fraca", "A senha deve ter no mínimo 6 caracteres.", "OK");
                return false;
            }

            return true;
        }
    }
}