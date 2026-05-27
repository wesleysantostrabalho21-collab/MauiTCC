using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class AgendarConsultaPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private Agendamento _agendamentoEdicao;

        public AgendarConsultaPage(Agendamento agendamentoParaRemarcar = null)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _agendamentoEdicao = agendamentoParaRemarcar;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarDadosComponentesAsync();
        }

        private async Task CarregarDadosComponentesAsync()
        {
            try
            {
                var listaPacientes = await _dbService.GetPacientesAsync();
                pckPaciente.ItemsSource = listaPacientes;

                var listaDentistas = await _dbService.GetTodosDentistasAsync();
                pckDentista.ItemsSource = listaDentistas;

                if (_agendamentoEdicao != null)
                {
                    Title = "Remarcar Consulta";
                    btnFinalizar.Text = "SALVAR ALTERAÇÕES";

                    dtpData.Date = _agendamentoEdicao.Data;

                    pckHorario.SelectedItem = _agendamentoEdicao.Horario;
                    pckSala.SelectedItem = _agendamentoEdicao.SalaCadeira;

                    pckPaciente.IsEnabled = false;
                    pckDentista.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao carregar dados iniciais: " + ex.Message, "OK");
            }
        }

        private async void OnConfirmarAgendamentoClicked(object sender, EventArgs e)
        {
            if (pckPaciente.SelectedIndex == -1 || pckDentista.SelectedIndex == -1 ||
                pckSala.SelectedIndex == -1 || pckHorario.SelectedIndex == -1)
            {
                await DisplayAlert("Aviso", "Por favor, preencha todas as opções da consulta.", "OK");
                return;
            }

            var pacienteSelecionado = (Paciente)pckPaciente.SelectedItem;
            var dentistaSelecionado = (Usuario)pckDentista.SelectedItem;

            DateTime dataSelecionada = (DateTime)dtpData.Date;
            string horarioSelecionado = pckHorario.SelectedItem.ToString();
            string salaSelecionada = pckSala.SelectedItem.ToString();

            int idAtual = _agendamentoEdicao?.Id ?? 0;

            bool agendaDisponivel = await _dbService.VerificarDisponibilidadeAsync(
                dataSelecionada, horarioSelecionado, salaSelecionada, dentistaSelecionado.Id, idAtual);

            if (!agendaDisponivel)
            {
                await DisplayAlert("Agenda Ocupada", "O Dentista ou a Sala selecionada já possuem compromisso neste mesmo dia e horário!", "OK");
                return;
            }

            try
            {
                if (_agendamentoEdicao == null)
                {
                    var novaConsulta = new Agendamento
                    {
                        // 🌟 CORREÇÃO: Garante o ID correto do objeto Paciente
                        IdPaciente = pacienteSelecionado.Id,
                        NomePaciente = pacienteSelecionado.Nome,
                        IdDentista = dentistaSelecionado.Id,
                        NomeDentista = dentistaSelecionado.Nome,
                        Data = dataSelecionada,
                        Horario = horarioSelecionado,
                        SalaCadeira = salaSelecionada,
                        Status = "Agendado"
                    };
                    await _dbService.SalvarAgendamentoAsync(novaConsulta);
                    await DisplayAlert("Sucesso", "Consulta agendada com êxito!", "OK");
                }
                else
                {
                    _agendamentoEdicao.Data = dataSelecionada;
                    _agendamentoEdicao.Horario = horarioSelecionado;
                    _agendamentoEdicao.SalaCadeira = salaSelecionada;
                    _agendamentoEdicao.Status = "Remarcado";

                    await _dbService.SalvarAgendamentoAsync(_agendamentoEdicao);
                    await DisplayAlert("Sucesso", "Consulta remarcada com êxito!", "OK");
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao processar agendamento: " + ex.Message, "OK");
            }
        }
    }
}