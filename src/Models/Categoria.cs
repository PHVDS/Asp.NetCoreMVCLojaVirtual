using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models
{
	public class Categoria
	{
		[Display(Name = "Código")]
		public int Id { get; set; }

		[NomeCategoriaUnico(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E011")]
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[MinLength(4, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E002")]
		public string Nome { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[MinLength(4, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E002")]
		public string Slug { get; set; }

		[Display(Name ="Categoria Pai")]
		public int? CategoriaPaiId { get; set; }

		[ForeignKey("CategoriaPaiId")]
		public virtual Categoria CategoriaPai { get; set; }
	}
}
