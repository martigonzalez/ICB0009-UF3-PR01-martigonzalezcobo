using System;
using System.IO;
using System.Xml.Serialization;
using VehiculoClass;
using CarreteraClass;
using NetworkStreamNS;
using System.Net.Sockets;

namespace Cliente
{
    class Program
    {
        static TcpClient cliente = new TcpClient(); // Inicializa el cliente
        static NetworkStream ns;
        static Vehiculo vehiculo = new Vehiculo(); // Inicializa el vehículo

        static void Main(string[] args)
        {
            // Dirección del servidor y puerto
            string servidorIp = "127.0.0.1"; // IP del servidor
            int puerto = 12345; // Puerto que estará escuchando el servidor

            try
            {
                // Establecemos la conexión con el servidor
                cliente = new TcpClient(servidorIp, puerto);
                ns = cliente.GetStream();

                // Crear un nuevo vehículo
                vehiculo = new Vehiculo()
                {
                    Id = new Random().Next(1, 1000), // ID aleatorio
                    Pos = 0, // Inicia en la posición 0
                    Velocidad = new Random().Next(100, 500), // Velocidad aleatoria
                    Acabado = false,
                    Direccion = "Norte", // Puede ser "Norte" o "Sur"
                    Parado = false
                };

                // Enviar los datos del vehículo al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                Console.WriteLine($"Vehículo creado con ID: {vehiculo.Id} y enviado al servidor.");

                // Iniciar un hilo para recibir los datos de la carretera
                Thread recibirDatosThread = new Thread(RecibirDatosDelServidor);
                recibirDatosThread.Start();

                // Hacer que el vehículo avance (simulando su movimiento)
                AvanzarVehiculo();

                // Cerrar la conexión
                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Método para hacer avanzar el vehículo
        static void AvanzarVehiculo()
        {
            for (int i = 0; i <= 100; i++)
            {
                // Actualizar la posición del vehículo
                vehiculo.Pos = i;
                // Enviar la actualización al servidor
                try
                {
                    NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                    Console.WriteLine($"Vehículo {vehiculo.Id} avanzando a la posición {vehiculo.Pos}");
                    Thread.Sleep(1000); // Simula el tiempo que tarda en avanzar
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar datos del vehículo: {ex.Message}");
                }
            }

            // Al terminar, marcar el vehículo como acabado
            vehiculo.Acabado = true;
            try
            {
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                Console.WriteLine($"Vehículo {vehiculo.Id} ha terminado el recorrido.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar la información final del vehículo: {ex.Message}");
            }
        }

        // Método para recibir los datos del servidor de forma concurrente
        static void RecibirDatosDelServidor()
        {
            try
            {
                while (true)
                {
                    // Leer los datos de la carretera enviados por el servidor
                    Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(ns); // Cambiar a LeerDatosCarreteraNS
                    
                    // Mostrar los vehículos en la carretera
                    Console.Clear();
                    Console.WriteLine("Vehículos en la carretera:");
                    foreach (Vehiculo v in carreteraRecibida.VehiculosEnCarretera)  // Ahora accedes correctamente a la carretera
                    {
                        Console.WriteLine($"ID: {v.Id}, Posición: {v.Pos}, Dirección: {v.Direccion}, Acabado: {v.Acabado}");
                    }

                    // Si el vehículo ha terminado, mostramos un mensaje de ganador
                    if (vehiculo.Acabado)
                    {
                        Console.WriteLine($"¡El vehículo {vehiculo.Id} ha terminado su recorrido!");
                        break;
                    }

                    // Esperar un momento antes de recibir la siguiente actualización
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recibir los datos del servidor: {ex.Message}");
            }
        }
    }
}
