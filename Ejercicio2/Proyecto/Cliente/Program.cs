using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using System.Xml.Serialization;

class Cliente
{
    static void Main(string[] args)
    {
        // Dirección del servidor y puerto
        string servidorIp = "127.0.0.1"; // IP del servidor
        int puerto = 12345; // Puerto que estará escuchando el servidor

        try
        {
            // Establecemos la conexión con el servidor
            TcpClient cliente = new TcpClient(servidorIp, puerto);
            NetworkStream ns = cliente.GetStream();

            // Crear un nuevo vehículo
            Vehiculo nuevoVehiculo = new Vehiculo()
            {
                Id = new Random().Next(1, 1000), // ID aleatorio
                Pos = 0, // Inicia en la posición 0
                Velocidad = new Random().Next(100, 500), // Velocidad aleatoria
                Acabado = false,
                Direccion = "Norte", // Puede ser "Norte" o "Sur"
                Parado = false
            };

            // Enviar el vehículo al servidor
            NetworkStreamClass.EscribirDatosVehiculoNS(ns, nuevoVehiculo);
            Console.WriteLine($"Vehículo creado con ID: {nuevoVehiculo.Id} y enviado al servidor.");

            // Avanzar el vehículo en un bucle de 0 a 100
            for (int i = 1; i <= 100; i++)
            {
                // Actualizar la posición del vehículo
                nuevoVehiculo.Pos = i;

                // Si el vehículo ha llegado al final, marcarlo como acabado
                if (i == 100)
                {
                    nuevoVehiculo.Acabado = true;
                    Console.WriteLine("Vehículo ha completado su recorrido.");
                }

                // Enviar los datos actualizados del vehículo al servidor
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, nuevoVehiculo);

                // Mostrar el avance en la consola
                Console.WriteLine($"Vehículo ID: {nuevoVehiculo.Id} - Pos: {nuevoVehiculo.Pos}");

                // Simular el tiempo de avance según la velocidad del vehículo
                Thread.Sleep(1000 / (nuevoVehiculo.Velocidad / 100)); // La velocidad influye en la pausa

                // Si el vehículo ha terminado, salir del bucle
                if (nuevoVehiculo.Acabado)
                {
                    break;
                }
            }

            // Esperar un poco para que el servidor procese los datos y actualice la carretera
            Thread.Sleep(1000);

            // Mostrar los vehículos de la carretera (en este ejemplo solo se muestra el ID)
            string respuesta = NetworkStreamClass.LeerMensajeNetworkStream(ns);
            Console.WriteLine("Vehículos en la carretera:");
            Console.WriteLine(respuesta);

            // Cerrar la conexión
            cliente.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
