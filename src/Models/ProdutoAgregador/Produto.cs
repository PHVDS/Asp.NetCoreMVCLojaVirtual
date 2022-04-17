﻿using LojaVirtual.Libraries.Lang;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaVirtual.Models.ProdutoAgregador
{
	public class Produto
	{
		public int Id { get; set; }

		[JsonIgnore]
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		public string Nome { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Display(Name ="Descrição")]
		[JsonIgnore]
		public string  Descricao { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Display(Name ="Preço")]
		[JsonIgnore]
		public decimal Valor { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Range(0, 100000, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E006")]
		[JsonIgnore]
		public int Quantidade { get; set; }

		//Frete - Correios
		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Range(0.001, 30, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E006")]
		[JsonIgnore]
		public double Peso { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Range(11, 105, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E006")]
		[JsonIgnore]
		public int Largura { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Range(2, 105, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E006")]
		[JsonIgnore]
		public int Altura { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Range(16, 105, ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E006")]
		[JsonIgnore]
		public int Comprimento { get; set; }

		[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
		[Display(Name ="Categoria")]
		[JsonIgnore]
		public int CategoriaId { get; set; }

		[ForeignKey("CategoriaId")]
		[JsonIgnore]
		public virtual Categoria Categoria { get; set; }

		[JsonIgnore]
		public virtual ICollection<Imagem> Imagens { get; set; }	
	}
}
