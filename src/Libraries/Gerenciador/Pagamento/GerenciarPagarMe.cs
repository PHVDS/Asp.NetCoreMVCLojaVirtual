using Microsoft.Extensions.Configuration;
using System;
using PagarMe;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;
using LojaVirtual.Libraries.Texto;

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
		/*
		public object GerarPagCartaoCredito(CartaoCredito cartao)
		{
			Cliente cliente = _loginCliente.GetCliente();

			PagarMeService.DefaultApiKey = _configuration.GetValue<String>("Pagamento:PagarMe:ApiKey");
			PagarMeService.DefaultEncryptionKey = _configuration.GetValue<String>("Pagamento:PagarMe:EncryptionKey");

			Card card = new Card
			{
				Number = cartao.NumeroCartao,
				HolderName = cartao.NomeNoCartao,
				ExpirationDate = cartao.Vencimento,
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

			Billing = new Billing
				{
					Name = "Morty",
					Address = new Address()
					{
						Country = "br",
						State = "sp",
						City = "Cotia",
						Neighborhood = "Rio Cotia",
						Street = "Rua Matrix",
						StreetNumber = "213",
						Zipcode = "04250000"
					}
				}
			};

			var Today = DateTime.Now;

			transaction.Shipping = new Shipping
			{
				Name = "Rick",
				Fee = 100,
				DeliveryDate = Today.AddDays(4).ToString("yyyy-MM-dd"),
				Expedited = false,
				Address = new Address()
				{
					Country = "br",
					State = "sp",
					City = "Cotia",
					Neighborhood = "Rio Cotia",
					Street = "Rua Matrix",
					StreetNumber = "213",
					Zipcode = "04250000"
				}
			};

			transaction.Item = new[]
			{
				new Item()
				{
					Id = "1",
					Title = "Little Car",
					Quantity = 1,
					Tangible = true,
					UnitPrice = 1000
				},
				new Item()
				{
					Id = "2",
					Title = "Baby Crib",
					Quantity = 1,
					Tangible = true,
					UnitPrice = 1000
				}
			};

			transaction.Save();
		}
		*/
	}
}
