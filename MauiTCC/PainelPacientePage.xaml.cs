using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class PainelPacientePage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly int _idPacienteLogado;
        private readonly string _nomePacienteLogado;

        public PainelPacientePage(int idPaciente, string nomePaciente)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _idPacienteLogado = idPaciente;
            _nomePacienteLogado = nomePaciente;

            lblBoasVindas.Text = $"Olá, {nomePaciente}! Veja suas consultas abaixo:";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarMinhasConsultasAsync();
        }

        private async Task CarregarMinhasConsultasAsync()
        {
            try
            {
                
                var meusAgendamentos = await _dbService.GetAgendamentosPorPacienteAsync(_idPacienteLogado, _nomePacienteLogado);
                collMinhasConsultas.ItemsSource = meusAgendamentos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Não foi possível carregar suas consultas: " + ex.Message, "OK");
            }
        }
    }
}