using System.Collections.Generic;
using SimuladorTransferenciaCalor.Models;

namespace SimuladorTransferenciaCalor.Dados;

// Fornece os materiais disponíveis na simulação
internal class Materiais
{
    // Retorna uma lista com oito materiais
    internal static List<Material> GetMateriais()
    {
        // Cria a lista de materiais
        List<Material> materiais = new List<Material>();

        // Adiciona cobre
        materiais.Add(
            new Material(
                "Cobre",
                401.0,
                385.0,
                8960.0));

        // Adiciona alumínio
        materiais.Add(
            new Material(
                "Alumínio",
                237.0,
                897.0,
                2700.0));

        // Adiciona ferro
        materiais.Add(
            new Material(
                "Ferro",
                80.0,
                449.0,
                7874.0));

        // Adiciona aço
        materiais.Add(
            new Material(
                "Aço",
                50.0,
                486.0,
                7850.0));

        // Adiciona vidro
        materiais.Add(
            new Material(
                "Vidro",
                1.05,
                840.0,
                2500.0));

        // Adiciona madeira
        materiais.Add(
            new Material(
                "Madeira",
                0.15,
                1700.0,
                600.0));

        // Adiciona água
        materiais.Add(
            new Material(
                "Água",
                0.60,
                4186.0,
                1000.0));

        // Adiciona concreto
        materiais.Add(
            new Material(
                "Concreto",
                1.70,
                880.0,
                2400.0));

        // Retorna a lista completa
        return materiais;
    }
}