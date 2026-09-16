namespace SimuladorTransferenciaCalor.Models;

// Representa um material usado na simulação térmica
internal class Material
{
    // Nome do material
    private string nome;

    // Condutividade térmica em W/(m·K)
    private double condutividade;

    // Calor específico em J/(kg·K)
    private double calorEspecifico;

    // Densidade em kg/m³
    private double densidade;

    // Construtor do material
    internal Material(
        string nome,
        double condutividade,
        double calorEspecifico,
        double densidade)
    {
        this.nome = nome;
        this.condutividade = condutividade;
        this.calorEspecifico = calorEspecifico;
        this.densidade = densidade;
    }

    // Retorna o nome do material
    internal string GetNome()
    {
        return nome;
    }

    // Altera o nome do material
    internal void SetNome(string nome)
    {
        this.nome = nome;
    }

    // Retorna a condutividade térmica
    internal double GetCondutividade()
    {
        return condutividade;
    }

    // Altera a condutividade térmica
    internal void SetCondutividade(double condutividade)
    {
        this.condutividade = condutividade;
    }

    // Retorna o calor específico
    internal double GetCalorEspecifico()
    {
        return calorEspecifico;
    }

    // Altera o calor específico
    internal void SetCalorEspecifico(double calorEspecifico)
    {
        this.calorEspecifico = calorEspecifico;
    }

    // Retorna a densidade
    internal double GetDensidade()
    {
        return densidade;
    }

    // Altera a densidade
    internal void SetDensidade(double densidade)
    {
        this.densidade = densidade;
    }
}