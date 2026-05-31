using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly Usuario _usuarioAtual;

       
        public DashboardPage(Usuario usuarioLogado = null)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _usuarioAtual = usuarioLogado;

            
            if (_usuarioAtual != null && _usuarioAtual.Tipo == "Administrador")
            {
                menuAdmin.IsVisible = true;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarAgendaCompleta();
        }

        private async Task CarregarAgendaCompleta()
        {
            try
            {
                var agendamentos = await _dbService.GetAgendamentosAsync();
                collAgendamentos.ItemsSource = agendamentos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao carregar agendamentos: " + ex.Message, "OK");
            }
        }

        private async void OnDataSelecionadaChanged(object sender, DateChangedEventArgs e)
        {
            try
            {
                // Tratando para garantir o tipo DateTime comum esperado pelo banco
                DateTime dataFiltrada = (DateTime)e.NewDate;
                var filtrados = await _dbService.GetAgendamentosPorDataAsync(dataFiltrada);
                collAgendamentos.ItemsSource = filtrados;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao filtrar data: " + ex.Message, "OK");
            }
        }

        private async void OnLimparFiltrosClicked(object sender, EventArgs e)
        {
            await CarregarAgendaCompleta();
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var agendamentoSelecionado = (Agendamento)btn.CommandParameter;

            bool confirmar = await DisplayAlert("Confirmar", $"Deseja realmente cancelar a consulta de {agendamentoSelecionado.NomePaciente}?", "Sim", "Não");

            if (confirmar)
            {
                try
                {
                    await _dbService.CancelarAgendamentoAsync(agendamentoSelecionado);
                    await DisplayAlert("Sucesso", "Consulta cancelada com sucesso!", "OK");
                    await CarregarAgendaCompleta();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", "Falha ao cancelar: " + ex.Message, "OK");
                }
            }
        }

        private async void OnRemarcarClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var agendamentoSelecionado = (Agendamento)btn.CommandParameter;

            await Navigation.PushAsync(new AgendarConsultaPage(agendamentoSelecionado));
        }

        

        private async void OnMenuNovoPacienteClicked(object sender, EventArgs e)
        {
            // Redireciona para a sua página de cadastro geral existente
            await Navigation.PushAsync(new CadastroPage());
        }

        private async void OnMenuNovoProfissionalClicked(object sender, EventArgs e)
        {
            // Redireciona para a sua página de cadastro geral existente
            await Navigation.PushAsync(new CadastroDentistaPage());
        }

        private async void OnMenuFinanceiroClicked(object sender, EventArgs e)
        {
            // Feedback visual provisório para o módulo de fluxo de caixa
            await DisplayAlert("Módulo Financeiro", "Funcionalidade de fluxo de caixa em desenvolvimento para o TCC.", "OK");
        }

        private async void OnMenuAgendarConsultaClicked(object sender, EventArgs e)
        {
           
            await Navigation.PushAsync(new AgendarConsultaPage());
        }
    }
}