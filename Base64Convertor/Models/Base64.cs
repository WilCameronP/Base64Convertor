using System.ComponentModel.DataAnnotations;

namespace Base64Convertor.Models
{
    public class Base64
    {
        [Required(ErrorMessage = "This field is required.")]
        public string Text { get; set; }

        [Display(Name = "File Type")]
        [Required(ErrorMessage = "This field is required.")]
        public string FileType { get; set; }

        public string OtherFileType { get; set; }
    }
}