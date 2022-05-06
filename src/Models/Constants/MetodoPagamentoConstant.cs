using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.Constants
{
	public class MetodoPagamentoConstant
	{
		public const string CartaoCredito = "Cartão de Crédito";
		public const string Boleto = "Boleto Bancário";

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
