using LojaVirtual.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class ProdutoController : Controller
	{
		public ActionResult Visualizar() 
		{
			Produto produto = GetProduto();
			return View(produto);
		}

		private Produto GetProduto()
		{
			return new Produto()
			{ 
				Id = 1, Nome = "Xbox", Descricao = "Jogos 4k", Valor = 1000.00M
			};
		}
	}
}
