using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.Constants
{
	public class PedidoSituacaoConstant
	{
		public const string AGUARDANDO_PAGAMENTO = "Aguardando Pagamento";
		public const string PAGAMENTO_APROVADO = "Pagamento Aprovado";
		public const string PAGAMENTO_REJEITADO = "Pagamento Rejeitado";
		public const string NF_EMITIDA = "NF Emitida";
		public const string EM_TRANSPORTE = "Em Transporte";
		public const string ENTREGUE = "Entregue";
		public const string FINALIZADO = "Finalizado";
		public const string EM_CANCELAMENTO = "Em Cancelamento";
		public const string EM_ANALISE = "Em Análise";
		public const string cANCELAMENTO_ACEITO = "Cancelamento Aceito";
		public const string cANCELAMENTO_REJEITADO = "Cancelamento Rejeitado";
		public const string ESTORNO = "Estorno";

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
