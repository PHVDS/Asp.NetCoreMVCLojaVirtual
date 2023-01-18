using LojaVirtual.Libraries.Lang;
using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Models
{
    public class NFE
    {
        [Url(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E004")]
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string NFE_URL { get; set; }
    }
}
