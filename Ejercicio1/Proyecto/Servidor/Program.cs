using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Servidor
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 5000;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"[Servidor] Escuchando en el puerto {port}...");

            // Etapa 2: aceptar clientes concurrentemente
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Servidor] Cliente conectado, lanzando hilo de gestión...");
                var t = new Thread(HandleClient);
                t.Start(client);
            }
        }

        static void HandleClient(object? state)
        {
            if (state is not TcpClient client)
            {
                Console.WriteLine("[Servidor] Estado inválido en el hilo de cliente.");
                return;
            }

            Console.WriteLine("[Servidor] Gestionando nuevo vehículo...");
            try
            {
                using (var ns = client.GetStream())
                {
                    // Leer saludo del cliente
                    var mensaje = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                    Console.WriteLine($"[Servidor] Recibido: {mensaje}");

                    // Enviar confirmación
                    const string respuesta = "Vehículo recibido";
                    NetworkStreamClass.EscribirMensajeNetworkStream(ns, respuesta);
                    Console.WriteLine($"[Servidor] Respondido: {respuesta}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error manejando cliente: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("[Servidor] Conexión con vehículo cerrada.");
            }
        }
    }
}
