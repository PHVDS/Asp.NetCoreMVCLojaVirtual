using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Cookie;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Lang;
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
		private readonly Cookie _cookie;
		public PagamentoController(Cookie cookie, CookieValorPrazoFrete cookieValorPrazoFrete, CalcularPacote calcularPacote, WSCorreiosCalcularFrete wscorreios, IMapper mapper, CookieCarrinhoCompra cookieCarrinhoCompra, IProdutoRepository produtoRepository)
			: base(cookieValorPrazoFrete, calcularPacote, wscorreios, mapper, cookieCarrinhoCompra, produtoRepository)
		{
			_cookie = cookie;
		}

		public IActionResult Index()
		{
			var tipoFreteSelecionadoPeloUsuario = _cookie.Consultar("Carrinho.TipoFrete", false);

			if (tipoFreteSelecionadoPeloUsuario != null)
			{
				var frete = _cookieValorPrazoFrete.Consultar().Where(a => a.TipoFrete == tipoFreteSelecionadoPeloUsuario).FirstOrDefault();
				
				if (frete != null)
				{
					ViewBag.Frete = frete;
					List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
					return View(produtoItemCompleto);
				}
			}

			TempData["MSG_E"] = Mensagem.MSG_E009;
			return RedirectToAction("Index", "CarrinhoCompra");

		}
	}
}
