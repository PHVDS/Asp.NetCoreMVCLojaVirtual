using LojaVirtual.Models.Constants;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Coravel.Invocable;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
    public class PedidoDevolverEntregueJob : IInvocable
    {
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;

		public PedidoDevolverEntregueJob(IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
		}

		public Task Invoke()
		{
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

			return Task.CompletedTask;
		}

	}
}
