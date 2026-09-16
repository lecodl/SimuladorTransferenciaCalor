using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SimuladorTransferenciaCalor.View;

namespace SimuladorTransferenciaCalor;

// Representa a aplicação Avalonia
internal class App : Application
{
    // Inicializa os recursos visuais da aplicação
    public override void Initialize()
    {
        // Carrega o arquivo App.axaml
        AvaloniaXamlLoader.Load(this);
    }

    // Executa a aplicação depois da inicialização
    public override void OnFrameworkInitializationCompleted()
    {
        // Verifica se a aplicação usa desktop
        if (ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Define a janela principal
            desktop.MainWindow = new MainWindow();
        }

        // Finaliza a inicialização do framework
        base.OnFrameworkInitializationCompleted();
    }
}