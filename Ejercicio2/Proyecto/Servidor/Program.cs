using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

namespace Servidor
{
    class Program
    {
        private static List<Cliente> _clientes = new List<Cliente>();
        private static Carretera _carretera = new Carretera();

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

            // Crear objeto cliente
            var nuevoCliente = new Cliente(client);
            _clientes.Add(nuevoCliente);
            Console.WriteLine($"[Servidor] Nuevo cliente conectado. Total clientes: {_clientes.Count}");

            try
            {
                using NetworkStream ns = client.GetStream();
                // Recibir datos de vehículos
                while (true)
                {
                    // Leer el mensaje del cliente
                    string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(ns);

                    // Aquí podrías recibir información del vehículo
                    // Supongamos que se recibe un vehículo
                    Vehiculo vehiculoRecibido = Vehiculo.BytesAVehiculo(Encoding.Unicode.GetBytes(mensaje));
                    _carretera.AñadirVehiculo(vehiculoRecibido);

                    // Enviar la simulación completa a todos los clientes
                    foreach (var c in _clientes)
                    {
                        NetworkStreamClass.EscribirMensajeNetworkStream(c.NetworkStream, Encoding.Unicode.GetString(_carretera.CarreteraABytes()));
                    }

                    Console.WriteLine($"[Servidor] Vehículo ID {vehiculoRecibido.Id} recibido y agregado. Total vehículos en carretera: {_carretera.NumVehiculosEnCarrera}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error con cliente: {ex.Message}");
            }
            finally
            {
                _clientes.Remove(nuevoCliente);
                client.Close();
                Console.WriteLine($"[Servidor] Cliente desconectado. Total clientes: {_clientes.Count}");
            }
        }
    }
    
    // Clase Cliente
    public class Cliente
    {
        public TcpClient TcpClient { get; }
        public NetworkStream NetworkStream { get; }

        public Cliente(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            NetworkStream = tcpClient.GetStream();
        }
    }
}
