using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Libraries.Json.Resolver;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Models.ViewModels.Pedido;
using LojaVirtual.Repositories;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	[ColaboradorAutorizacao]
	public class PedidoController : Controller
	{
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;
		private readonly GerenciarPagarMe _gerenciarPagarMe;
		private readonly IProdutoRepository _produtoRepository;

		public PedidoController(IProdutoRepository produtoRepository, GerenciarPagarMe gerenciarPagarMe, IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_gerenciarPagarMe = gerenciarPagarMe;
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
			_produtoRepository = produtoRepository;
		}

		public IActionResult Index(int? pagina, string codigoPedido, string cpf)
		{
			var pedidos = _pedidoRepository.ObterTodosPedidos(pagina, codigoPedido, cpf);

			return View(pedidos);
		}

		public IActionResult Visualizar(int id)
		{
			Pedido pedido = _pedidoRepository.ObterPedido(id);

			var viewModel = new VisualizarViewModel()
			{
				Pedido = pedido
			};

			return View(viewModel);
		}

		public IActionResult NFE([FromForm]VisualizarViewModel visualizarViewModel, int id)
		{
			ModelState.Remove("Pedido");
			ModelState.Remove("CodigoRastreamento");
			ModelState.Remove("CartaoCredito");
			ModelState.Remove("Boleto");

			if (ModelState.IsValid)
			{
				string url = visualizarViewModel.NFE.NFE_URL;

				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.NFE = url;
				pedido.Situacao = PedidoSituacaoConstant.NF_EMITIDA;

				var pedidoSituacao = new PedidoSituacao
				{
					Data = DateTime.Now,
					Dados = url,
					PedidoId = id,
					Situacao = PedidoSituacaoConstant.NF_EMITIDA
				};

				_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);

				_pedidoRepository.Atualizar(pedido);
			}

			return RedirectToAction(nameof(Visualizar), new { id = id });
		}

		public IActionResult RegistrarRastreamento([FromForm]VisualizarViewModel visualizarViewModel, int id)
		{
			ModelState.Remove("Pedido");
			ModelState.Remove("NFE");
			ModelState.Remove("CartaoCredito");
			ModelState.Remove("Boleto");

			if (ModelState.IsValid)
			{
				string codRastreamento = visualizarViewModel.CodigoRastreamento.Codigo;

				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.FreteCodRastreamento = codRastreamento;
				pedido.Situacao = PedidoSituacaoConstant.EM_TRANSPORTE;

				var pedidoSituacao = new PedidoSituacao
				{
					Data = DateTime.Now,
					Dados = codRastreamento,
					PedidoId = id,
					Situacao = PedidoSituacaoConstant.EM_TRANSPORTE
				};

				_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);

				_pedidoRepository.Atualizar(pedido);
			}
			
			return RedirectToAction(nameof(Visualizar), new { id = id });
		}

		public IActionResult RegistrarCancelamentoCartaoCredito([FromForm]VisualizarViewModel visualizarViewModel, int id)
		{
			ModelState.Remove("Pedido");
			ModelState.Remove("NFE");
			ModelState.Remove("CartaoCredito");
			ModelState.Remove("Boleto");

			if (ModelState.IsValid)
			{
				visualizarViewModel.CartaoCredito.FormaPagamento = MetodoPagamentoConstant.CartaoCredito;

				Pedido pedido = _pedidoRepository.ObterPedido(id);

				_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);

				pedido.Situacao = PedidoSituacaoConstant.ESTORNO;

				var pedidoSituacao = new PedidoSituacao
				{
					Data = DateTime.Now,
					Dados = JsonConvert.SerializeObject(visualizarViewModel.CartaoCredito),
					PedidoId = id,
					Situacao = PedidoSituacaoConstant.ESTORNO
				};

				_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);

				_pedidoRepository.Atualizar(pedido);

				DevolverProdutosEstoque(pedido);
			}

			return RedirectToAction(nameof(Visualizar), new { id = id });
		}
		private void DevolverProdutosEstoque(Pedido pedido)
		{
			List<ProdutoItem> produtos = JsonConvert.DeserializeObject<List<ProdutoItem>>(pedido.DadosProdutos,
				new JsonSerializerSettings()
				{
					ContractResolver = new ProdutoItemResolver<List<ProdutoItem>>()
				});

			foreach (var produto in produtos)
			{
				Produto produtoDB = _produtoRepository.ObterProduto(produto.Id);
				produtoDB.Quantidade += produto.QuantidadeProdutoCarrinho;

				_produtoRepository.Atualizar(produtoDB);
			}
		}

	}
}
