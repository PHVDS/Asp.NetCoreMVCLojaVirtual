using Coravel.Invocable;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
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

		public PedidoEntregueJob(IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
		}
		
		public Task Invoke()
		{
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

			return Task.CompletedTask;
		}
	}
}
