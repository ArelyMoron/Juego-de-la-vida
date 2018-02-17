using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

// Elaborado por Arely M.
namespace Juego_de_la_vida
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Diseño Diseño;
        public int volver_id;
        // token de cancelacion que servira para cancelar el subproceso en el que se ejecuta la logica del juego
        private CancellationTokenSource CancellationToken;

        public MainWindow()
        {
            InitializeComponent();
            int color_tema = Properties.Settings.Default.Color;
            bool tema = Properties.Settings.Default.Tema;
            int color_celulas = Properties.Settings.Default.ColorCelulas;
            int tamaño_celulas = Properties.Settings.Default.TamañoCelulas;
            Celulas.Velocidad = (int)velocidad.Value;
            Diseño = new Diseño(this, color_tema, tema, color_celulas, tamaño_celulas);
            Diseño.Iniciar();
            Diseño.DibujarTablero((int)Width, (int)Height);

        }

        public static void Celula_Click(object sender, MouseButtonEventArgs e)
        {
            Rectangle celula = sender as Rectangle;
            if (celula.GetState())
                celula.SetState(false);
            else
                celula.SetState(true);
        }

        private void ajustes_Click(object sender, RoutedEventArgs e)
        {
            volver_id = 0;
            Diseño.MenuAjustes();
        }

        private void apariencia_Click(object sender, RoutedEventArgs e)
        {
            volver_id = 1;
            Diseño.MenuApariencia();
        }

        private void acercade_Click(object sender, RoutedEventArgs e)
        {
            volver_id = 1;
            Diseño.MenuAcercaDe();
        }

        private void volver_Click(object sender, RoutedEventArgs e)
        {
            if (volver_id == 1)
                GuardarConfiguracion();
            Diseño.Volver(volver_id);
        }

        private void Tema_Checked(object sender, RoutedEventArgs e)
        {
            Diseño.CambiarTema(true);
        }

        private void Tema_Unchecked(object sender, RoutedEventArgs e)
        {
            Diseño.CambiarTema(false);
        }

        private void ColorTema_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Diseño.CambiarColor_tema(ColorTema.SelectedIndex);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                Diseño.Maximizar();
            if (WindowState == WindowState.Normal)
                Diseño.Normal();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(WindowState != WindowState.Maximized)
               Diseño.CambiarTamaño((int)Width, (int)Height);
        }

        private void TamañoCelulas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Diseño.CambiarTamaño_Celulas(TamañoCelulas.SelectedIndex);
        }

        private void ColorCelulas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Diseño.CambiarColor_Celulas(ColorCelulas.SelectedIndex);
        }

        private void GuardarConfiguracion() // guardo las configuraciones
        {
            Properties.Settings.Default.Color = Diseño.ColorTema;
            Properties.Settings.Default.ColorCelulas = Diseño.ColorCelulas;
            Properties.Settings.Default.Tema = Diseño.Tema;
            Properties.Settings.Default.TamañoCelulas = Diseño.TamañoCelulas;
            Properties.Settings.Default.Save();
        }

        private async void iniciar_Click(object sender, RoutedEventArgs e)
        {
            if(CancellationToken == null)
            {
                CancellationToken = new CancellationTokenSource();
                aleatorio.IsEnabled = false;
                ajustes.IsEnabled = false;
                iniciar.IsEnabled = false;
                ResizeMode = ResizeMode.CanMinimize;
                await Celulas.IniciarJuego(Tablero.Children, CancellationToken.Token);
            }
        }

        private void pausar_Click(object sender, RoutedEventArgs e)
        {
            if (CancellationToken != null)
            {
                CancellationToken.Cancel();
                CancellationToken = null;
                aleatorio.IsEnabled = true;
                ajustes.IsEnabled = true;
                iniciar.IsEnabled = true;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void velocidad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Celulas.Velocidad = (int)velocidad.Value;
        }

        private void detener_Click(object sender, RoutedEventArgs e)
        {
            pausar_Click(sender, e);
            Celulas.SetStates(false, Tablero.Children);
        }

        private void aleatorio_Click(object sender, RoutedEventArgs e)
        {
            Celulas.SetStates(Tablero.Children);
        }
    }
}