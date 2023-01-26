using LojaVirtual.Libraries.Lang;
using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Models
{
    public class DadosCancelamentoCartao
    {
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string Motivo { get; set; }
		public string FormaPagamento { get; set; }
	}
}
