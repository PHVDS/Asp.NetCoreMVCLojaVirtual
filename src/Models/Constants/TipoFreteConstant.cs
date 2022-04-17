using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.Constants
{
	public class TipoFreteConstant
	{
		public const string SEDEX = "04014";
		public const string SEDEX10 = "40215";
		public const string PAC = "04510";

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
