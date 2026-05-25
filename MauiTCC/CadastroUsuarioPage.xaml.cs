using Microsoft.Maui.Controls;
using System;
using System.Linq;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class CadastroUsuarioPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public CadastroUsuarioPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private async void OnCadastrarUsuarioClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) ||
                string.IsNullOrWhiteSpace(txtCPF.Text) ||
                string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                await DisplayAlert("Campos Obrigatórios", "Por favor, preencha todos os campos básicos.", "OK");
                return;
            }

            if (pckTipo.SelectedIndex == -1)
            {
                await DisplayAlert("Nível de Acesso", "Por favor, selecione o nível de acesso do usuário.", "OK");
                return;
            }

            string cpfLimpo = txtCPF.Text.Trim();
            if (cpfLimpo.Length != 11 || !cpfLimpo.All(char.IsDigit))
            {
                await DisplayAlert("CPF Inválido", "O CPF deve conter exatamente 11 dígitos numéricos.", "OK");
                return;
            }

            try
            {
                var novoUsuario = new Usuario
                {
                    Nome = txtNome.Text.Trim(),
                    CPF = cpfLimpo,
                    Senha = txtSenha.Text,
                    Tipo = pckTipo.SelectedItem.ToString()
                };

                await _dbService.SalvarUsuarioAsync(novoUsuario);

                await DisplayAlert("Sucesso", $"{novoUsuario.Tipo} cadastrado com sucesso!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao salvar usuário: " + ex.Message, "OK");
            }
        }
    }
}