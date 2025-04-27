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
        private static int _nextId = 1;
        private static readonly object _idLock = new object();
        private static readonly Random _rand = new Random();

        static void Main(string[] args)
        {
            const int port = 5000;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"[Servidor] Escuchando en el puerto {port}...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Servidor] Cliente conectado, lanzando hilo de gestión...");
                Thread t = new Thread(HandleClient);
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

            // Etapa 3: asignar ID único y dirección aleatoria
            int id;
            string direccion;
            lock (_idLock)
            {
                id = _nextId++;
                direccion = _rand.Next(2) == 0 ? "Norte" : "Sur";
            }
            Vehiculo vehiculo = new Vehiculo { Id = id, Direccion = direccion };
            Console.WriteLine($"[Servidor] Asignado ID {vehiculo.Id} y dirección {vehiculo.Direccion} al vehículo.");

            try
            {
                // Etapa 4: obtener NetworkStream
                using NetworkStream ns = client.GetStream();
                Console.WriteLine($"[Servidor] NetworkStream obtenido para vehículo ID {vehiculo.Id}.");

                // Etapa 6: Handshake
                // 1. Servidor recibe petición 'INICIO'
                string inicio = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake: recibido '{inicio}' de ID provisional {vehiculo.Id}.");

                // 2. Servidor envía ID asignado
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, vehiculo.Id.ToString());
                Console.WriteLine($"[Servidor] Handshake: enviado ID '{vehiculo.Id}'.");

                // 3. Servidor recibe confirmación del ID
                string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake: recibido confirmación ID '{confirmacion}'.");

                Console.WriteLine($"[Servidor] Handshake completado con vehículo ID {vehiculo.Id}. Cliente listo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error en handshake con vehículo ID {id}: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"[Servidor] Conexión con vehículo ID {id} cerrada.");
            }
        }
    }
}