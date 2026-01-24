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
        static void Main(string[] args)
        {
            initServer server = new initServer(31416);
            server.Start();
        }
    }
}
