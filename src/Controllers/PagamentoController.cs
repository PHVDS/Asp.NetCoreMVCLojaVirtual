using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Cookie;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Texto;
using LojaVirtual.Models;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using PagarMe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class PagamentoController : BaseController
	{
		private readonly Cookie _cookie;
		private readonly GerenciarPagarMe _gerenciarPagarMe;

		public PagamentoController(
			LoginCliente loginCliente,
			IEnderecoEntregaRepository enderecoEntregaRepository,
			Cookie cookie,
			CookieFrete cookieValorPrazoFrete,
			CalcularPacote calcularPacote,
			WSCorreiosCalcularFrete wscorreios,
			IMapper mapper,
			CookieCarrinhoCompra cookieCarrinhoCompra,
			IProdutoRepository produtoRepository,
			GerenciarPagarMe gerenciarPagarMe)
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
			_gerenciarPagarMe = gerenciarPagarMe;
		}

		[HttpGet]
		[ClienteAutorizacao]
		public IActionResult Index()
		{
			var tipoFreteSelecionadoPeloUsuario = _cookie.Consultar("Carrinho.TipoFrete", false);

			if (tipoFreteSelecionadoPeloUsuario != null)
			{
				var enderecoEntrega = ObterEndereco();
				var carrinhoHash = GerarHash(_cookieCarrinhoCompra.Consultar());

				int cep = int.Parse(Mascara.Remover(enderecoEntrega.CEP));

				ViewBag.Frete = ObterFrete(cep.ToString());
				List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
				ViewBag.Produtos = produtoItemCompleto;

				return View("Index");
			}
			TempData["MSG_E"] = Mensagem.MSG_E009;
			return RedirectToAction("EnderecoEntrega", "CarrinhoCompra");
		}

		[HttpPost]
		[ClienteAutorizacao]
		public IActionResult Index([FromForm] CartaoCredito cartaoCredito)
		{
			if (ModelState.IsValid)
			{
				EnderecoEntrega enderecoEntrega = ObterEndereco();
				ValorPrazoFrete frete = ObterFrete(enderecoEntrega.CEP.ToString());
				List<ProdutoItem> produtos = CarregarProdutoDB();

				try
				{
					dynamic pagarMeResposta = _gerenciarPagarMe.GerarPagCartaoCredito(cartaoCredito, enderecoEntrega, frete, produtos);
					
					return new ContentResult() { Content = "Sucesso" + pagarMeResposta.TransactionId };
				}
				catch (PagarMeException e)
				{
					StringBuilder sb = new StringBuilder();

					if (e.Error.Errors.Count() > 0)
					{
						sb.Append("Erro no pagamento: ");
						foreach (var erro in e.Error.Errors)
						{
							sb.Append("- " + e.Message + "<br />");
						}
					}
					TempData["MSG_E"] = sb.ToString();

					return Index();
				}
				
			}
			else
			{
				return Index();
			}
		}

		private EnderecoEntrega ObterEndereco()
		{
			EnderecoEntrega enderecoEntrega = null;
			var enderecoEntregaId = int.Parse(_cookie.Consultar("Carrinho.Endereco", false).Replace("-end", ""));

			if (enderecoEntregaId == 0)
			{
				Cliente cliente = _loginCliente.GetCliente();
				enderecoEntrega = new EnderecoEntrega();
				enderecoEntrega.Nome = "Endereço do cliente";
				enderecoEntrega.Id = 0;
				enderecoEntrega.CEP = cliente.CEP;
				enderecoEntrega.Estado = cliente.Estado;
				enderecoEntrega.Cidade = cliente.Cidade;
				enderecoEntrega.Bairro = cliente.Bairro;
				enderecoEntrega.Endereco = cliente.Endereco;
				enderecoEntrega.Complemento = cliente.Complemento;
				enderecoEntrega.Numero = cliente.Numero;
			}
			else
			{
				var endereco = _enderecoEntregaRepository.ObterEnderecoEntrega(enderecoEntregaId);
			}

			return enderecoEntrega;
		}

		private ValorPrazoFrete ObterFrete(string cepDestino)
		{
			var tipoFreteSelecionadoPeloUsuario = _cookie.Consultar("Carrinho.TipoFrete", false);
			var carrinhoHash = GerarHash(_cookieCarrinhoCompra.Consultar());
			int cep = int.Parse(Mascara.Remover(cepDestino));

			Frete frete = _cookieFrete.Consultar().Where(a => a.CEP == cep && a.CodigoCarrinho == carrinhoHash).FirstOrDefault();

			if (frete != null)
			{
				return frete.ListaValores.Where(a => a.TipoFrete == tipoFreteSelecionadoPeloUsuario).FirstOrDefault();
			}
			return null;
		}
	}
}
