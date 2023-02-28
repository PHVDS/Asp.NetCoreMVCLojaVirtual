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

			if (ModelState.IsValid)
			{
				string url = visualizarViewModel.NFE.NFE_URL;

				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.NFE = url;
				pedido.Situacao = PedidoSituacaoConstant.NF_EMITIDA;

				SalvarPedidoSituacao(id, url, PedidoSituacaoConstant.NF_EMITIDA);

				_pedidoRepository.Atualizar(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_NFE = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarRastreamento([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.CodigoRastreamento));

			if (ModelState.IsValid)
			{
				string codRastreamento = visualizarViewModel.CodigoRastreamento.Codigo;

				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.FreteCodRastreamento = codRastreamento;
				pedido.Situacao = PedidoSituacaoConstant.EM_TRANSPORTE;

				SalvarPedidoSituacao(id, codRastreamento, PedidoSituacaoConstant.EM_TRANSPORTE);

				_pedidoRepository.Atualizar(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_RASTREAMENTO = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarCancelamentoCartaoCredito([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.CartaoCredito));

			if (ModelState.IsValid)
			{
				visualizarViewModel.CartaoCredito.FormaPagamento = MetodoPagamentoConstant.CartaoCredito;

				Pedido pedido = _pedidoRepository.ObterPedido(id);

				_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);

				pedido.Situacao = PedidoSituacaoConstant.ESTORNO;

				SalvarPedidoSituacao(id, visualizarViewModel.CartaoCredito, PedidoSituacaoConstant.ESTORNO);

				_pedidoRepository.Atualizar(pedido);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_CARTAOCREDITO = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarCancelamentoBoleto([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.Boleto));

			if (ModelState.IsValid)
			{
				visualizarViewModel.Boleto.FormaPagamento = MetodoPagamentoConstant.Boleto;

				Pedido pedido = _pedidoRepository.ObterPedido(id);

				_gerenciarPagarMe.EstornoBoletoBancario(pedido.TransactionId, visualizarViewModel.Boleto);

				pedido.Situacao = PedidoSituacaoConstant.ESTORNO;

				SalvarPedidoSituacao(id, visualizarViewModel.Boleto, PedidoSituacaoConstant.ESTORNO);

				_pedidoRepository.Atualizar(pedido);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_BOLETOBANCARIO = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedido([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.Devolucao));

			if (ModelState.IsValid)
			{
				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.Situacao = PedidoSituacaoConstant.DEVOLVER;

				SalvarPedidoSituacao(id, visualizarViewModel.Devolucao, PedidoSituacaoConstant.DEVOLVER);

				_pedidoRepository.Atualizar(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_DEVOLVER = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedidoRejeicao([FromForm] VisualizarViewModel visualizarViewModel, int id)
		{
			ValidarFormulario(nameof(visualizarViewModel.DevolucaoMotivoRejeicao));


			if (ModelState.IsValid)
			{
				Pedido pedido = _pedidoRepository.ObterPedido(id);
				pedido.Situacao = PedidoSituacaoConstant.DEVOLUCAO_REJEITADA;

				SalvarPedidoSituacao(id, visualizarViewModel.DevolucaoMotivoRejeicao, PedidoSituacaoConstant.DEVOLUCAO_REJEITADA);

				_pedidoRepository.Atualizar(pedido);

				return RedirectToAction(nameof(Visualizar), new { id = id });
			}
			ViewBag.MODAL_DEVOLVER_REJEITAR = true;
			visualizarViewModel.Pedido = _pedidoRepository.ObterPedido(id);
			return View(nameof(Visualizar), visualizarViewModel);
		}

		public IActionResult RegistrarDevolucaoPedidoAprovadoCartaoCredito(int id)
		{
			Pedido pedido = _pedidoRepository.ObterPedido(id);

			if (pedido.Situacao == PedidoSituacaoConstant.DEVOLVER_ENTREGUE)
			{
				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLUCAO_ACEITA);

				_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);

				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLVER_ESTORNO);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

				pedido.Situacao = PedidoSituacaoConstant.DEVOLVER_ESTORNO;
				_pedidoRepository.Atualizar(pedido);

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
				SalvarPedidoSituacao(id, "", PedidoSituacaoConstant.DEVOLUCAO_ACEITA);

				visualizarViewModel.Boleto.FormaPagamento = MetodoPagamentoConstant.Boleto;

				_gerenciarPagarMe.EstornoBoletoBancario(pedido.TransactionId, visualizarViewModel.Boleto);

				SalvarPedidoSituacao(id, visualizarViewModel.Boleto, PedidoSituacaoConstant.DEVOLVER_ESTORNO);

				pedido.Situacao = PedidoSituacaoConstant.DEVOLVER_ESTORNO;
				_pedidoRepository.Atualizar(pedido);

				_produtoRepository.DevolverProdutoAoEstoque(pedido);

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
	}
}
