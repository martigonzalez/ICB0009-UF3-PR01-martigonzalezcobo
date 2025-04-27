using System.Net.Sockets;

namespace Servidor
{
    public class Cliente
    {
        public required int Id { get; set; }
        public required NetworkStream NetworkStream { get; set; }
    }
}