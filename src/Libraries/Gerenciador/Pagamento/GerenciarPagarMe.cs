using Microsoft.Extensions.Configuration;
using System;
using PagarMe;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;

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
							Number = cliente.CPF
						},

					},
						PhoneNumbers = new string[]
						{
						"+55" + cliente.Telefone,
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
	}
}
