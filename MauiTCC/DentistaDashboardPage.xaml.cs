using System;
using Microsoft.Maui.Controls;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class DentistaDashboardPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly Usuario _dentistaLogado;

        // Construtor padrão caso precise, mas o ideal é o com parâmetros
        public DentistaDashboardPage() : this(new Usuario { Id = 1, Nome = "Dentista Plantonista" }) { }

        public DentistaDashboardPage(Usuario dentista)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _dentistaLogado = dentista;

            lblBoasVindasDentista.Text = $"Olá, Dr(a). {dentista.Nome}";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarConsultasDoMedigoAsync();
        }

        private async Task CarregarConsultasDoMedigoAsync()
        {
            try
            {
                // Busca na Database os agendamentos específicos desse médico
                var consultas = await _dbService.GetAgendamentosPorDentistaAsync(_dentistaLogado.Id);
                collConsultasDentista.ItemsSource = consultas;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao carregar agenda: " + ex.Message, "OK");
            }
        }

        private async void OnConsultaSelecionada(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Agendamento consultaSelecionada)
            {
                // Remove a seleção visual para poder clicar de novo depois
                ((CollectionView)sender).SelectedItem = null;

                // Redireciona para o Prontuário Clínico levando os dados da consulta
                await Navigation.PushAsync(new ProntuarioClinicoPage(consultaSelecionada));
            }
        }
    }
}