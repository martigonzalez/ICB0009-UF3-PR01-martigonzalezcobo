using System;
using System.Net.Sockets;
using NetworkStreamNS;
using VehiculoClass;
using CarreteraClass;

namespace Cliente
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

                using NetworkStream ns = client.GetStream();

                // Recibir simulación de la carretera
                while (true)
                {
                    string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(ns);

                    // Deserializar la carretera
                    Carretera carreteraRecibida = Carretera.BytesACarretera(System.Text.Encoding.Unicode.GetBytes(mensaje));

                    // Mostrar la simulación
                    foreach (var vehiculo in carreteraRecibida.VehiculosEnCarretera)
                    {
                        Console.WriteLine($"[{vehiculo.Direccion}] Vehículo #{vehiculo.Id}: {'█' * (vehiculo.Pos / 2)}{'▒' * ((100 - vehiculo.Pos) / 2)} (km {vehiculo.Pos} - {(vehiculo.Parado ? "Esperando" : "Cruzando")})");
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
