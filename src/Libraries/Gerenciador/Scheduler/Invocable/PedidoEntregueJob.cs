using Coravel.Invocable;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
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
				//var result = new Correios.NET.Services().GetPackageTracking(pedido.FreteCodRastreamento);

				if (true)
				{

				}
			}

			return Task.CompletedTask;
		}
	}
}
