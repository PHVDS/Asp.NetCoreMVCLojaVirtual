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

		public Transaction GerarBoleto(decimal valor, List<ProdutoItem> produtos, EnderecoEntrega enderecoEntrega, ValorPrazoFrete valorFrete)
		{
			Cliente cliente = _loginCliente.GetCliente();

			PagarMeService.DefaultApiKey = _configuration.GetValue<String>("Pagamento:PagarMe:ApiKey");
			PagarMeService.DefaultEncryptionKey = _configuration.GetValue<String>("Pagamento:PagarMe:EncryptionKey");

			Transaction transaction = new Transaction
			{
				Amount = Mascara.ConverterValorPagarMe(valor),
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

			var Today = DateTime.Now;
			var fee = Convert.ToDecimal(valorFrete.Valor);

			transaction.Shipping = new Shipping
			{
				Name = enderecoEntrega.Nome,
				Fee = Mascara.ConverterValorPagarMe(fee),
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
					Zipcode = Mascara.Remover(enderecoEntrega.CEP)
				}
			};

			Item[] itens = new Item[produtos.Count];

			for (var i = 0; i < produtos.Count; i++)
			{
				var item = produtos[i];
				var itemA = new Item()
				{
					Id = item.Id.ToString(),
					Title = item.Nome,
					Quantity = item.QuantidadeProdutoCarrinho,
					Tangible = true,
					UnitPrice = Mascara.ConverterValorPagarMe(item.Valor)
				};
				itens[i] = itemA;
			}
			transaction.Item = itens;

			transaction.Save();
			transaction.Customer.Gender = (cliente.Sexo == "M") ? Gender.Male : Gender.Female;
			return transaction;
		}

		public Transaction GerarPagCartaoCredito(CartaoCredito cartao, Parcelamento parcelamento, EnderecoEntrega enderecoEntrega, ValorPrazoFrete valorFrete, List<ProdutoItem> produtos)
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
				PaymentMethod = PaymentMethod.CreditCard,

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
						}

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
			var fee = Convert.ToDecimal(valorFrete.Valor);
			
			transaction.Shipping = new Shipping
			{
				Name = enderecoEntrega.Nome,
				Fee = Mascara.ConverterValorPagarMe(fee),
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
					Zipcode = Mascara.Remover(enderecoEntrega.CEP)
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
					UnitPrice = Mascara.ConverterValorPagarMe(item.Valor)
				};
				itens[i] = itemA;
			}
			transaction.Item = itens;
			transaction.Amount = Mascara.ConverterValorPagarMe(parcelamento.Valor);
			transaction.Installments = parcelamento.Numero;

			transaction.Save();
			transaction.Customer.Gender = (cliente.Sexo == "M") ? Gender.Male : Gender.Female;
			return transaction;
		}

		public List<Parcelamento> CalcularPagamentoParcelado(decimal valor)
		{
			List<Parcelamento> listaParcelamento = new List<Parcelamento>();

			int maxParcelamento = _configuration.GetValue<int>("Pagamento:PagarMe:MaxParcelas");
			int parcelaPagaVendedor = _configuration.GetValue<int>("Pagamento:PagarMe:ParcelaPagaVendedor");
			decimal juros = _configuration.GetValue<decimal>("Pagamento:PagarMe:Juros");

			for (int quantidadeParcelas = 1; quantidadeParcelas <= maxParcelamento; quantidadeParcelas++)
			{
				Parcelamento parcelamento = new Parcelamento
				{
					Numero = quantidadeParcelas
				};

				if (quantidadeParcelas > parcelaPagaVendedor)
				{
					//Juros -> quantidadeParcelas = (4 - (3parcelas = parcelaPagaVendedor)) + 5%
					int quantidadeParcelasComJuros = quantidadeParcelas - parcelaPagaVendedor;
					decimal valorDoJuros =  valor * juros / 100;

					parcelamento.Valor = quantidadeParcelasComJuros * valorDoJuros + valor;
					parcelamento.ValorPorParcela = parcelamento.Valor / parcelamento.Numero;
					parcelamento.Juros = true;
				}
				else
				{
					parcelamento.Valor = valor;
					parcelamento.ValorPorParcela = parcelamento.Valor / parcelamento.Numero;
					parcelamento.Juros = false;
				}

				listaParcelamento.Add(parcelamento);
			}
			return listaParcelamento;
		}

		public Transaction ObterTransacao(string transactionId)
		{
			PagarMeService.DefaultApiKey = _configuration.GetValue<String>("Pagamento:PagarMe:ApiKey");

			return PagarMeService.GetDefaultService().Transactions.Find(transactionId);
		}
	}
}
 