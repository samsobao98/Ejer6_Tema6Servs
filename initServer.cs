using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Ejer1_Tema6
{
    internal class initServer
    {
        private Socket serverSocket;
        private bool servFuncionando;
        private int port;

        public initServer(int port)
        {
            this.port = port;
            servFuncionando = true;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            bool puertoEnUso = true;

            while (puertoEnUso && port <= 65535)
            {
                try
                {
                    IPEndPoint ie = new IPEndPoint(IPAddress.Any, port);
                    serverSocket.Bind(ie);

                    Console.WriteLine($"¡Puerto {port} disponible!");
                    puertoEnUso = false;
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"Puerto {port} en uso, probando siguiente...");
                    port++;

                }
            }

            serverSocket.Listen(100);

            Console.WriteLine($"Servidor iniciado. Escuchando en {port}");
            Console.WriteLine("Esperando conexiones... (Ctrl+C para salir)");

            try
            {
                while (servFuncionando)
                {
                    Socket sClient = serverSocket.Accept();
                    Thread hilo = new Thread(() => hiloClient(sClient));
                    hilo.Start();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Servidor cerrado");
            }
        }

        private void hiloClient(Socket sClient)
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
                                            serverSocket.Close();
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
