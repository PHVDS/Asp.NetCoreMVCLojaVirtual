using Microsoft.Extensions.Configuration;
using System;
using PagarMe;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;
using LojaVirtual.Libraries.Texto;
using System.Collections.Generic;
using LojaVirtual.Models.ProdutoAgregador;

namespace LojaVirtual.Libraries.Gerenciador.Pagamento
{
	public class GerenciarPagarMe
	{
		private readonly LoginCliente _loginCliente;
		private readonly IConfiguration _configuration;
		public GerenciarPagarMe(IConfiguration configuration, LoginCliente loginCliente)
		{
			_configuration = configuration;
			_loginCliente = loginCliente;
		}

		public object GerarBoleto(decimal valor)
		{
			try
			{
				Cliente cliente = _loginCliente.GetCliente();

				PagarMeService.DefaultApiKey = _configuration.GetValue<String>("Pagamento:PagarMe:ApiKey");
				PagarMeService.DefaultEncryptionKey = _configuration.GetValue<String>("Pagamento:PagarMe:EncryptionKey");

				Transaction transaction = new Transaction
				{
					Amount = Convert.ToInt32(valor),
					PaymentMethod = PaymentMethod.Boleto,

					Customer = new Customer
					{
						ExternalId = cliente.Id.ToString(),
						Name = cliente.Nome,
						Type = CustomerType.Individual,
						Country = "br",
						Email = cliente.Email,
						Documents = new[]
						{
						new Document{
							Type = DocumentType.Cpf,
							Number = Mascara.Remover(cliente.CPF)
						},

					},
						PhoneNumbers = new string[]
						{
						"+55" + Mascara.Remover(cliente.Telefone)
						},

						Birthday = cliente.Nascimento.ToString("yyyy-MM-dd")
					}
				};

				transaction.Save();

				return new
				{
					BoletoUrl = transaction.BoletoUrl,
					BarCode = transaction.BoletoBarcode,
					Expiracao = transaction.BoletoExpirationDate
				};
			}
			catch (Exception e)
			{
				return new 
				{ 
					Erro = e.Message
				};
			}
		}
		
		public object GerarPagCartaoCredito(CartaoCredito cartao, EnderecoEntrega enderecoEntrega, ValorPrazoFrete valorFrete, List<ProdutoItem> produtos)
		{
			Cliente cliente = _loginCliente.GetCliente();

			PagarMeService.DefaultApiKey = _configuration.GetValue<String>("Pagamento:PagarMe:ApiKey");
			PagarMeService.DefaultEncryptionKey = _configuration.GetValue<String>("Pagamento:PagarMe:EncryptionKey");

			Card card = new Card
			{
				Number = cartao.NumeroCartao,
				HolderName = cartao.NomeNoCartao,
				ExpirationDate = cartao.VencimentoMM + cartao.VencimentoYY,
				Cvv = cartao.CodigoSeguranca
			};

			card.Save();

			Transaction transaction = new Transaction
			{
				Amount = 2100,
				Card = new Card
				{
					Id = card.Id
				},

				Customer = new Customer
				{
					ExternalId = cliente.Id.ToString(),
					Name = cliente.Nome,
					Type = CustomerType.Individual,
					Country = "br",
					Email = cliente.Email,
					Documents = new[]
						{
						new Document{
							Type = DocumentType.Cpf,
							Number = Mascara.Remover(cliente.CPF)
						},

					},
					PhoneNumbers = new string[]
						{
							"+55" + Mascara.Remover(cliente.Telefone)
						},

					Birthday = cliente.Nascimento.ToString("yyyy-MM-dd")
				}
			};

			transaction.Billing = new Billing
			{
				Name = cliente.Nome,
				Address = new Address()
				{
					Country = "br",
					State = cliente.Estado,
					City = cliente.Cidade,
					Neighborhood = cliente.Bairro,
					Street = cliente.Endereco + " " + cliente.Complemento,
					StreetNumber = cliente.Numero,
					Zipcode = Mascara.Remover(cliente.CEP)
				}
			};

			var Today = DateTime.Now;

			transaction.Shipping = new Shipping
			{
				Name = enderecoEntrega.Nome,
				Fee = Convert.ToInt32(valorFrete.Valor),
				DeliveryDate = Today.AddDays(_configuration.GetValue<int>("Frete:DiasNaEmpresa")).AddDays(valorFrete.Prazo).ToString("yyyy-MM-dd"),
				Expedited = false,
				Address = new Address()
				{
					Country = "br",
					State = enderecoEntrega.Estado,
					City = enderecoEntrega.Cidade,
					Neighborhood = enderecoEntrega.Bairro,
					Street = enderecoEntrega.Endereco + " " + enderecoEntrega.Complemento,
					StreetNumber = enderecoEntrega.Numero,
					Zipcode = enderecoEntrega.CEP
				}
			};

			Item[] itens = new Item[produtos.Count];
			for(var i = 0; i < produtos.Count; i++)
			{
				var item = produtos[i];
				var itemA = new Item()
				{
					Id = item.Id.ToString(),
					Title = item.Nome,
					Quantity = item.QuantidadeProdutoCarrinho,
					Tangible = true,
					UnitPrice = Convert.ToInt32(item.Valor),
				};

				itens[i] = itemA;
			}
			transaction.Item = itens;
			transaction.Save();

			return new { TransactionId = transaction.Id };
		}
	}
}
 