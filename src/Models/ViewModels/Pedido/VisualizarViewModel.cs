namespace LojaVirtual.Models.ViewModels.Pedido
{
    public class VisualizarViewModel
    {
        public Models.Pedido Pedido { get; set; }
        public NFE NFE { get; set; }
        public CodigoRastreamento CodigoRastreamento { get; set; }
        public DadosCancelamento CartaoCredito { get; set; }
        public DadosCancelamento Boleto { get; set; }
    }
}
