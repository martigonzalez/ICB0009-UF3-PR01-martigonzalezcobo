using System;
using System.IO;
using System.Xml.Serialization;

namespace VehiculoClass
{
    [Serializable]
    public class Vehiculo
    {
        public int Id { get; set; }
        public int Pos { get; set; }
        public int Velocidad { get; set; }
        public string Direccion { get; set; }
        public bool Acabado { get; set; }
        public bool Parado { get; set; }

        public Vehiculo()
        {
            var rnd = new Random();
            Velocidad = rnd.Next(100, 500);
            Pos = 0;
            Acabado = false;
        }

        public byte[] VehiculoaBytes()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Vehiculo));
            using MemoryStream ms = new MemoryStream();
            ser.Serialize(ms, this);
            return ms.ToArray();
        }

        public static Vehiculo BytesAVehiculo(byte[] data)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Vehiculo));
            using MemoryStream ms = new MemoryStream(data);
            return (Vehiculo)ser.Deserialize(ms)!;
        }
    }
}