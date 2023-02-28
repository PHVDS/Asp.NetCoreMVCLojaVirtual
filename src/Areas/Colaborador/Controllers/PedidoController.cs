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

		public IActionResult NFE([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.NFE));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				string url = visualizarViewModel.NFE.NFE_URL;

				SalvarPedidoSituacao(id, url, PedidoSituacaoConstant.NF_EMITIDA);
				SalvarPedido(pedido, PedidoSituacaoConstant.NF_EMITIDA, url);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_NFE = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarRastreamento([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.CodigoRastreamento));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				string codRastreamento = visualizarViewModel.CodigoRastreamento.Codigo;

				SalvarPedidoSituacao(id, codRastreamento, PedidoSituacaoConstant.EM_TRANSPORTE);
				SalvarPedido(pedido, PedidoSituacaoConstant.EM_TRANSPORTE, null, codRastreamento);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_RASTREAMENTO = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarCancelamentoCartaoCredito([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.CartaoCredito));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				visualizarViewModel.CartaoCredito.FormaPagamento = MetodoPagamentoConstant.CartaoCredito;

				_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);

				SalvarPedidoSituacao(id, visualizarViewModel.CartaoCredito, PedidoSituacaoConstant.ESTORNO);
				SalvarPedido(pedido, PedidoSituacaoConstant.ESTORNO);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_CARTAOCREDITO = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarCancelamentoBoleto([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.Boleto));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				visualizarViewModel.Boleto.FormaPagamento = MetodoPagamentoConstant.Boleto;

				_gerenciarPagarMe.EstornoBoletoBancario(pedido.TransactionId, visualizarViewModel.Boleto);

				SalvarPedidoSituacao(id, visualizarViewModel.Boleto, PedidoSituacaoConstant.ESTORNO);
				SalvarPedido(pedido, PedidoSituacaoConstant.ESTORNO);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_BOLETOBANCARIO = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedido([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.Devolucao));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				SalvarPedidoSituacao(id, visualizarViewModel.Devolucao, PedidoSituacaoConstant.DEVOLVER);
				SalvarPedido(pedido, PedidoSituacaoConstant.DEVOLVER);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_DEVOLVER = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedidoRejeicao([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.DevolucaoMotivoRejeicao));

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				SalvarPedidoSituacao(id, visualizarViewModel.DevolucaoMotivoRejeicao, PedidoSituacaoConstant.DEVOLUCAO_REJEITADA);
				SalvarPedido(pedido, PedidoSituacaoConstant.DEVOLUCAO_REJEITADA);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_DEVOLVER_REJEITAR = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedidoAprovadoCartaoCredito(int id)
		{
			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (pedido.Situacao == PedidoSituacaoConstant.DEVOLVER_ENTREGUE)
			{
				_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);
				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLUCAO_ACEITA);
				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLVER_ESTORNO);
				SalvarPedido(pedido, PedidoSituacaoConstant.DEVOLVER_ESTORNO);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			VisualizarViewModel visualizarViewModel = new VisualizarViewModel();
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedidoAprovadoBoleto([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.Boleto), "Boleto.Motivo");

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (ModelState.IsValid)
			{
				visualizarViewModel.Boleto.FormaPagamento = MetodoPagamentoConstant.Boleto;
				_gerenciarPagarMe.EstornoBoletoBancario(pedido.TransactionId, visualizarViewModel.Boleto);
				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLUCAO_ACEITA);
				SalvarPedidoSituacao(id, visualizarViewModel.Boleto, PedidoSituacaoConstant.DEVOLVER_ESTORNO);
				SalvarPedido(pedido, PedidoSituacaoConstant.DEVOLVER_ESTORNO);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_DEVOLVER_BOLETOBANCARIO = true;
			visualizarViewModel.Pedido = pedido;
			return View(nameof(Visualizar), visualizarViewModel);
		}

		private void ValidarFormulario(string formularioParaValidar, params string[] removerFormularios)
		{
			//ModelState -> Validações
			var propriedades = new VisualizarViewModel().GetType().GetProperties();

			foreach (var propriedade in propriedades)
			{
				//Pedido != NFE = true -> Sai da validação
				//NFE != NFE = false -> Continua na validação
				if (propriedade.Name != formularioParaValidar)
				{
					ModelState.Remove(propriedade.Name);
				}
			}

			foreach (var removerFormulario in removerFormularios)
			{
				ModelState.Remove(removerFormulario);
			}
		}

		private void SalvarPedidoSituacao(int pedidoId, object dados, string situacao)
		{
			var pedidoSituacao = new PedidoSituacao
			{
				Data = DateTime.Now,
				Dados = JsonConvert.SerializeObject(dados),
				PedidoId = pedidoId,
				Situacao = situacao
			};

			_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
		}

		private void SalvarPedido(Pedido pedido, string situacao, string nfe = null, string codRastreamento = null)
		{
			pedido.Situacao = situacao;
			if (nfe != null)
			{
				pedido.NFE = nfe;
			}
			if (codRastreamento != null)
			{
				pedido.FreteCodRastreamento = codRastreamento;
			}

			_pedidoRepository.Atualizar(pedido);
		}
	}
}
