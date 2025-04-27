using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
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
        
        // Lista de clientes conectados
        private static List<Cliente> _clientesConectados = new List<Cliente>();
        private static readonly object _clientesLock = new object();

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

            int id;
            string direccion;
            lock (_idLock)
            {
                id = _nextId++;
                direccion = _rand.Next(2) == 0 ? "Norte" : "Sur";
            }
            Vehiculo vehiculo = new Vehiculo { Id = id, Direccion = direccion };
            Cliente? nuevoCliente = null; // Cambio clave: añadir '?' para nullable

            try
            {
                NetworkStream ns = client.GetStream();
                Console.WriteLine($"[Servidor] NetworkStream obtenido para vehículo ID {vehiculo.Id}.");

                // Handshake
                string inicio = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake: recibido '{inicio}' de ID provisional {vehiculo.Id}.");

                NetworkStreamClass.EscribirMensajeNetworkStream(ns, vehiculo.Id.ToString());
                Console.WriteLine($"[Servidor] Handshake: enviado ID '{vehiculo.Id}'.");

                string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake: recibido confirmación ID '{confirmacion}'.");

                // Añadir a lista de clientes
                nuevoCliente = new Cliente { 
                    Id = id, 
                    NetworkStream = ns 
                };
                
                lock (_clientesLock)
                {
                    _clientesConectados.Add(nuevoCliente);
                    Console.WriteLine($"[Servidor] Cliente ID {id} añadido. Total conectados: {_clientesConectados.Count}");
                }

                Console.WriteLine($"[Servidor] Handshake completado con vehículo ID {vehiculo.Id}. Cliente listo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error en handshake con vehículo ID {id}: {ex.Message}");
            }
            finally
            {
                if (nuevoCliente != null)
                {
                    lock (_clientesLock)
                    {
                        _clientesConectados.Remove(nuevoCliente);
                        Console.WriteLine($"[Servidor] Cliente ID {nuevoCliente.Id} eliminado. Total conectados: {_clientesConectados.Count}");
                    }
                }
                client.Close();
                Console.WriteLine($"[Servidor] Conexión con vehículo ID {id} cerrada.");
            }
        }
    }
}