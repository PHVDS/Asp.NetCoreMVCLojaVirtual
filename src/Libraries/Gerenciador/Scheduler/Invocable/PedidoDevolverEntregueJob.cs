using LojaVirtual.Models.Constants;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
    public class PedidoDevolverEntregueJob : IInvocable
    {
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;
		private readonly ILogger<PedidoDevolverEntregueJob> _logger;

		public PedidoDevolverEntregueJob(ILogger<PedidoDevolverEntregueJob> logger, IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_logger = logger;
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
		}

		public Task Invoke()
		{
			_logger.LogInformation("> PedidoDevolverEntregueJob: Iniciando");

			var pedidos = _pedidoRepository.ObterTodosPedidosPorSituacao(PedidoSituacaoConstant.DEVOLVER);
			foreach (var pedido in pedidos)
			{
				var result = new Correios.NET.Services().GetPackageTracking(pedido.FreteCodRastreamento);

				if (result.IsDelivered)
				{
					PedidoSituacao pedidoSituacao = new PedidoSituacao
					{
						PedidoId = pedido.Id,
						Situacao = PedidoSituacaoConstant.DEVOLVER_ENTREGUE,
						Data = result.DeliveryDate.Value,
						Dados = JsonConvert.SerializeObject(result)
					};

					_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
					pedido.Situacao = PedidoSituacaoConstant.DEVOLVER_ENTREGUE;
					_pedidoRepository.Atualizar(pedido);
				}
			}
			_logger.LogInformation("> PedidoDevolverEntregueJob: Finalizado");

			return Task.CompletedTask;
		}

	}
}
