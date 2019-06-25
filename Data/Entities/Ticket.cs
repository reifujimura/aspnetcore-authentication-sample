using System.ComponentModel.DataAnnotations;

namespace AuthenticationSample.Data.Entites
{
    public class Ticket
    {
        [Key]
        public string Key { get; set; }
        public byte[] Value { get; set; }
    }
}