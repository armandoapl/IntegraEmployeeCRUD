using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prueba_integra.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El apellido es requerido")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; }


        [Required(ErrorMessage = "El número de telefono es requerido")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(12)]
        [MinLength(12)]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage ="El correo electronico es requerido")]
        public string Mail { get; set; }


        public string? PhotoFileName { get; set; }


        [NotMapped]
        public IFormFile? PhotoIformFile { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "La fecha de crontatación es requerido")]
        public DateTime ContractDate { get; set; }
    }
}
