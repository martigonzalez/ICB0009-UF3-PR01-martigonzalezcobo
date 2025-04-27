using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using VehiculoClass;

namespace CarreteraClass
{
    [Serializable]
    public class Carretera
    {
        public List<Vehiculo> VehiculosEnCarretera = new List<Vehiculo>();
        public int NumVehiculosEnCarrera = 0;

        public Carretera()
        {
        }

        // Crea un nuevo vehículo
        public void CrearVehiculo()
        {
            Vehiculo V = new Vehiculo();
            VehiculosEnCarretera.Add(V);
        }

        // Añade un vehículo ya creado a la lista de vehículos en carretera
        public void AñadirVehiculo(Vehiculo V)
        {
            VehiculosEnCarretera.Add(V);
            NumVehiculosEnCarrera++;
        }

        // Actualiza los datos de un vehículo ya existente en la lista de vehículos en carretera
        public void ActualizarVehiculo(Vehiculo V)
        {
            // Buscamos el vehículo en la lista por su ID
            Vehiculo veh = VehiculosEnCarretera.FirstOrDefault(x => x.Id == V.Id);
            if (veh != null)
            {
                // Actualizamos su posición, velocidad y estado de finalización
                veh.Pos = V.Pos;
                veh.Velocidad = V.Velocidad;
                veh.Acabado = V.Acabado;
                veh.Direccion = V.Direccion;
            }
        }

        // Muestra por pantalla los vehículos en la carretera (ID, posición y dirección)
        public void MostrarVehiculos()
        {
            string strVehs = "Vehículos en la carretera:\n";
            foreach (Vehiculo v in VehiculosEnCarretera)
            {
                strVehs += $"\tID: {v.Id} - Pos: {v.Pos} - Dir: {v.Direccion} - Vel: {v.Velocidad} - Acabado: {v.Acabado}\n";
            }

            Console.WriteLine(strVehs);
        }

        // Permite serializar Carretera a array de bytes mediante formato XML
        public byte[] CarreteraABytes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            MemoryStream MS = new MemoryStream();
            serializer.Serialize(MS, this);
            return MS.ToArray();
        }

        // Permite deserializar una cadena de bytes a un objeto de tipo Carretera
        public static Carretera BytesACarretera(byte[] bytesCarrera)
        {
            Carretera tmpCarretera;
            XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
            MemoryStream MS = new MemoryStream(bytesCarrera);
            tmpCarretera = (Carretera)serializer.Deserialize(MS);
            return tmpCarretera;
        }
    }
}
