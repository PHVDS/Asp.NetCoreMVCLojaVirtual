using LojaVirtual.Libraries.Lang;
using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Models
{
    public class DadosCancelamento
    {
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string Motivo { get; set; }
        public string FormaPagamento { get; set; }
        public string BancoCodigo { get; set; }
        public string Agencia { get; set; }
        public string AgenciaDV { get; set; }
        public string Conta { get; set; }
        public string ContaDV { get; set; }
        public string CPF { get; set; }
		
        [MinLength(5, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E002")]
		public string Nome { get; set; }
        
        public string TipoConta { get; set; }
    }
}
