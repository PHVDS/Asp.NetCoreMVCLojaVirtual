using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.Constants
{
	public class PedidoSituacaoConstant
	{
		public const string PEDIDO_REALIZADO = "Pedido Realizado";
		public const string PAGAMENTO_APROVADO = "Pagamento Aprovado";
		public const string PAGAMENTO_REJEITADO = "Pagamento Rejeitado";
		public const string PAGAMENTO_NAO_REALIZADO = "Pagamento Não Realizado";
		
		public const string NF_EMITIDA = "NF Emitida";
		public const string EM_TRANSPORTE = "Em Transporte";
		public const string ENTREGUE = "Entregue";
		public const string FINALIZADO = "Finalizado";
		public const string ESTORNO = "Estorno";
		
		public const string DEVOLVER = "Devolver (Em Transporte)";
		public const string DEVOLVER_ENTREGUE = "Devolver (Entregue)";

		public const string DEVOLUCAO_ACEITA = "Devolução Aceita";
		public const string DEVOLUCAO_REJEITADA = "Devolução Rejeitada";
		

		public static string ObterNomesConstant(string codigo)
		{
			foreach (var item in typeof(TipoFreteConstant).GetFields())
			{
				if ((string)item.GetValue(null) == codigo)
					return item.Name.ToString();
			}
			return "";
		}
	}
}
