using LojaVirtual.Models.ProdutoAgregador;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaVirtual.Models
{
	public class Imagem
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		public string Caminho { get; set; }

		public int ProdutoId { get; set; }

		[ForeignKey("ProdutoId")]
		public virtual Produto Produto { get; set; }
	}
}