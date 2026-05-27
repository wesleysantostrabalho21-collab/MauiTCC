using System;
using Microsoft.Maui.Controls;
using MauiTCC.Models;
using MauiTCC.Services;

namespace MauiTCC
{
    public partial class ProntuarioClinicoPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly Agendamento _consultaAtual;
        private Prontuario _prontuarioPaciente;
        private string _caminhoArquivoSelecionado = string.Empty;

        public ProntuarioClinicoPage(Agendamento consulta)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _consultaAtual = consulta;

            // Alimenta a interface visual com as informações da consulta
            lblNomePaciente.Text = $"Paciente: {consulta.NomePaciente}";
            lblInfoConsulta.Text = $"Consulta em: {consulta.Data:dd/MM/yyyy} às {consulta.Horario} - {consulta.SalaCadeira}";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarProntuarioExistenteAsync();
        }

        private async Task CarregarProntuarioExistenteAsync()
        {
            try
            {
                // Busca se o paciente já possui algum prontuário salvo no banco
                _prontuarioPaciente = await _dbService.GetProntuarioAsync(_consultaAtual.IdPaciente);

                if (_prontuarioPaciente != null)
                {
                    // Se já existir, preenche os campos com o histórico antigo
                    txtHistorico.Text = _prontuarioPaciente.HistoricoTratamentos;
                    txtProcedimentos.Text = _prontuarioPaciente.ProcedimentosRealizados;
                    _caminhoArquivoSelecionado = _prontuarioPaciente.CaminhoAnexoDocumento;

                    if (!string.IsNullOrEmpty(_caminhoArquivoSelecionado))
                    {
                        lblCaminhoArquivo.Text = $"Arquivo: {Path.GetFileName(_caminhoArquivoSelecionado)}";

                        // Se o anexo for uma imagem válida, exibe o preview na tela
                        imgPreviewAnexo.Source = ImageSource.FromFile(_caminhoArquivoSelecionado);
                        imgPreviewAnexo.IsVisible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar dados do prontuário: " + ex.Message, "OK");
            }
        }

        // 🌟 LÓGICA DO FILE PICKER (ARMAZENAMENTO DE EXAMES/RADIOGRAFIAS)
        private async void OnAnexarDocumentoClicked(object sender, EventArgs e)
        {
            try
            {
                // Opções para aceitar apenas imagens (raio-x, fotos da boca, etc)
                var opcoesFiltro = new PickOptions
                {
                    PickerTitle = "Selecione a Radiografia/Exame do Paciente",
                    FileTypes = FilePickerFileType.Images
                };

                var resultadoArquivo = await FilePicker.Default.PickAsync(opcoesFiltro);

                if (resultadoArquivo != null)
                {
                    // Captura o caminho absoluto onde o arquivo está guardado no aparelho
                    _caminhoArquivoSelecionado = resultadoArquivo.FullPath;
                    lblCaminhoArquivo.Text = $"Anexo: {resultadoArquivo.FileName}";

                    // Exibe a imagem selecionada no componente de preview
                    imgPreviewAnexo.Source = ImageSource.FromFile(_caminhoArquivoSelecionado);
                    imgPreviewAnexo.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro ao carregar arquivo", ex.Message, "OK");
            }
        }

        private async void OnSalvarProntuarioClicked(object sender, EventArgs e)
        {
            try
            {
                // Se for o primeiro atendimento do paciente, criamos a instância do objeto
                if (_prontuarioPaciente == null)
                {
                    _prontuarioPaciente = new Prontuario
                    {
                        IdPaciente = _consultaAtual.IdPaciente,
                        NomePaciente = _consultaAtual.NomePaciente,

                        // 🌟 SOLUÇÃO DO ERRO: Preenchendo os campos obrigatórios do banco
                        DataAbertura = DateTime.Now,
                        StatusGeral = "Ativo" // Define um status padrão para não ir nulo
                    };
                }

                // Se por algum motivo o prontuário já existia mas veio com status nulo, garante o valor
                if (string.IsNullOrEmpty(_prontuarioPaciente.StatusGeral))
                {
                    _prontuarioPaciente.StatusGeral = "Ativo";
                }

                // Atualiza os dados com o que o dentista digitou e anexou
                _prontuarioPaciente.HistoricoTratamentos = txtHistorico.Text;
                _prontuarioPaciente.ProcedimentosRealizados = txtProcedimentos.Text;
                _prontuarioPaciente.CaminhoAnexoDocumento = _caminhoArquivoSelecionado;
                _prontuarioPaciente.UltimaAtualizacao = DateTime.Now;

                // Salva na tabela através do SQLite de forma unificada
                await _dbService.SalvarProntuarioAsync(_prontuarioPaciente);

                await DisplayAlert("Sucesso", "Prontuário e anexos atualizados com sucesso!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Falha ao gravar prontuário: " + ex.Message, "OK");
            }
        }
    }
        
    
}