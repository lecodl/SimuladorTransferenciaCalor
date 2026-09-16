using Avalonia;

namespace SimuladorTransferenciaCalor;


// Classe inicial do programa
internal class Program
{

    // Método principal que inicia a aplicação
    public static void Main(string[] args)
    {
        // Inicia o Avalonia como aplicação desktop
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }


    // Configuração inicial do Avalonia
    static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}