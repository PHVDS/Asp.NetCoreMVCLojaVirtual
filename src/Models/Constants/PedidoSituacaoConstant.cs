using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.Constants
{
	public class PedidoSituacaoConstant
	{
		public const string AGUARDANDO_PAGAMENTO = "04014";
		public const string PAGAMENTO_APROVADO = "40215";
		public const string PAGAMENTO_REJEITADO = "04510";
		public const string NF_EMITIDA = "04510";
		public const string EM_TRANSPORTE = "04510";
		public const string ENTREGUE = "04510";
		public const string FINALIZADO = "04510";
		public const string EM_CANCELAMENTO = "04510";
		public const string EM_ANALISE = "04510";
		public const string cANCELAMENTO_ACEITO = "04510";
		public const string cANCELAMENTO_REJEITADO = "04510";
		public const string ESTORNO = "04510";

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
