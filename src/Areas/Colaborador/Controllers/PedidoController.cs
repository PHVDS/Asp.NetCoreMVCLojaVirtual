using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
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

		public PedidoController(GerenciarPagarMe gerenciarPagarMe, IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_gerenciarPagarMe = gerenciarPagarMe;
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
		}

		public IActionResult Index(int? pagina, string codigoPedido, string cpf)
		{
			var pedidos = _pedidoRepository.ObterTodosPedidos(pagina, codigoPedido, cpf);

			return View(pedidos);
		}

		public IActionResult Visualizar(int id)
		{
			Pedido pedido = _pedidoRepository.ObterPedido(id);

			return View(pedido);
		}

		public IActionResult NFE(int id)
		{
			string url = HttpContext.Request.Form["nfe_url"];

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

			return RedirectToAction(nameof(Visualizar), new { id = id });
		}

		public IActionResult RegistrarRastreamento(int id)
		{
			string codRastreamento = HttpContext.Request.Form["cod_rastreamento"];

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

			return RedirectToAction(nameof(Visualizar), new { id = id });
		}

		public IActionResult RegistrarCancelamentoCartaoCredito(int id)
		{
			string motivo = HttpContext.Request.Form["motivo"];

			Pedido pedido = _pedidoRepository.ObterPedido(id);

			_gerenciarPagarMe.EstornoCartaoCredito(pedido.TransactionId);
		}
	}
}
