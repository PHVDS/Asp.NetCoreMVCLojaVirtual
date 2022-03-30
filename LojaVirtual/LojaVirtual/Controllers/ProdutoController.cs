using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class ProdutoController : Controller
	{
		private readonly ICategoriaRepository _categoriaRepository;
		private readonly IProdutoRepository _produtoRepository;

		public ProdutoController(ICategoriaRepository categoriaRepository, IProdutoRepository produtoRepository)
		{
			_categoriaRepository = categoriaRepository;
			_produtoRepository = produtoRepository;
		}

		[HttpGet]
		[Route("/Produto/Categoria/{slug}")]
		public IActionResult ListagemCategoria(string slug)
		{
			Categoria CategoriaPrincipal = _categoriaRepository.ObterCategoria(slug);

			List<Categoria> lista = _categoriaRepository.ObterCategoriasRecursivas(CategoriaPrincipal).ToList();
			ViewBag.Categorias = lista;
			
			return View();
		}

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
