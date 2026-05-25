using System;
using Microsoft.Maui.Controls;

namespace MauiTCC
{
    public partial class PainelPacientePage : ContentPage
    {
        public PainelPacientePage()
        {
            InitializeComponent();
        }

        private async void OnSairClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}