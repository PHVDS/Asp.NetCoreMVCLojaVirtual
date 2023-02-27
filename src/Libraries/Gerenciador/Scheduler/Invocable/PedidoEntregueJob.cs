using Coravel.Invocable;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
	public class PedidoEntregueJob : IInvocable
	{
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;
		private readonly ILogger<PedidoEntregueJob> _logger;

		public PedidoEntregueJob(ILogger<PedidoEntregueJob> logger, IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_logger = logger;
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
		}
		
		public Task Invoke()
		{
			_logger.LogInformation("> PedidoEntregueJob: Iniciando");

			var pedidos =  _pedidoRepository.ObterTodosPedidosPorSituacao(PedidoSituacaoConstant.EM_TRANSPORTE);
			foreach (var pedido in pedidos)
			{
				var result = new Correios.NET.Services().GetPackageTracking(pedido.FreteCodRastreamento);

				if (result.IsDelivered)
				{
					PedidoSituacao pedidoSituacao = new PedidoSituacao
					{
						PedidoId = pedido.Id,
						Situacao = PedidoSituacaoConstant.ENTREGUE,
						Data = result.DeliveryDate.Value,
						Dados = JsonConvert.SerializeObject(result)
					};

					_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
					pedido.Situacao = PedidoSituacaoConstant.ENTREGUE;
					_pedidoRepository.Atualizar(pedido);
				}
			}
			_logger.LogInformation("> PedidoEntregueJob: Finalizado");
			return Task.CompletedTask;
		}
	}
}
