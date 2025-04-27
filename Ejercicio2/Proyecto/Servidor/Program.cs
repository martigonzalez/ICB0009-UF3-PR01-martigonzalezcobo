using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;
using System.Collections.Generic;
using System.Xml.Serialization;

class Servidor
{
    static List<TcpClient> clientesConectados = new List<TcpClient>();  // Lista para almacenar los clientes conectados
    static Carretera carretera = new Carretera();  // La carretera que contiene los vehículos

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

            while (true)
            {
                // Aceptar la conexión de un cliente
                TcpClient cliente = servidor.AcceptTcpClient();
                clientesConectados.Add(cliente);  // Agregar cliente a la lista de conectados
                NetworkStream ns = cliente.GetStream();
                Console.WriteLine("Cliente conectado.");

                // Crear un hilo para manejar la comunicación con el cliente
                Thread clienteThread = new Thread(() => ManejarCliente(cliente, ns));
                clienteThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Método para manejar la comunicación con un cliente específico
    static void ManejarCliente(TcpClient cliente, NetworkStream ns)
    {
        try
        {
            while (true)
            {
                // Leer el vehículo enviado por el cliente
                Vehiculo vehiculoRecibido = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                Console.WriteLine($"Vehículo recibido con ID: {vehiculoRecibido.Id}");

                // Actualizar la carretera con el vehículo recibido
                carretera.ActualizarVehiculo(vehiculoRecibido);

                // Enviar los datos de la carretera a todos los clientes
                EnviarDatosATodosLosClientes();

                // Esperar un poco antes de continuar (simula la actualización periódica)
                Thread.Sleep(500);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la comunicación con el cliente: {ex.Message}");
        }
        finally
        {
            // Cerrar la conexión con el cliente cuando se desconecte
            clientesConectados.Remove(cliente);
            cliente.Close();
        }
    }

    // Método para enviar los datos de la carretera a todos los clientes conectados
    static void EnviarDatosATodosLosClientes()
    {
        // Serializar la carretera
        byte[] carreteraBytes = carretera.CarreteraABytes();

        // Recorrer todos los clientes conectados y enviarles los datos de la carretera
        foreach (TcpClient cliente in clientesConectados)
        {
            try
            {
                NetworkStream ns = cliente.GetStream();
                // Cambiar esta línea
                NetworkStreamClass.EscribirDatosCarreteraNS(ns, carretera);  // Usamos EscribirDatosCarreteraNS

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar datos al cliente: {ex.Message}");
            }
        }
    }
}
