using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SimuladorTransferenciaCalor.Models;
using SimuladorTransferenciaCalor.Dados;
using SimuladorTransferenciaCalor.Engine;

namespace SimuladorTransferenciaCalor.View
{
    // Classe responsável pela janela principal da aplicação
    internal partial class MainWindow : Window
    {
        // Campo que representa o controle de linhas
        private TextBox linhasInput = null!;

        // Campo que representa o controle de colunas
        private TextBox colunasInput = null!;

        // Campo que representa a lista de materiais
        private ComboBox materialInput = null!;

        // Campo que representa o Grid da matriz
        private Grid matrizVisual = null!;

        // Campo que controla a execução automática
        private DispatcherTimer? timerExecucao;

        // Cronometra o tempo de execução da simulação
        private readonly Stopwatch cronometro = new Stopwatch();

        // Campo que representa o tempo exibido na tela
        private TextBlock tempoText = null!;

        // Imagens dos materiais carregadas durante a execução
        private readonly Dictionary<string, Bitmap?> imagensMateriais = new();

        // Campo que guarda a matriz lógica atual
        private MatrizCorpos? matrizCorposAtual;

        // Campo que guarda o simulador térmico atual
        private Simulador? simuladorAtual;

        // Guarda a linha selecionada
        private int linhaSelecionada = -1;

        // Guarda a coluna selecionada
        private int colunaSelecionada = -1;

        // Construtor da janela principal
        public MainWindow()
        {
            // Carrega os componentes definidos no arquivo AXAML
            InitializeComponent();

            // Cria o temporizador da simulação automática
            timerExecucao = new DispatcherTimer();
            timerExecucao.Interval = TimeSpan.FromMilliseconds(100);
            timerExecucao.Tick += TimerExecucao_Tick;

            // Localiza o campo de linhas
            linhasInput =
                this.FindControl<TextBox>("LinhasInput")!;

            // Localiza o campo de colunas
            colunasInput =
                this.FindControl<TextBox>("ColunasInput")!;

            // Localiza a lista de materiais
            materialInput =
                this.FindControl<ComboBox>("MaterialInput")!;

            // Localiza o Grid da matriz
            matrizVisual =
                this.FindControl<Grid>("MatrizVisual")!;

            // Localiza o temporizador visual
            tempoText =
                this.FindControl<TextBlock>("TempoText")!;
        }

        // Carrega os componentes visuais
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // Executado quando o botão Gerar matriz é clicado
        private void GerarMatrizButton_Click(
            object? sender,
            RoutedEventArgs e)
        {
            // Obtém a quantidade de linhas
            int linhas = LerInteiro(linhasInput.Text, 4);

            // Obtém a quantidade de colunas
            int colunas = LerInteiro(colunasInput.Text, 4);

            linhas = Math.Clamp(linhas, 1, 20);
            colunas = Math.Clamp(colunas, 1, 20);

            linhasInput.Text = linhas.ToString();
            colunasInput.Text = colunas.ToString();

            // Obtém o material selecionado
            ComboBoxItem? itemSelecionado =
                materialInput.SelectedItem as ComboBoxItem;

            // Define o nome do material
            string nomeMaterial =
                itemSelecionado?.Content?.ToString()
                ?? "Cobre";

            // Gera a matriz
            GerarMatriz(linhas, colunas, nomeMaterial);
        }

        // Executado quando o botão Executar passo é clicado
        private void ExecutarPassoButton_Click(
            object? sender,
            RoutedEventArgs e)
        {
            // Verifica se existe um simulador
            if (simuladorAtual == null)
            {
                return;
            }

            // Executa uma etapa da transferência de calor
            simuladorAtual.ExecutarPasso();

            // Atualiza os valores na tela
            AtualizarMatrizVisual();
        }

        // Executado quando o botão Iniciar é clicado
        private void IniciarButton_Click(
            object? sender,
            RoutedEventArgs e)
        {
            if (simuladorAtual == null)
            {
                return;
            }

            if (timerExecucao != null)
            {
                cronometro.Start();
                timerExecucao.Start();
            }
        }

        // Executado quando o botão Parar é clicado
        private void PararButton_Click(
            object? sender,
            RoutedEventArgs e)
        {
            if (timerExecucao != null)
            {
                cronometro.Stop();
                timerExecucao.Stop();
                AtualizarTempoVisual();
            }
        }

        // Executado a cada intervalo do temporizador
        private void TimerExecucao_Tick(
            object? sender,
            EventArgs e)
        {
            if (simuladorAtual == null)
            {
                return;
            }

            simuladorAtual.ExecutarPasso();
            AtualizarMatrizVisual();
            AtualizarTempoVisual();
        }

        // Cria a matriz visual e lógica
        private void GerarMatriz(
            int linhas,
            int colunas,
            string nomeMaterial)
        {
            const double ladoPadrao = 0.05;

            if (timerExecucao != null)
            {
                timerExecucao.Stop();
            }

            cronometro.Reset();
            AtualizarTempoVisual();

            // Remove os elementos antigos
            matrizVisual.Children.Clear();

            // Remove as linhas antigas
            matrizVisual.RowDefinitions.Clear();

            // Remove as colunas antigas
            matrizVisual.ColumnDefinitions.Clear();

            // Obtém os materiais cadastrados
            List<Material> materiais =
                Materiais.GetMateriais();

            // Procura o material escolhido
            Material? materialEscolhido = null;

            // Percorre os materiais
            foreach (Material material in materiais)
            {
                // Verifica se encontrou o material
                if (material.GetNome() == nomeMaterial)
                {
                    // Guarda o material encontrado
                    materialEscolhido = material;

                    // Encerra a procura
                    break;
                }
            }

            // Usa o primeiro material caso não encontre
            materialEscolhido ??= materiais[0];

            // Define a dimensão da malha lógica
            int tamanhoMatriz =
                Math.Max(linhas, colunas);

            // Cria a matriz lógica
            matrizCorposAtual =
                new MatrizCorpos(tamanhoMatriz);

            // Cria o simulador
            simuladorAtual =
                new Simulador(matrizCorposAtual);

            // Limpa a seleção anterior
            linhaSelecionada = -1;
            colunaSelecionada = -1;

            // Cria as linhas visuais
            for (int linha = 0; linha < linhas; linha++)
            {
                // Adiciona uma linha de 80 pixels
                matrizVisual.RowDefinitions.Add(
                    new RowDefinition(
                        new GridLength(80)));
            }

            // Cria as colunas visuais
            for (int coluna = 0; coluna < colunas; coluna++)
            {
                // Adiciona uma coluna de 80 pixels
                matrizVisual.ColumnDefinitions.Add(
                    new ColumnDefinition(
                        new GridLength(80)));
            }

            // Percorre as linhas da matriz
            for (int linha = 0; linha < linhas; linha++)
            {
                // Percorre as colunas da matriz
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    // Cria um corpo com temperatura inicial de 25 °C
                    Corpo corpo =
                        new Corpo(
                            materialEscolhido,
                            ladoPadrao,
                            25.0);

                    // Adiciona o corpo à matriz lógica
                    matrizCorposAtual.AdicionarCorpo(
                        linha,
                        coluna,
                        corpo);

                    // Cria o texto inicial da célula
                    TextBlock texto =
                        new TextBlock
                        {
                            // Mostra posição e temperatura em Celsius
                            Text =
                                $"{linha + 1},{coluna + 1}\n25,00 °C",

                            // Centraliza horizontalmente
                            HorizontalAlignment =
                                HorizontalAlignment.Center,

                            // Centraliza verticalmente
                            VerticalAlignment =
                                VerticalAlignment.Center,

                            // Define o tamanho do texto
                            FontSize = 14,

                            // Destaca a posição e a temperatura sobre a imagem
                            FontWeight = FontWeight.Bold,
                            Foreground = Brushes.Black,
                            Padding = new Avalonia.Thickness(4)
                        };

                    // Cria a imagem do material
                    Image imagemMaterial =
                        new Image
                        {
                            Source = ObterImagemMaterial(nomeMaterial),
                            Stretch = Avalonia.Media.Stretch.UniformToFill
                        };

                    // Cria o filtro vermelho controlado pela temperatura
                    Border filtroCalor =
                        new Border
                        {
                            Background = ObterFiltroCalor(25.0)
                        };

                    Grid conteudoCelula =
                        new Grid();
                    conteudoCelula.Children.Add(imagemMaterial);
                    conteudoCelula.Children.Add(filtroCalor);
                    conteudoCelula.Children.Add(texto);

                    // Cria o quadrado visual
                    Border corpoVisual =
                        new Border
                        {
                            // Define a espessura da borda
                            BorderThickness =
                                new Avalonia.Thickness(1),

                            // Define a cor da borda
                            BorderBrush =
                                Brushes.Black,

                            // Define a margem
                            Margin =
                                new Avalonia.Thickness(2),

                            // Coloca imagem, filtro e texto no quadrado
                            Child = conteudoCelula
                        };

                    // Permite clicar no quadrado
                    corpoVisual.PointerPressed +=
                        CorpoVisual_PointerPressed;

                    // Define a linha visual
                    Grid.SetRow(corpoVisual, linha);

                    // Define a coluna visual
                    Grid.SetColumn(corpoVisual, coluna);

                    // Adiciona o quadrado à matriz
                    matrizVisual.Children.Add(corpoVisual);
                }
            }
        }

        // Executado quando um quadrado é pressionado
        private async void CorpoVisual_PointerPressed(
            object? sender,
            Avalonia.Input.PointerPressedEventArgs e)
        {
            // Verifica se o controle é uma borda
            if (sender is not Border corpoVisual)
            {
                return;
            }

            // Obtém a linha selecionada
            int linha =
                Grid.GetRow(corpoVisual);

            // Obtém a coluna selecionada
            int coluna =
                Grid.GetColumn(corpoVisual);

            // Verifica se a matriz existe
            if (matrizCorposAtual == null)
            {
                return;
            }

            // Obtém o corpo selecionado
            Corpo? corpo =
                matrizCorposAtual.ObterCorpo(
                    linha,
                    coluna);

            // Verifica se o corpo existe
            if (corpo == null)
            {
                return;
            }

            // Guarda a posição selecionada
            linhaSelecionada = linha;
            colunaSelecionada = coluna;

            // Abre a janela para alterar a temperatura
            TemperaturaWindow janelaTemperatura =
                new TemperaturaWindow(
                    corpo.GetTemperatura());

            // Exibe a janela como diálogo
            await janelaTemperatura.ShowDialog(this);

            // Obtém a nova temperatura em Kelvin
            double novaTemperatura =
                janelaTemperatura.GetTemperatura();

            // Verifica se a temperatura é válida em Celsius
            if (double.IsFinite(novaTemperatura))
            {
                // Altera a temperatura do corpo
                corpo.SetTemperatura(novaTemperatura);
            }

            // Permite trocar o material do corpo selecionado
            ComboBoxItem? itemSelecionado =
                materialInput.SelectedItem as ComboBoxItem;

            string nomeMaterialSelecionado =
                itemSelecionado?.Content?.ToString()
                ?? "Cobre";

            Material? materialSelecionado =
                ObterMaterialPorNome(nomeMaterialSelecionado);

            if (materialSelecionado != null)
            {
                corpo.SetMaterial(materialSelecionado);
            }

            // Atualiza a matriz visual
            AtualizarMatrizVisual();

            // Mostra a seleção no terminal
            Console.WriteLine(
                $"Temperatura alterada na linha {linha + 1}, coluna {coluna + 1}: {novaTemperatura:F2} °C");
        }

        // Atualiza as temperaturas mostradas na matriz
        private void AtualizarMatrizVisual()
        {
            // Verifica se a matriz existe
            if (matrizCorposAtual == null)
            {
                return;
            }

            // Percorre os controles visuais
            foreach (Control controle in matrizVisual.Children)
            {
                // Verifica se o controle é uma borda
                if (controle is not Border borda)
                {
                    continue;
                }

                // Obtém a linha
                int linha =
                    Grid.GetRow(borda);

                // Obtém a coluna
                int coluna =
                    Grid.GetColumn(borda);

                // Obtém o corpo correspondente
                Corpo? corpo =
                    matrizCorposAtual.ObterCorpo(
                        linha,
                        coluna);

                // Verifica se o corpo existe
                if (corpo == null)
                {
                    continue;
                }

                // Obtém a temperatura em Kelvin
                double temperatura =
                    corpo.GetTemperatura();

                if (borda.Child is Grid conteudoCelula)
                {
                    if (conteudoCelula.Children[0] is Image imagemMaterial)
                    {
                        imagemMaterial.Source =
                            ObterImagemMaterial(corpo.GetMaterial().GetNome());
                    }

                    if (conteudoCelula.Children[1] is Border filtroCalor)
                    {
                        filtroCalor.Background =
                            ObterFiltroCalor(temperatura);
                    }

                    if (conteudoCelula.Children[2] is TextBlock texto)
                    {
                        texto.Text =
                            $"{linha + 1},{coluna + 1}\n"
                            + $"{temperatura:F2} °C";
                    }
                }

                // Destaca o quadrado selecionado
                if (linha == linhaSelecionada &&
                    coluna == colunaSelecionada)
                {
                    // Define a borda vermelha
                    borda.BorderBrush =
                        Brushes.Red;

                    // Aumenta a espessura da borda
                    borda.BorderThickness =
                        new Avalonia.Thickness(3);
                }
                else
                {
                    // Define a borda normal
                    borda.BorderBrush =
                        Brushes.Black;

                    // Define a espessura normal
                    borda.BorderThickness =
                        new Avalonia.Thickness(1);
                }
            }
        }

        // Busca um material pelo nome
        private Material? ObterMaterialPorNome(string nomeMaterial)
        {
            List<Material> materiais = Materiais.GetMateriais();

            foreach (Material material in materiais)
            {
                if (material.GetNome() == nomeMaterial)
                {
                    return material;
                }
            }

            return null;
        }

        private Bitmap? ObterImagemMaterial(string nomeMaterial)
        {
            if (imagensMateriais.TryGetValue(nomeMaterial, out Bitmap? imagem))
            {
                return imagem;
            }

            string nomeArquivo = nomeMaterial switch
            {
                "Aço" => "aco.png",
                "Água" => "agua.png",
                "Alumínio" => "aluminio.png",
                "Cobre" => "cobre.png",
                "Concreto" => "concreto.png",
                "Ferro" => "ferro.png",
                "Madeira" => "madeira.png",
                "Vidro" => "vidro.png",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(nomeArquivo))
            {
                imagensMateriais[nomeMaterial] = null;
                return null;
            }

            string caminhoImagem =
                Path.Combine(AppContext.BaseDirectory, "img", nomeArquivo);

            imagem = File.Exists(caminhoImagem)
                ? new Bitmap(caminhoImagem)
                : null;

            imagensMateriais[nomeMaterial] = imagem;
            return imagem;
        }

        // Calcula um filtro vermelho suave conforme a temperatura aumenta
        private IBrush ObterFiltroCalor(double temperatura)
        {
            const double temperaturaFria = 25.0;
            const double temperaturaQuente = 100.0;

            double progresso =
                Math.Clamp(
                    (temperatura - temperaturaFria)
                    / (temperaturaQuente - temperaturaFria),
                    0.0,
                    1.0);

            double transicaoSuave =
                progresso * progresso * (3.0 - 2.0 * progresso);

            byte opacidade =
                (byte)Math.Round(220.0 * transicaoSuave);

            return new SolidColorBrush(
                Color.FromArgb(opacidade, 220, 20, 30));
        }

        private void AtualizarTempoVisual()
        {
            tempoText.Text = cronometro.Elapsed.ToString(@"hh\:mm\:ss");
        }

        private static int LerInteiro(string? valor, int padrao)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return padrao;
            }

            if (int.TryParse(valor.Trim(), out int numero))
            {
                return numero;
            }

            return padrao;
        }
    }
}