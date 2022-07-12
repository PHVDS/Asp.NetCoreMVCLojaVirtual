using AutoMapper;
using Coravel.Invocable;
using LojaVirtual.Libraries.Gerenciador.Pagamento;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
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
		public PedidoPagamentoSituacao(IPedidoSituacaoRepository pedidoSituacaoRepository, IMapper mapper, IConfiguration configuration, IPedidoRepository pedidoRepository, GerenciarPagarMe gerenciarPagarMe)
		{
			_pedidoSituacaoRepository = pedidoSituacaoRepository;
			_mapper = mapper;
			_configuration = configuration;
			_pedidoRepository = pedidoRepository;
			_gerenciarPagarMe = gerenciarPagarMe;
		}

		public Task Invoke()
		{
			var pedidosRealizados = _pedidoRepository.ObterTodosPedidosRealizados();

			foreach (var pedido in pedidosRealizados)
			{
				string situacao = null;
				var transaction = _gerenciarPagarMe.ObterTransacao(pedido.TransactionId);

				int toleranciaDias = _configuration.GetValue<int>("Pagamento:PagarMe:BoletoDiaExpiracao") + _configuration.GetValue<int>("Pagamento:PagarMe:BoletoDiaToleranciaVencido");
				
				if (transaction.Status == TransactionStatus.WaitingPayment && transaction.PaymentMethod == PaymentMethod.Boleto && DateTime.Now > pedido.DataRegistro.AddDays(toleranciaDias))
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_NAO_REALIZADO;
				}

				if (transaction.Status == TransactionStatus.Refused)
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_REJEITADO;
				}

				if (transaction.Status == TransactionStatus.Authorized || transaction.Status == TransactionStatus.Paid)
				{
					situacao = PedidoSituacaoConstant.PAGAMENTO_APROVADO;
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


			Debug.WriteLine("--Executado--");

			return Task.CompletedTask;
		}
	}
}
