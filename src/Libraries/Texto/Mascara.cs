using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Texto
{
	public class Mascara
	{
		public static string Remover(string valor)
		{
			return valor.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace("R$","").Replace(",","").Replace(" ","");
		}

		public static int ConverterValorPagarMe(decimal valor)
		{
			string valorString = valor.ToString("C");
			valorString = Remover(valorString);

			int valorInt = int.Parse(valorString);

			return valorInt;
		}

		public static decimal ConverterPagarMeIntToDecimal(int valor)
		{
			string valorPagarMeString =  valor.ToString();
			string valorDecimalString = valorPagarMeString.Substring(0, valorPagarMeString.Length - 2) + "," + valorPagarMeString.Substring(valorPagarMeString.Length - 2);

			var dec = decimal.Parse(valorDecimalString);

			return dec;
		}

		public static int ExtrairCodigoPedido(string codigoPedido, out string transactionId)
		{
			string[] resultadoSepracao = codigoPedido.Split("-");

			transactionId = resultadoSepracao[1];

			return int.Parse(resultadoSepracao[0]);
		}
	}
}
