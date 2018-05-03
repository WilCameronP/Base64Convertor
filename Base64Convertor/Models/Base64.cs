using System.ComponentModel.DataAnnotations;

namespace Base64Convertor.Models
{
    public class Base64
    {
        [Required(ErrorMessage = "This field is required.")]
        public string Text { get; set; }

        [Display(Name = "MIME Type")]
        [Required(ErrorMessage = "This field is required.")]
        public string MIMEType { get; set; }

        public string OtherMIMEType { get; set; }
    }
}