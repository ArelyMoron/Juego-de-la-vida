using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
// Elaborado por Arely M.
namespace Juego_de_la_vida
{
    class Diseño
    {
        private static MainWindow Aplicacion { get; set; }
        public static int ColorTema;
        public static bool Tema;
        public static int ColorCelulas;
        public static int TamañoCelulas;
        private bool dibujar; // esto va a indicar si se tiene que dibujar el tablero
        
        public Diseño(MainWindow window, int color_tema, bool tema, int color_celulas, int tamaño_celulas)
        {
            Aplicacion = window; // obtengo una referencia a la clase principal para poder modificar los controles
            ColorTema = color_tema;
            Tema = tema;
            ColorCelulas = color_celulas;
            TamañoCelulas = tamaño_celulas;
        }

        public void Iniciar()
        {
            MenuPrincipal();
            PonerColores_tema();
            PonerColores_celulas(Tema, ColorTema);
            PonerTamaño_celulas();
            CambiarTema(Tema);
            Aplicacion.Tema.IsChecked = Tema;
            Aplicacion.AcercaDe.Text = "Juego de la vida \n\nElaborado por Arely Moron \n\nVersion 18.1";
            Aplicacion.ColorTema.SelectedIndex = ColorTema;
            Aplicacion.ColorCelulas.SelectedIndex = ColorCelulas;
            Aplicacion.TamañoCelulas.SelectedIndex = TamañoCelulas;
        }

        public void MenuPrincipal() // muestra el menu principal de la app
        {
            Aplicacion.Tablero.Visibility = Visibility.Visible;
            Aplicacion.BarraDeAccion.Visibility = Visibility.Visible;
            Aplicacion.MenuAjustes.Visibility = Visibility.Hidden;
            Aplicacion.BarraVolver.Visibility = Visibility.Hidden;
            Aplicacion.MenuApariencia.Visibility = Visibility.Hidden;
            Aplicacion.MenuAcercaDe.Visibility = Visibility.Hidden;
            dibujar = true;
        }

        public void MenuAjustes() // muestra el menu de ajustes de la app
        {
            Aplicacion.MenuAjustes.Visibility = Visibility.Visible;
            Aplicacion.BarraVolver.Visibility = Visibility.Visible;
            Aplicacion.Tablero.Visibility = Visibility.Hidden;
            Aplicacion.BarraDeAccion.Visibility = Visibility.Hidden;
            Aplicacion.MenuApariencia.Visibility = Visibility.Hidden;
            Aplicacion.MenuAcercaDe.Visibility = Visibility.Hidden;
            dibujar = false;
        }

        public void MenuApariencia() // muestra el menu donde se configura la apariencia de la app
        {
            Aplicacion.MenuApariencia.Visibility = Visibility.Visible;
            Aplicacion.BarraVolver.Visibility = Visibility.Visible;
            Aplicacion.MenuAjustes.Visibility = Visibility.Hidden;
            Aplicacion.Tablero.Visibility = Visibility.Hidden;
            Aplicacion.BarraDeAccion.Visibility = Visibility.Hidden;
            Aplicacion.MenuAcercaDe.Visibility = Visibility.Hidden;
            dibujar = false;
        }

        public void MenuAcercaDe() // muestra el menu donde muestro informacion acerca de mi 
        {
            Aplicacion.MenuAcercaDe.Visibility = Visibility.Visible;
            Aplicacion.BarraVolver.Visibility = Visibility.Visible;
            Aplicacion.MenuAjustes.Visibility = Visibility.Hidden;
            Aplicacion.Tablero.Visibility = Visibility.Hidden;
            Aplicacion.BarraDeAccion.Visibility = Visibility.Hidden;
            Aplicacion.MenuApariencia.Visibility = Visibility.Hidden;
            dibujar = false;
        }

        public void Volver(int id) // controlo la logica del boton volver para regresar al menu anterior
        {
            switch(id)
            {
                case 0:
                    {
                        MenuPrincipal();
                        if (dibujar)
                            DibujarTablero((int)Aplicacion.ActualWidth, (int)Aplicacion.ActualHeight);
                        break;
                    }
                case 1:
                    {
                        MenuAjustes();
                        Aplicacion.volver_id = 0;
                        break;
                    }
            }
        }

        public async void DibujarTablero(int ancho, int alto) // dibujo el tablero
        {
            Aplicacion.Tablero.Children.Clear();
            await Task.Run(() =>
            {
                int aux = ancho / Celulas.Tamaño;
                aux = aux * Celulas.Espacio;
                ancho = ancho - aux;
                int filas = ancho / Celulas.Tamaño;

                alto = alto - (int)Aplicacion.Principal.RowDefinitions[2].ActualHeight;
                aux = alto / Celulas.Tamaño;
                aux = aux * Celulas.Espacio;
                alto = alto - aux;
                int columnas = alto / Celulas.Tamaño;
                int num1 = Celulas.Espacio;
                int num2 = Celulas.Espacio;
                Celulas.CrearCelulas(columnas, filas);
                for (int j = 0; j < filas; j++)
                {
                    for (int i = 0; i < columnas - 1; i++)
                    {
                        Aplicacion.Dispatcher.Invoke(() =>
                        {
                            Rectangle celula = new Rectangle();
                            celula.SetPosition(i, j);
                            celula.HorizontalAlignment = HorizontalAlignment.Left;
                            celula.VerticalAlignment = VerticalAlignment.Top;
                            celula.Height = Celulas.Tamaño;
                            celula.Width = Celulas.Tamaño;
                            celula.Stroke = Celulas.ColorBorde();
                            celula.Margin = new Thickness(num2, num1, 0, 0);
                            celula.Fill = Celulas.ColorRelleno();
                            celula.MouseLeftButtonDown += MainWindow.Celula_Click;
                            Aplicacion.Tablero.Children.Add(celula);
                        });
                        num1 += Celulas.Tamaño + Celulas.Espacio;
                    }
                    num1 = Celulas.Espacio;
                    num2 += Celulas.Tamaño + Celulas.Espacio;
                }
            });
        }

        private void PonerColores_tema()
        {
            Aplicacion.ColorTema.Items.Add("Rosa");
            Aplicacion.ColorTema.Items.Add("Morado");
            Aplicacion.ColorTema.Items.Add("Azul");
            Aplicacion.ColorTema.Items.Add("Amarillo");
            Aplicacion.ColorTema.Items.Add("Verde");

        }

        private void PonerColores_celulas(bool tema, int color)
        {
            int ColorAnterior = Aplicacion.ColorCelulas.SelectedIndex;
            Aplicacion.ColorCelulas.Items.Clear();
            Aplicacion.ColorCelulas.Items.Add("Aleatorio");
            switch(color)
            {
                case 0:
                    {
                        Aplicacion.ColorCelulas.Items.Add("Rosa");
                        Aplicacion.ColorCelulas.Items.Add("Morado");
                        break;
                    }

                case 1:
                    {
                        Aplicacion.ColorCelulas.Items.Add("Morado");
                        Aplicacion.ColorCelulas.Items.Add("Verde");
                        break;
                    }

                case 2:
                    {
                        Aplicacion.ColorCelulas.Items.Add("Azul");
                        Aplicacion.ColorCelulas.Items.Add("Verde");
                        break;
                    }

                case 3:
                    {
                        Aplicacion.ColorCelulas.Items.Add("Amarillo");
                        Aplicacion.ColorCelulas.Items.Add("Rojo");
                        break;
                    }

                case 4:
                    {
                        Aplicacion.ColorCelulas.Items.Add("Verde");
                        Aplicacion.ColorCelulas.Items.Add("Amarillo");
                        break;
                    }
            }
            if (tema)
                Aplicacion.ColorCelulas.Items.Add("Blanco");
            else
                Aplicacion.ColorCelulas.Items.Add("Negro");
            Aplicacion.ColorCelulas.SelectedIndex = ColorAnterior;
        }

        private void PonerTamaño_celulas()
        {
            Aplicacion.TamañoCelulas.Items.Add("Chico");
            Aplicacion.TamañoCelulas.Items.Add("Mediano");
            Aplicacion.TamañoCelulas.Items.Add("Grande");
        }

        public void CambiarTema(bool tema) // cambia el tema de la app entre claro y oscuro
        {
            if(tema)
            {
                Aplicacion.LbTema.Content = "Tema Oscuro";
                new PaletteHelper().SetLightDark(tema);
            }
            else
            {
                Aplicacion.LbTema.Content = "Tema Claro";
                new PaletteHelper().SetLightDark(tema);
            }
            Tema = tema;
            PonerColores_celulas(Tema, ColorTema);
        }

        public void CambiarColor_tema(int color) // cambio el color de la app
        {
            switch(color)
            {
                case 0:
                    {
                        new PaletteHelper().ReplacePrimaryColor("Pink");
                        new PaletteHelper().ReplaceAccentColor("Purple");
                        break;
                    }

                case 1:
                    {
                        new PaletteHelper().ReplacePrimaryColor("DeepPurple");
                        new PaletteHelper().ReplaceAccentColor("Lime");
                        break;
                    }

                case 2:
                    {
                        new PaletteHelper().ReplacePrimaryColor("Blue");
                        new PaletteHelper().ReplaceAccentColor("Lime");
                        break;
                    }

                case 3:
                    {
                        new PaletteHelper().ReplacePrimaryColor("Amber");
                        new PaletteHelper().ReplaceAccentColor("Red");
                        break;
                    }

                case 4:
                    {
                        new PaletteHelper().ReplacePrimaryColor("Green");
                        new PaletteHelper().ReplaceAccentColor("Yellow");
                        break;
                    }
            }
            ColorTema = color;
            PonerColores_celulas(Tema, ColorTema);
        }

        public void Maximizar() // relizo los ajustes necesarios cuando se maximiza la app
        {
            Aplicacion.BarraVolver.Width = SystemParameters.FullPrimaryScreenWidth;
            Aplicacion.volver.Margin = new Thickness(1, 15, 0, 0);
            if(dibujar && Aplicacion.volver_id == 0)
                DibujarTablero((int)SystemParameters.FullPrimaryScreenWidth, (int)SystemParameters.FullPrimaryScreenHeight);
        }

        public void Normal() // relizo los ajustes necesarios cuando la app vuelve a su estado normal
        {
            Aplicacion.Height = 650;
            Aplicacion.Width = 900;
            Aplicacion.BarraVolver.Width = 895;
            Aplicacion.volver.Margin = new Thickness(5, 5, 0, 0);
            if(dibujar && Aplicacion.volver_id == 0)
                DibujarTablero(900, 650);
        }

        public void CambiarTamaño(int ancho, int alto) // relizo los ajustes necesarios cuando se cambia el tamaño de la app
        {
            Aplicacion.BarraVolver.Width = ancho;
            if(dibujar && Aplicacion.volver_id == 0)
                DibujarTablero(ancho, alto);
        }

        public void CambiarTamaño_Celulas(int tamaño)
        {
            switch(tamaño)
            {
                case 0:
                    {
                        Celulas.Tamaño = 25;
                        break;
                    }

                case 1:
                    {
                        Celulas.Tamaño = 35;
                        break;
                    }

                case 2:
                    {
                        Celulas.Tamaño = 45;
                        break;
                    }
            }
            dibujar = true;
            TamañoCelulas = tamaño;
        }

        public void CambiarColor_Celulas(int color)
        {
            switch(color)
            {
                case 0:
                    {
                        Celulas.ColorCelula = Celulas.ColorBasico(Tema);
                        Celulas.Aleatorio = true;
                        break;
                    }

                case 1:
                    {
                        Celulas.ColorCelula = Celulas.ColorPrincipal();
                        Celulas.Aleatorio = false;
                        break;
                    }

                case 2:
                    {
                        Celulas.ColorCelula = Celulas.ColorSecundario();
                        Celulas.Aleatorio = false;
                        break;
                    }

                case 3:
                    {
                        Celulas.ColorCelula = Celulas.ColorBasico(Tema);
                        Celulas.Aleatorio = false;
                        break;
                    }
            }
            dibujar = true;
            ColorCelulas = color;
        }

    }
}
