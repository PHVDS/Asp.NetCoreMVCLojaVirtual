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
using LojaVirtual.Models.ViewModels.Pagamento;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagarMe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	[ClienteAutorizacao]
	[ValidateCookiePagamentoController]
	public class PagamentoController : BaseController
	{
		private readonly Cookie _cookie;
		private readonly GerenciarPagarMe _gerenciarPagarMe;
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;

		public PagamentoController(
			IPedidoSituacaoRepository pedidoSituacaoRepository,
			IPedidoRepository pedidoRepository,
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
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
			_pedidoRepository = pedidoRepository;
			_cookie = cookie;
			_gerenciarPagarMe = gerenciarPagarMe;
		}

		[HttpGet]
		public IActionResult Index()
		{
			List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
			ValorPrazoFrete frete = ObterFrete();

			ViewBag.Frete = frete;
			ViewBag.Produtos = produtoItemCompleto;
			ViewBag.Parcelamentos = CalcularParcelamento(produtoItemCompleto);

			return View("Index");
		}

		[HttpPost]
		public IActionResult Index([FromForm] IndexViewModel indexViewModel)
		{
			if (ModelState.IsValid)
			{
				EnderecoEntrega enderecoEntrega = ObterEndereco();
				ValorPrazoFrete frete = ObterFrete();
				List<ProdutoItem> produtos = CarregarProdutoDB();
				Parcelamento parcela = BuscarParcelamento(produtos, indexViewModel.Parcelamento.Numero);

				try
				{
					Transaction transaction = _gerenciarPagarMe.GerarPagCartaoCredito(indexViewModel.CartaoCredito, parcela, enderecoEntrega, frete, produtos);

					SalvarPedido(produtos, transaction);

					return new ContentResult() { Content = "Sucesso! Cartão de Crédito" + transaction.Id };
				}
				catch (PagarMeException e)
				{

					TempData["MSG_E"] = MontarMensagensDeErro(e);

					return Index();
				}
			}
			else
			{
				return Index();
			}
		}

		private void SalvarPedido(List<ProdutoItem> produtos, Transaction transaction)
		{
			Pedido pedido = new Pedido
			{
				ClienteId = int.Parse(transaction.Customer.Id),
				TransactionId = transaction.Id,
				FreteEmpresa = "ECT - Correios",
				FormaPagamento = (transaction.PaymentMethod == 0) ? "Cartão de Crédito" : "Boleto",
				ValorTotal = ObterValorTotalCompra(produtos),
				DadosTransaction = transaction,
				DadosProdutos = produtos,
				DataRegistro = DateTime.Now,
				Situacao = "",
			};

			_pedidoRepository.Cadastrar(pedido);

			PedidoSituacao pedidoSituacao = new PedidoSituacao
			{
				PedidoId = pedido.Id,
				Data = DateTime.Now,
				Dados = new { Transaction = transaction, Produtos = produtos },
				Situacao = ""
			};

			_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
		}

		public IActionResult BoletoBancario()
		{
			EnderecoEntrega enderecoEntrega = ObterEndereco();
			ValorPrazoFrete frete = ObterFrete();
			List<ProdutoItem> produtos = CarregarProdutoDB();
			var valorTotal = ObterValorTotalCompra(produtos);

			try
			{
				Transaction transaction = _gerenciarPagarMe.GerarBoleto(valorTotal);
				return new ContentResult() { Content = "Sucesso! Boleto -" + transaction.Id };
				//return View("PedidoSucesso");
			}
			catch (PagarMeException e)
			{
				TempData["MSG_E"] = MontarMensagensDeErro(e);
				return RedirectToAction(nameof(Index));
			}
		}

		private EnderecoEntrega ObterEndereco()
		{
			EnderecoEntrega enderecoEntrega = null;
			var enderecoEntregaId = int.Parse(_cookie.Consultar("Carrinho.Endereco", false).Replace("-end", ""));

			if (enderecoEntregaId == 0)
			{
				Cliente cliente = _loginCliente.GetCliente();
				enderecoEntrega = _mapper.Map<EnderecoEntrega>(cliente);
			}
			else
			{
				_enderecoEntregaRepository.ObterEnderecoEntrega(enderecoEntregaId);
			}

			return enderecoEntrega;
		}

		private ValorPrazoFrete ObterFrete()
		{
			var enderecoEntrega = ObterEndereco();
			int cep = int.Parse(Mascara.Remover(enderecoEntrega.CEP));
			var tipoFreteSelecionadoPeloUsuario = _cookie.Consultar("Carrinho.TipoFrete", false);
			var carrinhoHash = GerarHash(_cookieCarrinhoCompra.Consultar());

			Frete frete = _cookieFrete.Consultar().Where(a => a.CEP == cep && a.CodigoCarrinho == carrinhoHash).FirstOrDefault();

			if (frete != null)
			{
				return frete.ListaValores.Where(a => a.TipoFrete == tipoFreteSelecionadoPeloUsuario).FirstOrDefault();
			}
			return null;
		}

		private decimal ObterValorTotalCompra(List<ProdutoItem> produtos)
		{
			ValorPrazoFrete frete = ObterFrete();
			decimal total = Convert.ToDecimal(frete.Valor);

			foreach (var produto in produtos)
			{
				total += produto.Valor;
			}
			return total;
		}

		private List<SelectListItem> CalcularParcelamento(List<ProdutoItem> produtos)
		{
			var total = ObterValorTotalCompra(produtos);
			var parcelamento = _gerenciarPagarMe.CalcularPagamentoParcelado(total);

			return parcelamento.Select(a => new SelectListItem(
				String.Format
				(
					"{0}x {1} {2} - TOTAL: {3}",
					a.Numero,
					a.ValorPorParcela.ToString("C"),
					a.Juros ? "c/ juros" : "s/ juros",
					a.Valor.ToString("C")
				),
					a.Numero.ToString()
			)).ToList();
		}

		private Parcelamento BuscarParcelamento(List<ProdutoItem> produtos, int numero)
		{
			return _gerenciarPagarMe.CalcularPagamentoParcelado(ObterValorTotalCompra(produtos))
					.Where(a => a.Numero == numero).First();
		}

		private string MontarMensagensDeErro(PagarMeException e)
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
			return sb.ToString();
		}
	}
}
