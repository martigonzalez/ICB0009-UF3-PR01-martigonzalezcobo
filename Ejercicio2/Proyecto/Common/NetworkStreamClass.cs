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
            // Serializamos el objeto Carretera en bytes
            byte[] datosCarretera = C.CarreteraABytes();

            // Escribimos los bytes en el NetworkStream
            NS.Write(datosCarretera, 0, datosCarretera.Length);
        }

        // Leer datos de tipo Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            byte[] buffer = new byte[1024]; // Tamaño de buffer para la lectura
            int bytesLeidos = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                // Leemos hasta que no haya más datos disponibles
                do
                {
                    int read = NS.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, read);
                    bytesLeidos += read;
                } while (NS.DataAvailable);

                // Deserializamos los bytes leídos en un objeto Carretera
                return Carretera.BytesACarretera(ms.ToArray());
            }
        }

        // Escribir datos de tipo Vehiculo
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            // Serializamos el objeto Vehiculo en bytes
            XmlSerializer serializer = new XmlSerializer(typeof(Vehiculo));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, V);
                byte[] datosVehiculo = ms.ToArray();

                // Escribimos los bytes en el NetworkStream
                NS.Write(datosVehiculo, 0, datosVehiculo.Length);
            }
        }

        // Leer datos de tipo Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            byte[] buffer = new byte[1024]; // Tamaño de buffer para la lectura
            int bytesLeidos = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                // Leemos hasta que no haya más datos disponibles
                do
                {
                    int read = NS.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, read);
                    bytesLeidos += read;
                } while (NS.DataAvailable);

                // Deserializamos los bytes leídos en un objeto Vehiculo
                XmlSerializer serializer = new XmlSerializer(typeof(Vehiculo));
                ms.Seek(0, SeekOrigin.Begin); // Nos aseguramos de posicionarnos al principio del stream
                return (Vehiculo)serializer.Deserialize(ms);
            }
        }

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
