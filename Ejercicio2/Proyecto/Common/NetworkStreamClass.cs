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
        // Escribir datos de tipo Carretera (implementar cuando se necesite)
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            // TODO: Serializar y enviar C a través de NS
        }

        // Leer datos de tipo Carretera (implementar cuando se necesite)
        // public static Carretera LeerDatosCarreteraNS(NetworkStream NS) { }

        // Escribir datos de tipo Vehiculo (implementar cuando se necesite)
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            // TODO: Serializar y enviar V a través de NS
        }

        // Leer datos de tipo Vehiculo (implementar cuando se necesite)
        // public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS) { }

        // Leer mensaje de texto
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] buffer = new byte[1024];
            int bytesLeidos = 0;
            using MemoryStream ms = new MemoryStream();
            do
            {
                int read = NS.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, read);
                bytesLeidos += read;
            } while (NS.DataAvailable);

            return Encoding.Unicode.GetString(ms.ToArray(), 0, bytesLeidos);
        }

        // Escribir mensaje de texto
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] msg = Encoding.Unicode.GetBytes(Str);
            NS.Write(msg, 0, msg.Length);
        }
    }
}