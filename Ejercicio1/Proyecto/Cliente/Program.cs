using System;
using System.Net.Sockets;
using System.Text;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serverIp = "127.0.0.1";
            const int serverPort = 5000;

            try
            {
                Console.WriteLine($"[Cliente] Intentando conectar a {serverIp}:{serverPort}...");
                using (var client = new TcpClient())
                {
                    client.Connect(serverIp, serverPort);
                    Console.WriteLine("[Cliente] Conectado al servidor.");

                    using (var ns = client.GetStream())
                    {
                        // Etapa 1: enviar saludo
                        const string saludo = "Hola servidor, soy un vehículo";
                        NetworkStreamClass.EscribirMensajeNetworkStream(ns, saludo);
                        Console.WriteLine($"[Cliente] Enviado: {saludo}");

                        // Leer respuesta
                        var respuesta = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                        Console.WriteLine($"[Cliente] Recibido: {respuesta}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cliente] Error: {ex.Message}");
            }

            Console.WriteLine("[Cliente] Fin de ejecución. Pulsa Enter para cerrar.");
            Console.ReadLine();
        }
    }
}