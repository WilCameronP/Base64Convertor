using System.ComponentModel.DataAnnotations;

namespace Base64Convertor.Models
{
    public class FileCache
    {
        public string Name { get; set; }

        public byte[] Contents { get; set; }

        public string Mime { get; set; }
    }
}