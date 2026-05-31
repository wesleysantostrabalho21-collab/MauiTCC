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
               
                _prontuarioPaciente = await _dbService.GetProntuarioAsync(_consultaAtual.IdPaciente);

                if (_prontuarioPaciente != null)
                {
                    
                    txtHistorico.Text = _prontuarioPaciente.HistoricoTratamentos;
                    txtProcedimentos.Text = _prontuarioPaciente.ProcedimentosRealizados;
                    _caminhoArquivoSelecionado = _prontuarioPaciente.CaminhoAnexoDocumento;

                    if (!string.IsNullOrEmpty(_caminhoArquivoSelecionado))
                    {
                        lblCaminhoArquivo.Text = $"Arquivo: {Path.GetFileName(_caminhoArquivoSelecionado)}";


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

    
        private async void OnAnexarDocumentoClicked(object sender, EventArgs e)
        {
            try
            {
               
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
                
                if (_prontuarioPaciente == null)
                {
                    _prontuarioPaciente = new Prontuario
                    {
                        IdPaciente = _consultaAtual.IdPaciente,
                        NomePaciente = _consultaAtual.NomePaciente,

                        
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