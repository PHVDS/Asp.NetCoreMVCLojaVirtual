using LojaVirtual.Libraries.Lang;
using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Models.ViewModels.Pedido
{
    public class VisualizarViewModel
    {
        public Models.Pedido Pedido { get; set; }
        public NFE NFE { get; set; }
        public CodigoRastreamento CodigoRastreamento { get; set; }
        public DadosCancelamentoCartao CartaoCredito { get; set; }
        public DadosCancelamentoBoleto Boleto { get; set; }
        public DadosDevolucao Devolucao { get; set; }

        [Display(Name = "Motivo da Rejeição")]
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string DevolucaoMotivoRejeicao { get; set; }
    }
}
