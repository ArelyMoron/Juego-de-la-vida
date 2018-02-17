using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
// Elaborado por Arely M.
namespace Juego_de_la_vida
{
    static class Celulas
    {
        /*
         * Nota:
         * Las celulas las represento mediante objetos de la clase Rectangle los cuales van colocados 
         * dentro de el tablero que represento con un Grid. 
         * Los metodos GetState, SetState, GetPosition y SetPosition son metodos de extension de la 
         * clase Rectangle
        */
        public static int Tamaño = 35; // representa el tamaño de la celula
        public static int Espacio = 1; // representa el espacio entre las celulas
        public static SolidColorBrush ColorCelula = ColorPrincipal(); // representa el color de la celula
        public static bool Aleatorio; // indica si el color de las celulas se elige de forma aleatoria
        private static bool[,] celulas, _celulas; // representa el estado de las celulas (true -> viva, false -> muerta)
        public static int Velocidad; // representa el tiempo que espero para cambiar el estado de las celulas

        public static bool GetState(this Rectangle celula) // obtiene el estado de una celula
        {
            SolidColorBrush color;
            color = celula.Fill as SolidColorBrush;
            if (color == ColorRelleno())
                return false;
            else
                return true;
        }

        public static void SetState(this Rectangle celula, bool state) // cambia el estado de una celula
        {
            int filas = (int)celula.GetPosition().GetValue(0);
            int columnas = (int)celula.GetPosition().GetValue(1);
            celulas[filas, columnas] = state;
            if (state)
            {
                if (!Aleatorio)
                    celula.Fill = ColorCelula;
                else
                {
                    Random rnd = new Random();
                    int numero = rnd.Next(0, 3);
                    if (numero == 0)
                    {
                        celula.Fill = ColorPrincipal();
                    }
                    else if (numero == 1)
                    {
                        celula.Fill = ColorSecundario();
                    }
                    else if (numero == 2)
                    {
                        celula.Fill = ColorCelula;
                    }

                }
            }
            else
            {
                SolidColorBrush color = ColorRelleno();
                celula.Fill = color;
            }
        }

        public static void SetPosition(this Rectangle celula, int x, int y) // obtiene la posicion de la celula en la matriz 
        {
            celula.Uid = "_" + x.ToString() + "_" + y.ToString() + "_";
        }

        public static int[] GetPosition(this Rectangle celula) // establece la posicion de la celula en la matriz 
        {
            int[] position = new int[2];
            string nombre = celula.Uid;
            nombre = nombre.Remove(0, 1);
            int x = Convert.ToInt32(nombre.Substring(0, nombre.IndexOf('_')));
            nombre = nombre.Remove(1, nombre.IndexOf('_'));
            int y = Convert.ToInt32(nombre.Substring(1, nombre.IndexOf('_') - 1));
            position[0] = x;
            position[1] = y;
            return position;
        }

        public static void SetStates(UIElementCollection tablero) // establezco el estado de todas las celulas de forma aleatoria 
        {
            Random rnd = new Random();
            bool aleatorio;
            foreach (Rectangle celula in tablero)
            {
                if (rnd.Next(2) == 0)
                    aleatorio = false;
                else
                    aleatorio = true;
                celula.SetState(aleatorio);
            }
        }

        public static void SetStates(bool state, UIElementCollection tablero)
        {
            foreach (Rectangle celula in tablero)
            {
                celula.SetState(state);
            }
        }

        public static void SetStates(bool[,] states, UIElementCollection tablero)
        {
            foreach (Rectangle celula in tablero)
            {
                int filas = (int)celula.GetPosition().GetValue(0);
                int columnas = (int)celula.GetPosition().GetValue(1);
                celula.SetState(states[filas, columnas]);
            }
        }

        public static SolidColorBrush ColorBorde() // obtengo el color del borde de la celula
        {
            return Application.Current.Resources["PrimaryHueLightForegroundBrush"] as SolidColorBrush;
        }

        public static SolidColorBrush ColorRelleno() // obtengo el color de relleno de la celula
        {
            return Application.Current.Resources["MaterialDesignPaper"] as SolidColorBrush;
        }

        public static SolidColorBrush ColorPrincipal()
        {
            return Application.Current.Resources["PrimaryHueLightBrush"] as SolidColorBrush;
        }

        public static SolidColorBrush ColorSecundario()
        {
            return Application.Current.Resources["SecondaryAccentBrush"] as SolidColorBrush;
        }

        public static SolidColorBrush ColorBasico(bool tema) // obtengo el color basico (blanco o negro) que le corresponde a la celula
        {
            if (tema)
                return Application.Current.Resources["MaterialDesignBody"] as SolidColorBrush;
            else
                return Application.Current.Resources["MaterialDesignBodyLight"] as SolidColorBrush;
        }

        public static async Task IniciarJuego(UIElementCollection tablero, CancellationToken cancellation) // inicia el juego o simulacion
        {
            int Filas = celulas.GetLength(0);
            int Columnas = celulas.GetLength(1);
            int vecinas_vivas;
            while (true)
            {
                await Task.Run(() => // ejecuto toda la logica del juego en un subproceso para evitar que se ralentize la app
                {
                    _celulas = (bool[,])celulas.Clone(); // hago una copia de la matriz de celulas
                    for (int filas = 0; filas < Filas; filas++)
                    {
                        for (int columnas = 0; columnas < Columnas; columnas++)
                        {
                            vecinas_vivas = ObtenerVecinasVivas(filas, columnas);

                            if (celulas[filas, columnas])
                            {
                                if (vecinas_vivas == 2 || vecinas_vivas == 3)
                                    _celulas[filas, columnas] = true;
                                else
                                    _celulas[filas, columnas] = false;
                            }
                            else
                            {
                                if (vecinas_vivas == 3)
                                    _celulas[filas, columnas] = true;
                                else
                                    _celulas[filas, columnas] = false;
                            }
                        }
                    }
                });
                await Task.Delay(Velocidad); // espero un momento para despues cambiar el estado de las celulas
                if (cancellation.IsCancellationRequested) 
                    return; // salgo del ciclo infinito si se presiono el boton para cancelar 
                SetStates(_celulas, tablero);
            }
        }

        private static int ObtenerVecinasVivas(int filas, int columnas) // obtengo las vecinas vivas que tiene una celula
        {
            int vecinas_vivas = 0;
            if (filas == 0)
            {
                if (columnas == 0)
                {
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas + 1])
                        vecinas_vivas++;
                }

                else if (columnas == celulas.GetLength(1) - 1)
                {
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas - 1])
                        vecinas_vivas++;
                }

                else
                {
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas + 1])
                        vecinas_vivas++;
                }
            }

            else if (filas == celulas.GetLength(0) - 1)
            {
                if (columnas == 0)
                {
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                }

                else if (columnas == celulas.GetLength(1) - 1)
                {
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                }

                else
                {
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                }
            }

            else
            {
                if (columnas == 0)
                {
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas + 1])
                        vecinas_vivas++;
                }

                else if (columnas == celulas.GetLength(1) - 1)
                {
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas - 1])
                        vecinas_vivas++;
                }

                else
                {
                    if (celulas[filas + 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas + 1, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas, columnas + 1])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas - 1])
                        vecinas_vivas++;
                    if (celulas[filas - 1, columnas + 1])
                        vecinas_vivas++;

                }
            }

            return vecinas_vivas;
        }

        public static void CrearCelulas(int filas, int columnas) // creo la matriz de celulas 
        {
            celulas = new bool[filas, columnas];
        }
    }
}
