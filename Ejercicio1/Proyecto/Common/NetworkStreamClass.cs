using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        // Escribir datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            // Serializar y enviar (pendiente implementación)
        }

        // Leer datos de tipo Carretera (pendiente)
        // public static Carretera LeerDatosCarreteraNS(NetworkStream NS) { }

        // Escribir datos de tipo Vehiculo
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            // Serializar y enviar (pendiente implementación)
        }

        // Leer datos de tipo Vehiculo (pendiente)
        // public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS) { }

        // Leer mensaje de texto
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] buffer = new byte[1024];
            int bytesLeidos = 0;
            using var tmp = new MemoryStream();
            do
            {
                int read = NS.Read(buffer, 0, buffer.Length);
                tmp.Write(buffer, 0, read);
                bytesLeidos += read;
            } while (NS.DataAvailable);

            return Encoding.Unicode.GetString(tmp.ToArray(), 0, bytesLeidos);
        }

        // Escribir mensaje de texto
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] msg = Encoding.Unicode.GetBytes(Str);
            NS.Write(msg, 0, msg.Length);
        }
    }
}
