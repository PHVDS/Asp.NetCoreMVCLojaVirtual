﻿using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Models;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class CarrinhoCompraController : Controller
	{
		private readonly CarrinhoCompra _carrinhoCompra;
		private readonly IProdutoRepository _produtoRepository;
		public CarrinhoCompraController(CarrinhoCompra carrinhoCompra, IProdutoRepository produtoRepository)
		{
			_carrinhoCompra = carrinhoCompra;
			_produtoRepository = produtoRepository;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AdicionarItem(int id)
		{
			Produto produto = _produtoRepository.ObterProduto(id);
			if (produto == null)
			{

				return View("NaoExisteItem");
			}
			else
			{
				var item = new ProdutoItem() { Id = id, QuantidadeProdutoCarrinho = 1};
				_carrinhoCompra.Cadastrar(item);

				return RedirectToAction(nameof(Index));
			}
		}

		public IActionResult AlterarQuantidade(int id, int quantidade)
		{
			var item = new ProdutoItem() { Id = id, QuantidadeProdutoCarrinho = quantidade};
			_carrinhoCompra.Atualizar(item);
			return RedirectToAction(nameof(Index));
		}

		public IActionResult RemoverItem(int id)
		{
			_carrinhoCompra.Remover(new ProdutoItem() { Id = id });
			return RedirectToAction(nameof(Index));
		}
	}
}
