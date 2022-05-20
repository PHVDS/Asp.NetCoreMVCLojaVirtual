using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Component
{
	public class PedidoSituacaoViewComponent : ViewComponent
	{
		List<PedidoSituacaoStatus> TimeLine1 { get; set; }
		public PedidoSituacaoViewComponent()
		{
			TimeLine1 = new List<PedidoSituacaoStatus>
			{
				new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PEDIDO_REALIZADO, Concluido = false },
				new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PAGAMENTO_APROVADO, Concluido = false },					new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.NF_EMITIDA, Concluido = false },
				new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.EM_TRANSPORTE, Concluido = false },
				new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.ENTREGUE, Concluido = false },
				new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.FINALIZADO, Concluido = false }
			};
		}

		public async Task<IViewComponentResult> InvokeAsync(Pedido pedido)
		{
			List<PedidoSituacaoStatus> timeline = null;

			var listaStatusTimeline1 = new List<string>()
			{
				PedidoSituacaoConstant.PEDIDO_REALIZADO,
				PedidoSituacaoConstant.PAGAMENTO_APROVADO,
				PedidoSituacaoConstant.NF_EMITIDA,
				PedidoSituacaoConstant.EM_TRANSPORTE,
				PedidoSituacaoConstant.ENTREGUE,
				PedidoSituacaoConstant.FINALIZADO
			};

			if (listaStatusTimeline1.Contains(pedido.Situacao))
			{
				timeline = TimeLine1;

				foreach(var pedidoSituacao in pedido.PedidoSituacoes)	
				{
					var pedidoSituacaoTimeline = TimeLine1.Where(a=> a.Situacao == pedidoSituacao.Situacao).FirstOrDefault();
					pedidoSituacaoTimeline.Data = pedidoSituacao.Data;
					pedidoSituacaoTimeline.Concluido = true;
				}
			}
			return View(timeline);
		}
	}
}
