using System;
using System.Collections.Generic;
using System.Threading;

namespace Servidor
{
    public class ControlDeTráfico
    {
        private string? VehiculoEnPuente = null; // Estado del puente: null si está libre
        private Queue<string> colaEsperandoNorte = new Queue<string>(); // Cola para los vehículos del Norte
        private Queue<string> colaEsperandoSur = new Queue<string>(); // Cola para los vehículos del Sur
        private readonly object lockObject = new object(); // Para sincronizar el acceso al puente

        // Método para simular que un vehículo intenta entrar al puente
        public void IntentarCruzarPuente(string direccion, string vehiculoId)
        {
            lock (lockObject) // Sincronizamos el acceso a la sección crítica
            {
                if (VehiculoEnPuente == null) // Si el puente está libre
                {
                    // El vehículo entra al puente
                    VehiculoEnPuente = vehiculoId;
                    Console.WriteLine($"Vehículo {vehiculoId} ({direccion}) entra al puente.");
                }
                else
                {
                    // Si el puente está ocupado, el vehículo debe esperar
                    Console.WriteLine($"Vehículo {vehiculoId} ({direccion}) espera: Puente ocupado por {VehiculoEnPuente}.");
                    if (direccion == "Norte")
                    {
                        colaEsperandoNorte.Enqueue(vehiculoId); // Encola el vehículo para el Norte
                    }
                    else
                    {
                        colaEsperandoSur.Enqueue(vehiculoId); // Encola el vehículo para el Sur
                    }
                }
            }
        }

        // Método para simular que un vehículo ha cruzado el puente
        public void CruceFinalizado(string vehiculoId)
        {
            lock (lockObject) // Sincronizamos el acceso a la sección crítica
            {
                if (VehiculoEnPuente == vehiculoId)
                {
                    Console.WriteLine($"Vehículo {vehiculoId} sale del puente.");

                    // Libera el puente
                    VehiculoEnPuente = null;

                    // Notifica al siguiente vehículo en espera (si lo hay)
                    NotificarSiguienteVehiculo();
                }
            }
        }

        // Método para notificar al siguiente vehículo en la cola que puede avanzar
        private void NotificarSiguienteVehiculo()
        {
            // Prioriza la cola de la dirección opuesta, luego la misma dirección
            if (colaEsperandoSur.Count > 0)
            {
                string siguienteVehiculoSur = colaEsperandoSur.Dequeue();
                Console.WriteLine($"Vehículo {siguienteVehiculoSur} (Sur) puede avanzar al puente.");
                VehiculoEnPuente = siguienteVehiculoSur; // El vehículo pasa al puente
            }
            else if (colaEsperandoNorte.Count > 0)
            {
                string siguienteVehiculoNorte = colaEsperandoNorte.Dequeue();
                Console.WriteLine($"Vehículo {siguienteVehiculoNorte} (Norte) puede avanzar al puente.");
                VehiculoEnPuente = siguienteVehiculoNorte; // El vehículo pasa al puente
            }
        }
    }

    public class Servidor
    {
        private ControlDeTráfico controlDeTráfico;

        public Servidor()
        {
            controlDeTráfico = new ControlDeTráfico();
        }

        // Método para manejar las solicitudes de los vehículos
        public void ProcesarSolicitud(string direccion, string vehiculoId)
        {
            controlDeTráfico.IntentarCruzarPuente(direccion, vehiculoId);
        }

        // Método para simular que un vehículo ha cruzado el puente
        public void CruceFinalizado(string vehiculoId)
        {
            controlDeTráfico.CruceFinalizado(vehiculoId);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Servidor servidor = new Servidor();

            // Simulación de vehículos intentando cruzar el puente
            servidor.ProcesarSolicitud("Norte", "Vehiculo1");
            Thread.Sleep(2000); // Espera de 2 segundos antes de otro intento
            servidor.ProcesarSolicitud("Sur", "Vehiculo2");
            Thread.Sleep(2000);
            servidor.ProcesarSolicitud("Norte", "Vehiculo3");

            // Simulación de vehículos cruzando el puente
            Thread.Sleep(5000); // El vehículo 1 cruza el puente
            servidor.CruceFinalizado("Vehiculo1");

            Thread.Sleep(3000); // El vehículo 2 cruza el puente
            servidor.CruceFinalizado("Vehiculo2");

            // El vehículo 3 ahora puede cruzar
            Thread.Sleep(2000);
            servidor.CruceFinalizado("Vehiculo3");
        }
    }
}
