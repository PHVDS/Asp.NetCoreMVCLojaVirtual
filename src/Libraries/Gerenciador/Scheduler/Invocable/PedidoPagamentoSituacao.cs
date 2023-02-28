using AutoMapper;
using Coravel.Invocable;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Libraries.Json.Resolver;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PagarMe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Gerenciador.Scheduler.Invocable
{
	public class PedidoPagamentoSituacao : IInvocable
	{
		private readonly IPedidoSituacaoRepository _pedidoSituacaoRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly IPedidoRepository _pedidoRepository;
		private readonly GerenciarPagarMe _gerenciarPagarMe;
		private readonly IProdutoRepository _produtoRepository;
		private readonly ILogger<PedidoPagamentoSituacao> _logger;
		public PedidoPagamentoSituacao(ILogger<PedidoPagamentoSituacao> logger, IProdutoRepository produtoRepository, IPedidoSituacaoRepository pedidoSituacaoRepository, IMapper mapper, IConfiguration configuration, IPedidoRepository pedidoRepository, GerenciarPagarMe gerenciarPagarMe)
		{
			_logger = logger;
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
			_mapper = mapper;
			_configuration = configuration;
			_pedidoRepository = pedidoRepository;
			_gerenciarPagarMe = gerenciarPagarMe;
			_produtoRepository = produtoRepository;
		}

		public Task Invoke()
		{
			_logger.LogInformation("> PedidoPagamentoSituacao: Iniciando");
			var pedidosRealizados = _pedidoRepository.ObterTodosPedidosPorSituacao(PedidoSituacaoConstant.PEDIDO_REALIZADO);

			foreach (var pedido in pedidosRealizados)
			{
				string situacao = null;
				var transaction = _gerenciarPagarMe.ObterTransacao(pedido.TransactionId);

				int toleranciaDias = _configuration.GetValue<int>("Pagamento:PagarMe:BoletoDiaExpiracao") + _configuration.GetValue<int>("Pagamento:PagarMe:BoletoDiaToleranciaVencido");
				
				if (transaction.Status == TransactionStatus.WaitingPayment && transaction.PaymentMethod == PaymentMethod.Boleto && DateTime.Now > pedido.DataRegistro.AddDays(toleranciaDias))
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_NAO_REALIZADO;
					_produtoRepository.DevolverProdutoAoEstoque(pedido);
				}

				if (transaction.Status == TransactionStatus.Refused)
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_REJEITADO;
					_produtoRepository.DevolverProdutoAoEstoque(pedido);

				}

				if (transaction.Status == TransactionStatus.Authorized || transaction.Status == TransactionStatus.Paid)
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_APROVADO;
				}

				if (transaction.Status == TransactionStatus.Refunded)
				{
					situacao = PedidoSituacaoConstant.ESTORNO;
					_produtoRepository.DevolverProdutoAoEstoque(pedido);
				}

				if (situacao != null)
				{
					TransacaoPagarMe transacaoPagarMe = _mapper.Map<Transaction, TransacaoPagarMe>(transaction);
					transacaoPagarMe.Customer.Gender = (pedido.Cliente.Sexo == "M") ? Gender.Male : Gender.Female;

					PedidoSituacao pedidoSituacao = new PedidoSituacao
					{
						PedidoId = pedido.Id,
						Situacao = situacao,
						Data = transaction.DateUpdated.Value,
						Dados = JsonConvert.SerializeObject(transacaoPagarMe)
					};

					_pedidoSituacaoRepository.Cadastrar(pedidoSituacao);
					pedido.Situacao = situacao;
					_pedidoRepository.Atualizar(pedido);
				}
			}
			_logger.LogInformation("> PedidoPagamentoSituacao: Finalizado");

			return Task.CompletedTask;
		}
	}
}
