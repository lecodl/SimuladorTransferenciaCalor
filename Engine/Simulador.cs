using SimuladorTransferenciaCalor.Models;

namespace SimuladorTransferenciaCalor.Engine;

// Classe responsável pelos cálculos da transferência de calor
internal class Simulador
{
    // Matriz de corpos utilizada na simulação
    private MatrizCorpos matriz;

    // Tempo utilizado em cada passo da simulação, em segundos
    private double intervaloTempo;

    // Construtor que recebe a matriz da simulação
    internal Simulador(MatrizCorpos matrizInicial)
    {
        // Guarda a matriz recebida
        matriz = matrizInicial;

        // Define o intervalo de tempo de cada cálculo
        intervaloTempo = 0.1;
    }

    // Executa uma etapa da transferência de calor
    internal void ExecutarPasso()
    {
        int tamanho = matriz.GetTamanho();

        double[,] temperaturasAtuais =
            new double[tamanho, tamanho];

        for (int linha = 0; linha < tamanho; linha++)
        {
            for (int coluna = 0; coluna < tamanho; coluna++)
            {
                Corpo? corpoAtual =
                    matriz.ObterCorpo(linha, coluna);

                if (corpoAtual == null)
                {
                    continue;
                }

                temperaturasAtuais[linha, coluna] =
                    corpoAtual.GetTemperatura();
            }
        }

        for (int linha = 0; linha < tamanho; linha++)
        {
            for (int coluna = 0; coluna < tamanho; coluna++)
            {
                if (coluna + 1 < tamanho)
                {
                    TrocarCalorEntreCorpos(
                        linha,
                        coluna,
                        linha,
                        coluna + 1,
                        temperaturasAtuais);
                }

                if (linha + 1 < tamanho)
                {
                    TrocarCalorEntreCorpos(
                        linha,
                        coluna,
                        linha + 1,
                        coluna,
                        temperaturasAtuais);
                }
            }
        }

        for (int linha = 0; linha < tamanho; linha++)
        {
            for (int coluna = 0; coluna < tamanho; coluna++)
            {
                Corpo? corpoAtual =
                    matriz.ObterCorpo(linha, coluna);

                if (corpoAtual == null)
                {
                    continue;
                }

                corpoAtual.SetTemperatura(
                    temperaturasAtuais[linha, coluna]);
            }
        }
    }

    // Calcula a troca de calor entre dois corpos em contato
    private void TrocarCalorEntreCorpos(
        int linhaAtual,
        int colunaAtual,
        int linhaVizinho,
        int colunaVizinho,
        double[,] temperaturas)
    {
        Corpo? corpoAtual =
            matriz.ObterCorpo(linhaAtual, colunaAtual);

        Corpo? corpoVizinho =
            matriz.ObterCorpo(linhaVizinho, colunaVizinho);

        if (corpoAtual == null || corpoVizinho == null)
        {
            return;
        }

        double temperaturaAtual =
            temperaturas[linhaAtual, colunaAtual];

        double temperaturaVizinho =
            temperaturas[linhaVizinho, colunaVizinho];

        if (Math.Abs(temperaturaAtual - temperaturaVizinho) < 0.0001)
        {
            return;
        }

        double ladoAtual = corpoAtual.GetLado();
        double ladoVizinho = corpoVizinho.GetLado();
        double area = Math.Max(ladoAtual, ladoVizinho);
        area = area * area;

        double condutividadeAtual =
            corpoAtual.GetMaterial().GetCondutividade();

        double condutividadeVizinho =
            corpoVizinho.GetMaterial().GetCondutividade();

        double condutividadeMinima =
            Math.Min(condutividadeAtual, condutividadeVizinho);

        double diferencaTemperatura =
            temperaturaAtual - temperaturaVizinho;

        // Como os corpos estão em contato físico direto, consideramos a
        // distância de contato como o próprio lado do corpo, em metros.
        double deltaX =
            Math.Max(0.001, Math.Min(ladoAtual, ladoVizinho));

        double q =
            condutividadeMinima
            * area
            * (diferencaTemperatura / deltaX);

        double massaAtual = corpoAtual.CalcularMassa();
        double massaVizinho = corpoVizinho.CalcularMassa();

        double calorEspecificoAtual =
            corpoAtual.GetMaterial().GetCalorEspecifico();

        double calorEspecificoVizinho =
            corpoVizinho.GetMaterial().GetCalorEspecifico();

        double deltaQ = q * intervaloTempo;

        double deltaTAtual =
            deltaQ / (massaAtual * calorEspecificoAtual);

        double deltaTVizinho =
            deltaQ / (massaVizinho * calorEspecificoVizinho);

        if (diferencaTemperatura > 0)
        {
            temperaturas[linhaAtual, colunaAtual] =
                temperaturaAtual - deltaTAtual;

            temperaturas[linhaVizinho, colunaVizinho] =
                temperaturaVizinho + deltaTVizinho;
        }
        else
        {
            temperaturas[linhaAtual, colunaAtual] =
                temperaturaAtual + Math.Abs(deltaTAtual);

            temperaturas[linhaVizinho, colunaVizinho] =
                temperaturaVizinho - Math.Abs(deltaTVizinho);
        }
    }
}