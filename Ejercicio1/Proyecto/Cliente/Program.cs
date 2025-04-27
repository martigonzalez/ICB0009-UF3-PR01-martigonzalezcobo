using System;
using System.Net.Sockets;
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
                using var client = new TcpClient();
                client.Connect(serverIp, serverPort);
                Console.WriteLine("[Cliente] Conectado al servidor.");

                // Etapa 4: obtener NetworkStream
                using NetworkStream ns = client.GetStream();
                Console.WriteLine("[Cliente] NetworkStream obtenido.");

                // Etapa 6: Handshake
                // 1. Cliente inicia handshake
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INICIO");
                Console.WriteLine("[Cliente] Handshake: enviado 'INICIO'.");

                // 2. Cliente recibe ID asignado
                string idAsignado = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Handshake: recibido ID '{idAsignado}'.");

                // 3. Cliente envía confirmación del ID
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, idAsignado);
                Console.WriteLine($"[Cliente] Handshake: enviado confirmación ID '{idAsignado}'.");

                Console.WriteLine("[Cliente] Handshake completado. Cliente listo para ejecutar.");
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