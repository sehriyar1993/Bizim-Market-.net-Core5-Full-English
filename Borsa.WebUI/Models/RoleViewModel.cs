using System.ComponentModel.DataAnnotations;

namespace Borsa.WebUI.Models
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Rol Adı Daxil Edin")]
        public string Name { get; set; }
    }
}
