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

		List<string> StatusTimeline1 = new List<string>()
		{
			PedidoSituacaoConstant.PEDIDO_REALIZADO,
			PedidoSituacaoConstant.PAGAMENTO_APROVADO,
			PedidoSituacaoConstant.NF_EMITIDA,
			PedidoSituacaoConstant.EM_TRANSPORTE,
			PedidoSituacaoConstant.ENTREGUE,
			PedidoSituacaoConstant.FINALIZADO
		};
		List<PedidoSituacaoStatus> TimeLine2 { get; set; }
		List<string> StatusTimeline2 = new List<string>()
		{
			PedidoSituacaoConstant.PAGAMENTO_NAO_REALIZADO
		};

		public PedidoSituacaoViewComponent()
		{
			TimeLine1 = new List<PedidoSituacaoStatus>();
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PEDIDO_REALIZADO, Concluido = false, Cor = "complete" });
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PAGAMENTO_APROVADO, Concluido = false, Cor = "complete" });
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.NF_EMITIDA, Concluido = false, Cor = "complete" });
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.EM_TRANSPORTE, Concluido = false, Cor = "complete" });
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.ENTREGUE, Concluido = false, Cor = "complete" });
			TimeLine1.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.FINALIZADO, Concluido = false, Cor = "complete" });

			TimeLine2 = new List<PedidoSituacaoStatus>();
			TimeLine2.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PEDIDO_REALIZADO, Concluido = false, Cor = "complete" });
			TimeLine2.Add(new PedidoSituacaoStatus() { Situacao = PedidoSituacaoConstant.PAGAMENTO_NAO_REALIZADO, Concluido = false, Cor = "complete-red" });
		}

		public async Task<IViewComponentResult> InvokeAsync(Pedido pedido)
		{
			List<PedidoSituacaoStatus> timeline = null;

			if (StatusTimeline1.Contains(pedido.Situacao))
			{
				timeline = TimeLine1;
			}
			if (StatusTimeline2.Contains(pedido.Situacao))
			{
				timeline = TimeLine2;
			}
			if (timeline != null)
			{
				foreach (var pedidoSituacao in pedido.PedidoSituacoes)
				{
					var pedidoSituacaoTimeline = TimeLine1.Where(a => a.Situacao == pedidoSituacao.Situacao).FirstOrDefault();
					pedidoSituacaoTimeline.Data = pedidoSituacao.Data;
					pedidoSituacaoTimeline.Concluido = true;
				}
			}
			return View(timeline);
		}
	}
}
