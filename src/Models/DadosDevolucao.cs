using LojaVirtual.Libraries.Lang;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LojaVirtual.Models
{
    public class DadosDevolucao
    {
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string Motivo { get; set; }


		[Display(Name = "Código de rastreamento")]
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[MinLength(5, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E002")]
		public string CodigoRastreamento { get; set; }
    }
}
