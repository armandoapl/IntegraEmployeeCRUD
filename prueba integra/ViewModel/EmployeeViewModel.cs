using prueba_integra.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prueba_integra.ViewModel
{
    public class EmployeeViewModel
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public IFormFile PhotoFormFile { get; set; }
    }
}
