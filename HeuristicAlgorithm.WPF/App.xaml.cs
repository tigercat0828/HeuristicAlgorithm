using HeuristicAlgorithm.WPF.Views;
using System.Windows;

namespace HeuristicAlgorithm.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {

    protected override void OnStartup(StartupEventArgs e) {
        var window = new MainWindow();
        window.Show();
    }
}
