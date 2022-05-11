using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.AutoMapper;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Cookie;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Texto;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Models.ViewModels.Pagamento;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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

					Pedido pedido = ProcessarPedido(produtos, transaction);

					return new RedirectToActionResult("Index", "Pedido", new { id = pedido.Id });
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

		public IActionResult BoletoBancario()
		{
			EnderecoEntrega enderecoEntrega = ObterEndereco();
			ValorPrazoFrete frete = ObterFrete();
			List<ProdutoItem> produtos = CarregarProdutoDB();
			var valorTotal = ObterValorTotalCompra(produtos);

			try
			{
				Transaction transaction = _gerenciarPagarMe.GerarBoleto(valorTotal, produtos, enderecoEntrega, frete);
				Pedido pedido = ProcessarPedido(produtos, transaction);

				return new RedirectToActionResult("Index", "Pedido", new { id = pedido.Id });
			}
			catch (PagarMeException e)
			{
				TempData["MSG_E"] = MontarMensagensDeErro(e);
				return RedirectToAction(nameof(Index));
			}
		}

		private Pedido ProcessarPedido(List<ProdutoItem> produtos, Transaction transaction)
		{
			TransacaoPagarMe transacaoPagarMe;
			Pedido pedido;
			SalvarPedido(produtos, transaction, out transacaoPagarMe, out pedido);
			SalvarPedidoSituacao(produtos, transacaoPagarMe, pedido);
			DarBaixaNoEstoque(produtos);

			return pedido;
		}

		private void DarBaixaNoEstoque(List<ProdutoItem> produtos)
		{
			foreach (var produto in produtos)
			{
				Produto produtoDB = _produtoRepository.ObterProduto(produto.Id);
				produtoDB.Quantidade -= produto.QuantidadeProdutoCarrinho;

				_produtoRepository.Atualizar(produtoDB);
			}
		}

		private void SalvarPedidoSituacao(List<ProdutoItem> produtos, TransacaoPagarMe transacaoPagarMe, Pedido pedido)
		{
			TransactionProduto transactionProduto = new TransactionProduto { Transaction = transacaoPagarMe, Produtos = produtos };
			PedidoSituacao pedidoSituacao = _mapper.Map<Pedido, PedidoSituacao>(pedido);
			pedidoSituacao = _mapper.Map<TransactionProduto, PedidoSituacao>(transactionProduto, pedidoSituacao);
			pedidoSituacao.Situacao = PedidoSituacaoConstant.AGUARDANDO_PAGAMENTO;

			_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
		}

		private void SalvarPedido(List<ProdutoItem> produtos, Transaction transaction, out TransacaoPagarMe transacaoPagarMe, out Pedido pedido)
		{
			transacaoPagarMe = _mapper.Map<TransacaoPagarMe>(transaction);
			pedido = _mapper.Map<TransacaoPagarMe, Pedido>(transacaoPagarMe);
			pedido = _mapper.Map<List<ProdutoItem>, Pedido>(produtos, pedido);
			pedido.Situacao = PedidoSituacaoConstant.AGUARDANDO_PAGAMENTO;

			_pedidoRepository.Cadastrar(pedido);
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
