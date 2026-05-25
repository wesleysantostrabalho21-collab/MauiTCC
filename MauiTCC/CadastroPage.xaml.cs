using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class CadastroPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public CadastroPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            // Força o MAUI a entender que os campos começam vazios.
            LimparCampos();
        }

        private void LimparCampos()
        {
            txtNome.Text = string.Empty;
            txtCPF.Text = string.Empty;
            txtTelefone.Text = string.Empty;
            txtConvenio.Text = string.Empty;
            txtSenha.Text = string.Empty;
        }

        private async void OnFinalizarCadastroClicked(object sender, EventArgs e)
        {
            // 1. Validação dos campos obrigatórios comuns para o Login
            if (string.IsNullOrWhiteSpace(txtNome.Text) ||
                string.IsNullOrWhiteSpace(txtCPF.Text) ||
                string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                await DisplayAlert("Aviso", "Por favor, preencha Nome, CPF e Senha!", "OK");
                return;
            }

            // Validação básica do tamanho do CPF
            string cpfLimpo = txtCPF.Text.Trim();
            if (cpfLimpo.Length != 11 || !cpfLimpo.All(char.IsDigit))
            {
                await DisplayAlert("CPF Inválido", "O CPF deve conter exatamente 11 dígitos numéricos.", "OK");
                return;
            }

            try
            {
                // Se o usuário digitar no campo de Convênio a palavra "ADM123", ele vira Administrador!
                // Caso contrário, ele é cadastrado como um Paciente normal.
                string tipoUsuario = "Paciente";

                if (txtConvenio.Text != null && txtConvenio.Text.Trim().ToUpper() == "ADM123")
                {
                    tipoUsuario = "Administrador";
                }

                //  Criamos o objeto USUÁRIO com o Tipo definido pela regra acima
                var novoUsuario = new MauiTCC.Models.Usuario
                {
                    Nome = txtNome.Text.Trim(),
                    CPF = cpfLimpo,
                    Senha = txtSenha.Text,
                    Tipo = tipoUsuario // Será "Paciente" ou "Administrador"
                };

                // Grava na tabela geral de Usuários (indispensável para o Login)
                await _dbService.SalvarUsuarioAsync(novoUsuario);

                // Se for Paciente, gravamos também os dados clínicos na tabela de Paciente
                if (tipoUsuario == "Paciente")
                {
                    var novoPaciente = new MauiTCC.Models.Paciente
                    {
                        Nome = txtNome.Text.Trim(),
                        CPF = cpfLimpo,
                        Senha = txtSenha.Text,
                        Telefone = txtTelefone.Text?.Trim() ?? string.Empty,
                        Convenio = txtConvenio.Text?.Trim() ?? string.Empty
                    };

                    // Grava na tabela de dados específicos do Paciente
                    await _dbService.SalvarPacienteAsync(novoPaciente);

                    await DisplayAlert("Sucesso!", "Cadastro de Paciente realizado com sucesso! Você já pode fazer o seu login.", "OK");
                }
                else
                {
                    // Se digitou a palavra-chave, avisa com um alerta especial de Administrador
                    await DisplayAlert("Sucesso Master!", "Novo Administrador do sistema cadastrado!", "OK");
                }

                // Limpa os campos da tela
                LimparCampos();

                // Volta de forma segura para a tela de login
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao realizar o cadastro: " + ex.Message, "OK");
            }
        }
    }
}