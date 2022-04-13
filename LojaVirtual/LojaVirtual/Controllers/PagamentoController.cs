using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class PagamentoController : BaseController
	{
		public PagamentoController(CookieValorPrazoFrete cookieValorPrazoFrete, CalcularPacote calcularPacote, WSCorreiosCalcularFrete wscorreios, IMapper mapper, CookieCarrinhoCompra cookieCarrinhoCompra, IProdutoRepository produtoRepository)
			: base(cookieValorPrazoFrete, calcularPacote, wscorreios, mapper, cookieCarrinhoCompra, produtoRepository)
		{ }

		public IActionResult Index()
		{
			List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
			return View(produtoItemCompleto);
		}
	}
}
