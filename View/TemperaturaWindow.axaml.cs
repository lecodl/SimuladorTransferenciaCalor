using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SimuladorTransferenciaCalor.View
{
    // Janela usada para alterar a temperatura de um corpo
    internal partial class TemperaturaWindow : Window
    {
        // Campo de entrada da temperatura
        private TextBox temperaturaInput = null!;

        // Guarda a temperatura informada pelo usuário
        private double temperaturaInformada;

        // Construtor da janela
        internal TemperaturaWindow(double temperaturaAtual)
        {
            // Carrega os controles definidos no arquivo AXAML
            InitializeComponent();

            // Localiza o campo de temperatura
            temperaturaInput =
                this.FindControl<TextBox>("TemperaturaInput")!;

            // Mostra a temperatura atual em Celsius
            temperaturaInput.Text =
                temperaturaAtual.ToString(
                    "F2",
                    CultureInfo.InvariantCulture);
        }

        // Inicializa os componentes visuais
        private void InitializeComponent()
        {
            // Carrega a interface definida no arquivo AXAML
            AvaloniaXamlLoader.Load(this);
        }

        // Executado quando o botão Confirmar é pressionado
        private void ConfirmarButton_Click(
            object? sender,
            RoutedEventArgs e)
        {
            // Tenta converter o texto para número usando ponto decimal
            bool conversaoValida =
                double.TryParse(
                    temperaturaInput.Text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double temperatura);

            // Verifica se a conversão foi válida
            if (conversaoValida)
            {
                // Guarda a temperatura informada em Celsius
                temperaturaInformada = temperatura;

                // Fecha a janela
                Close();
            }
        }

        // Retorna a temperatura informada
        internal double GetTemperatura()
        {
            return temperaturaInformada;
        }
    }
}