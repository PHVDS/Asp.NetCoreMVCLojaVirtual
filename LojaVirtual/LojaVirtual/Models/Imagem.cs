using System.ComponentModel.DataAnnotations.Schema;

namespace LojaVirtual.Models
{
	public class Imagem
	{
		public int Id { get; set; }
		public string Caminho { get; set; }

		public int ProdutoId { get; set; }

		[ForeignKey("ProdutoId")]
		public Produto Produto { get; set; }
	}
}