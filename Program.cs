using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Ejer1_Tema6
{
    internal class Program
    {
        static Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416);
           

            using (s)
            {
                s.Bind(ie);

                s.Listen(100);

                Console.WriteLine($"Servidor iniciado. Escuchando en {ie.Address}:{ie.Port}");
                Console.WriteLine("Esperando conexiones... (Ctrl+C para salir)");

                try
                {
                    while (servFuncionando)
                    {
                        Socket sClient = s.Accept();
                        Thread hilo = new Thread(() => hiloClient(sClient));
                        hilo.Start();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Servidor cerrado");
                }

            }
        }

        static bool servFuncionando = true;

        private static void hiloClient(Socket sClient)
        {
            if (servFuncionando)
            {
                using (sClient)
                {
                    IPEndPoint ieClient = (IPEndPoint)sClient.RemoteEndPoint;
                    Console.WriteLine($"Cliente conectado:{ieClient.Address} en puerto {ieClient.Port}");


                    using (NetworkStream ns = new NetworkStream(sClient))
                    using (StreamReader sr = new StreamReader(ns))
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
                        sw.AutoFlush = true;
                        sw.WriteLine("Bienvenido, ¿Qué te gustaría realizar?");

                        try
                        {
                            string respuesta = sr.ReadLine();

                            string[] partes = respuesta.Split(' ');

                                switch (partes[0])
                                {

                                    case "time":

                                        sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));

                                        break;

                                    case "date":

                                        sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy"));

                                        break;


                                    case "all":

                                        sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                                        break;

                                    case "close":

                                        if (partes.Length == 2)
                                        {
                                            string ruta = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%") + "\\password.txt";
                                            string texto = "";
                                            using (StreamReader sr2 = new StreamReader(ruta))
                                            {
                                                texto = sr2.ReadLine();
                                            }

                                            if (partes[1] == texto)
                                            {
                                                sw.WriteLine("Cerrando servidor...");
                                                servFuncionando = false;
                                                s.Close();
                                            }
                                            else
                                            {
                                                sw.WriteLine("Has ingresado una contraseña inválida");
                                            }
                                        }
                                        else
                                        {
                                            sw.WriteLine("No has ingresado una contraseña");
                                        }
                                        break;

                                    default:

                                        sw.WriteLine("El comando que has introducido no existe");

                                        break;
                                }
                            
                        }
                        catch (IOException e) { }
                    }
                }
            }
        }
    }
}
