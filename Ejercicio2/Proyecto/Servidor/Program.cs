using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;
using System.Xml.Serialization;

class Servidor
{
    static void Main(string[] args)
    {
        // Dirección y puerto del servidor
        string servidorIp = "127.0.0.1"; // IP del servidor
        int puerto = 12345; // Puerto donde escucha el servidor

        try
        {
            // Crear un servidor TCP
            TcpListener servidor = new TcpListener(IPAddress.Parse(servidorIp), puerto);
            servidor.Start();
            Console.WriteLine("Servidor iniciado. Esperando clientes...");

            // Crear una carretera para almacenar los vehículos
            Carretera carretera = new Carretera();

            while (true)
            {
                // Aceptar la conexión de un cliente
                TcpClient cliente = servidor.AcceptTcpClient();
                NetworkStream ns = cliente.GetStream();
                Console.WriteLine("Cliente conectado.");

                // Leer los datos del vehículo enviado por el cliente
                Vehiculo vehiculoRecibido = null;

                // Bucle que maneja la actualización del vehículo
                while (true)
                {
                    // Leer el vehículo desde el cliente
                    vehiculoRecibido = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                    Console.WriteLine($"Vehículo recibido con ID: {vehiculoRecibido.Id} - Pos: {vehiculoRecibido.Pos}");

                    // Actualizar la carretera con el vehículo
                    carretera.ActualizarVehiculo(vehiculoRecibido);

                    // Mostrar los vehículos en la carretera
                    string vehiculosEnCarretera = "Vehículos en la carretera:\n";
                    foreach (Vehiculo v in carretera.VehiculosEnCarretera)
                    {
                        vehiculosEnCarretera += $"ID: {v.Id} - Pos: {v.Pos} - Dir: {v.Direccion}\n";
                    }

                    // Enviar la lista de vehículos al cliente
                    NetworkStreamClass.EscribirMensajeNetworkStream(ns, vehiculosEnCarretera);
                    Console.WriteLine("Vehículos enviados al cliente.");

                    // Si el vehículo ha terminado su recorrido, salir del bucle
                    if (vehiculoRecibido.Acabado)
                    {
                        Console.WriteLine($"El vehículo ID: {vehiculoRecibido.Id} ha completado su recorrido.");
                        break;
                    }

                    // Esperar antes de leer el siguiente paquete
                    Thread.Sleep(500);
                }

                // Cerrar la conexión con el cliente
                cliente.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
