using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaVirtual.Models
{
	public class Produto
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string  Descricao { get; set; }
		public decimal Valor { get; set; }
		public int Quantidade { get; set; }

		//Frete - Correios
		public double Peso { get; set; }
		public int Largura { get; set; }
		public int Altura { get; set; }
		public int Comprimento { get; set; }

		public int CategoriaId { get; set; }

		[ForeignKey("CategoriaId")]
		public virtual Categoria Categoria { get; set; }

		public virtual ICollection<Imagem> Imagens { get; set; }	
	}
}
