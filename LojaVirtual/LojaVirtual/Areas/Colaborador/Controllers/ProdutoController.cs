using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	public class ProdutoController : Controller
	{
		private readonly IProdutoRepository _produtoRepository;
		public ProdutoController(IProdutoRepository produtoRepository)
		{
			_produtoRepository = produtoRepository;
		}

		public IActionResult Index(int? pagina, string pesquisa)
		{
			var produtos = _produtoRepository.ObterTodosProdutos(pagina, pesquisa);
			return View(produtos);
		}
	}
}
