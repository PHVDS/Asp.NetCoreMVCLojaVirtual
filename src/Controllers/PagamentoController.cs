using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Cookie;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Texto;
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
	public class PagamentoController : BaseController
	{
		private readonly Cookie _cookie;
		public PagamentoController(
			LoginCliente loginCliente,
			IEnderecoEntregaRepository enderecoEntregaRepository,
			Cookie cookie,
			CookieFrete cookieValorPrazoFrete,
			CalcularPacote calcularPacote,
			WSCorreiosCalcularFrete wscorreios,
			IMapper mapper,
			CookieCarrinhoCompra cookieCarrinhoCompra,
			IProdutoRepository produtoRepository)
			: base(
				  loginCliente,
				  enderecoEntregaRepository,
				  cookieValorPrazoFrete,
				  calcularPacote,
				  wscorreios,
				  mapper,
				  cookieCarrinhoCompra,
				  produtoRepository)
		{
			_cookie = cookie;
		}

		[ClienteAutorizacao]
		public IActionResult Index()
		{
			var tipoFreteSelecionadoPeloUsuario = _cookie.Consultar("Carrinho.TipoFrete", false);

			if (tipoFreteSelecionadoPeloUsuario != null)
			{
				var enderecoEntregaId = int.Parse(_cookie.Consultar("Carrinho.Endereco", false).Replace("-end", ""));
				var cep = 0;

				if (enderecoEntregaId == 0)
				{
					cep = int.Parse(Mascara.Remover(_loginCliente.GetCliente().CEP));
				}
				else
				{
					var endereco = _enderecoEntregaRepository.ObterEnderecoEntrega(enderecoEntregaId);
					cep = int.Parse(Mascara.Remover(endereco.CEP));
				}
				var carrinhoHash = GerarHash(_cookieCarrinhoCompra.Consultar());
				
				Frete frete = _cookieFrete.Consultar().Where(a => a.CEP == cep && a.CodigoCarrinho == carrinhoHash).FirstOrDefault();
	
				if (frete != null)
				{
					ViewBag.Frete = frete.ListaValores.Where(a => a.TipoFrete == tipoFreteSelecionadoPeloUsuario).FirstOrDefault();
					List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
					
					return View(produtoItemCompleto);
				}
			}

			TempData["MSG_E"] = Mensagem.MSG_E009;
			return RedirectToAction("Index", "CarrinhoCompra");

		}
	}
}
