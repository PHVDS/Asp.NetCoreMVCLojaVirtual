using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models
{
	public class Frete
	{
		public int CEP { get; set; }
		public string CodigoCarrinho { get; set; }
		public List<ValorPrazoFrete> ListaValorPrazoFrete { get; set; }
	}
}
