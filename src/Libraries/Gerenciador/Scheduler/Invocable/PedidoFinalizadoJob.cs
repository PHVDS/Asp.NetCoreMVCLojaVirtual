using Coravel.Invocable;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
	public class PedidoFinalizadoJob : IInvocable
	{
		private readonly IPedidoRepository _pedidoRepository;
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;
		private readonly IConfiguration _configuration;
		private readonly ILogger<PedidoFinalizadoJob> _logger;

		public PedidoFinalizadoJob(ILogger<PedidoFinalizadoJob> logger, IConfiguration configuration, IPedidoRepository pedidoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository)
		{
			_logger = logger;
			_pedidoRepository = pedidoRepository;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
			_configuration = configuration;
		}

		public Task Invoke()
		{
			_logger.LogInformation("> PedidoFinalizadoJob: Iniciando");

			var pedidos = _pedidoRepository.ObterTodosPedidosPorSituacao(PedidoSituacaoConstant.ENTREGUE);
			foreach (var pedido in pedidos)
			{
				var pedidoSituacaoDB = pedido.PedidoSituacoes.FirstOrDefault(a => a.Situacao == PedidoSituacaoConstant.ENTREGUE);

				if (pedidoSituacaoDB != null)
				{
					int tolerancia = _configuration.GetValue<int>("Finalizado:Days");

					if (DateTime.Now >= pedidoSituacaoDB.Data.AddDays(tolerancia))
					{
						PedidoSituacao pedidoSituacao = new PedidoSituacao
						{
							PedidoId = pedido.Id,
							Situacao = PedidoSituacaoConstant.FINALIZADO,
							Data = DateTime.Now,
							Dados = string.Empty
						};

						_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
						pedido.Situacao = PedidoSituacaoConstant.FINALIZADO;
						_pedidoRepository.Atualizar(pedido);
					}
				}
			}
			_logger.LogInformation("> PedidoFinalizadoJob: Finalizado");
			return Task.CompletedTask;
			
		}
	}
}
