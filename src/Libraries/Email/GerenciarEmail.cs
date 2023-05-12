using LojaVirtual.Libraries.Seguranca;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace LojaVirtual.Libraries.Email
{
	public class GerenciarEmail
	{
		private readonly SmtpClient _smtp;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GerenciarEmail(IHttpContextAccessor httpContextAccessor, SmtpClient smtp, IConfiguration configuration)
		{ 
			_smtp = smtp;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

		public void EnviarContatoPorEmail(Contato contato)
		{
			//SMTP -> Servidor q vai enviar a mensagem.
			
			var corpoMsg = string.Format("<h2>Contato - LojaVirtual</h2>" +
				"<b>Nome: </b> {0} <br />" +
				"<b>Email: </b> {1} <br />" +
				"<b>Texto: </b> {2} <br />" +
				"<br /> E-mail enviado automaticamente do site LojaVirtual.",
				contato.Nome,
				contato.Email,
				contato.Texto
				);

			//MailMessage -> Construir a mensagem.
			MailMessage mensagem = new MailMessage();
			mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
			mensagem.To.Add("paulocraazy@gmail.com");
			mensagem.Subject = "Contato - LojaVirtual - E-mail: " + contato.Email;
			mensagem.Body = corpoMsg;
			mensagem.IsBodyHtml = true;

			//Enviar mensagem via SMTP
			_smtp.Send(mensagem);
		}

		public void EnviarSenhaParaColaboradorPorEmail(Colaborador colaborador)
		{
			var corpoMsg = string.Format("<h2>Colaborador - LojaVirtual</h2>" +
				"Sua senha é: " +
				"<h3>{0}</h3>", colaborador.Senha);

			//MailMessage -> Construir a mensagem.
			MailMessage mensagem = new MailMessage();
			mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
			mensagem.To.Add(colaborador.Email);
			mensagem.Subject = "Colaborador - LojaVirtual - E-mail: " + colaborador.Nome;
			mensagem.Body = corpoMsg;
			mensagem.IsBodyHtml = true;

			//Enviar mensagem via SMTP
			_smtp.Send(mensagem);
		}

		public void EnviarDadosDoPedido(Cliente cliente, Pedido pedido)
		{
			//SMTP -> Servidor q vai enviar a mensagem.

			var corpoMsg = string.Format(				
				"<h1>Pedido realizado com sucesso!</h1><br />" +
				"<h3> N° pedido: {0} </h3>" +
				"<br /> Faça o login em nossa loja virtual e acompanhe o andamento.",
				pedido.Id + "/" + pedido.TransactionId
			);

			//MailMessage -> Construir a mensagem.
			MailMessage mensagem = new MailMessage();
			mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
			mensagem.To.Add(cliente.Email);
			mensagem.Subject = "LojaVirtual - Pedido " + "(" + pedido.Id + "/" + pedido.TransactionId + ")";
			mensagem.Body = corpoMsg;
			mensagem.IsBodyHtml = true;

			//Enviar mensagem via SMTP
			_smtp.Send(mensagem);
		}

        public void EnviarLinkResetarSenha(dynamic usuario, string idCrip)
        {
			var request = _httpContextAccessor.HttpContext.Request;
			string url = "";

			if (usuario.GetType() == typeof(Cliente))
			{
				url = $"{request.Scheme}://{request.Host}/Cliente/Home/CriarSenha/{idCrip}";
			}
			else
			{
                url = $"{request.Scheme}://{request.Host}/Colaborador/Home/CriarSenha/{idCrip}";
            }

            var corpoMsg = string.Format(
                "<h1>Criar nova senha para {1}({2})!</h1><br />" +
                "Clique no link abaixo para criar uma nova senha!<br />" +
                "<a href='{0}' target='_blank'>{0}</a><br />",
				url,
				usuario.Nome,
				usuario.Email
            );

            //MailMessage -> Construir a mensagem.
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "LojaVirtual - Criar nova senha - " + usuario.Nome;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar mensagem via SMTP
            _smtp.Send(mensagem);
        }
    }
}
