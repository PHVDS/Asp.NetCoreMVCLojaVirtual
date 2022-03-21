using LojaVirtual.Libraries.Arquivo;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
		private readonly ICategoriaRepository _categoriaRepository;
		private readonly IImagemRepository _imagemRepository;
		public ProdutoController(IImagemRepository imagemRepository, IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
		{
			_imagemRepository = imagemRepository;
			_produtoRepository = produtoRepository;
			_categoriaRepository = categoriaRepository;
		}

		public IActionResult Index(int? pagina, string pesquisa)
		{
			var produtos = _produtoRepository.ObterTodosProdutos(pagina, pesquisa);
			return View(produtos);
		}

		[HttpGet]
		public IActionResult Cadastrar()
		{
			ViewBag.Categorias = _categoriaRepository.ObterTodasCategorias().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
			return View();
		}

		[HttpPost]
		public IActionResult Cadastrar(Produto produto)
		{
			if (ModelState.IsValid)
			{
				_produtoRepository.Cadastrar(produto);
				List<Imagem> ListaImagensDef = GerenciadorArquivo.MoverImagensProduto(new List<string>(Request.Form["imagem"]), produto.Id);
				_imagemRepository.CadastrarImagens(ListaImagensDef, produto.Id);

				TempData["MSG_S"] = Mensagem.MSG_S001;

				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Categorias = _categoriaRepository.ObterTodasCategorias().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
				produto.Imagens = new List<string>(Request.Form["imagem"]).Where(a => a.Trim().Length > 0).Select(a => new Imagem() { Caminho = a }).ToList();

				return View(produto);
			}
		}

		[HttpGet]
		public IActionResult Atualizar(int id)
		{
			ViewBag.Categorias = _categoriaRepository.ObterTodasCategorias().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
			Produto produto = _produtoRepository.ObterProduto(id);

			return View(produto);
		}

		[HttpPost]
		public IActionResult Atualizar(Produto produto, int id)
		{
			if (ModelState.IsValid)
			{
				_produtoRepository.Atualizar(produto);
				List<Imagem> ListaImagensDef = GerenciadorArquivo.MoverImagensProduto(new List<string>(Request.Form["imagem"]), produto.Id);

				_imagemRepository.ExcluirImagensDoProduto(produto.Id);
				_imagemRepository.CadastrarImagens(ListaImagensDef, produto.Id);

				TempData["MSG_S"] = Mensagem.MSG_S001;

				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Categorias = _categoriaRepository.ObterTodasCategorias().Select(a => new SelectListItem(a.Nome, a.Id.ToString()));
				produto.Imagens = new List<string>(Request.Form["imagem"]).Where(a => a.Trim().Length > 0).Select(a => new Imagem() { Caminho = a }).ToList();

				return View(produto);
			}
		}
	}
}
