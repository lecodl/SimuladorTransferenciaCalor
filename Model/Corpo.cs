namespace SimuladorTransferenciaCalor.Models;

// Classe que representa um corpo da matriz térmica
internal class Corpo
{
    // Material utilizado pelo corpo
    private Material material;

    // Tamanho do lado do corpo em metros
    private double lado;

    // Temperatura atual do corpo em Kelvin
    private double temperatura;

    // Construtor que cria um corpo com material, tamanho e temperatura
    internal Corpo(
        Material materialInicial,
        double ladoInicial,
        double temperaturaInicial)
    {
        // Armazena o material recebido
        material = materialInicial;

        // Armazena o tamanho recebido
        lado = ladoInicial;

        // Armazena a temperatura inicial recebida
        temperatura = temperaturaInicial;
    }

    // Retorna o material do corpo
    internal Material GetMaterial()
    {
        // Devolve o material armazenado
        return material;
    }

    // Altera o material do corpo
    internal void SetMaterial(Material novoMaterial)
    {
        // Atualiza o material armazenado
        material = novoMaterial;
    }

    // Retorna o tamanho do lado do corpo
    internal double GetLado()
    {
        // Devolve o tamanho armazenado
        return lado;
    }

    // Altera o tamanho do lado do corpo
    internal void SetLado(double novoLado)
    {
        // Atualiza o tamanho do lado
        lado = novoLado;
    }

    // Retorna a temperatura atual do corpo
    internal double GetTemperatura()
    {
        // Devolve a temperatura armazenada
        return temperatura;
    }

    // Altera a temperatura atual do corpo
    internal void SetTemperatura(double novaTemperatura)
    {
        // Atualiza a temperatura do corpo
        temperatura = novaTemperatura;
    }

    // Calcula o volume do corpo considerando um cubo
    internal double CalcularVolume()
    {
        // Calcula o volume usando o lado elevado ao cubo
        return lado * lado * lado;
    }

    // Calcula a massa do corpo
    internal double CalcularMassa()
    {
        // Obtém a densidade do material
        double densidade = material.GetDensidade();

        // Calcula a massa usando densidade multiplicada pelo volume
        return densidade * CalcularVolume();
    }
}