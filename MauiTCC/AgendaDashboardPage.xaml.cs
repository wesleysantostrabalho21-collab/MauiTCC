using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class AgendaDashboardPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public AgendaDashboardPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
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
              
                DateTime dataFiltrada = e.NewDate.HasValue ? e.NewDate.Value.Date : DateTime.Today;

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
    }
}