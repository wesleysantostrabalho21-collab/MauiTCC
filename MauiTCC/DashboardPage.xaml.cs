using System;
using Microsoft.Maui.Controls;

namespace MauiTCC
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private async void OnCadastrarDentistaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroDentistaPage());
        }

        private async void OnCadastrarUsuarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroUsuarioPage());
        }

        // Esse método precisa ter exatamente (object sender, EventArgs e)
        private async void OnSairClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}