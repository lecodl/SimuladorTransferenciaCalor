using SimuladorTransferenciaCalor.Models;

namespace SimuladorTransferenciaCalor.Engine;


// Classe responsável por armazenar os corpos em uma matriz
internal class MatrizCorpos
{

    // Matriz privada que armazena os corpos
    private Corpo[,] corpos;

    // Tamanho da matriz
    private int tamanho;


    // Construtor que cria uma matriz quadrada
    internal MatrizCorpos(int tamanhoInicial)
    {
        // Guarda o tamanho informado
        tamanho = tamanhoInicial;

        // Cria a matriz de corpos
        corpos = new Corpo[tamanho, tamanho];
    }


    // Adiciona um corpo em uma posição da matriz
    internal void AdicionarCorpo(
        int linha,
        int coluna,
        Corpo corpo)
    {
        // Verifica se a posição está dentro dos limites
        if (linha >= 0 &&
            linha < tamanho &&
            coluna >= 0 &&
            coluna < tamanho)
        {
            // Armazena o corpo na posição indicada
            corpos[linha, coluna] = corpo;
        }
    }


    // Retorna o corpo de uma posição da matriz
    internal Corpo? ObterCorpo(
        int linha,
        int coluna)
    {
        // Verifica se a posição está dentro dos limites
        if (linha >= 0 &&
            linha < tamanho &&
            coluna >= 0 &&
            coluna < tamanho)
        {
            // Retorna o corpo armazenado na posição
            return corpos[linha, coluna];
        }

        // Retorna nulo quando a posição é inválida
        return null;
    }


    // Retorna o tamanho da matriz
    internal int GetTamanho()
    {
        // Devolve o tamanho da matriz
        return tamanho;
    }
}