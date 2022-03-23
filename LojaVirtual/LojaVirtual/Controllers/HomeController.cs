using LojaVirtual.Database;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class HomeController : Controller
	{
		private readonly LoginCliente _loginCliente;
		private readonly IClienteRepository _repositoryCliente;
		private readonly INewsletterRepository _repositoryNewsletter;
		private readonly GerenciarEmail _gerenciarEmail;
		
		public HomeController(LoginCliente loginCliente, IClienteRepository repositoryCliente, INewsletterRepository repositoryNewsletter, GerenciarEmail gerenciarEmail)
		{
			_loginCliente = loginCliente;
			_repositoryCliente = repositoryCliente;
			_repositoryNewsletter = repositoryNewsletter;
			_gerenciarEmail = gerenciarEmail;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Index([FromForm] NewsletterEmail newsletterEmail)
		{
			if (ModelState.IsValid)
			{
				_repositoryNewsletter.Cadastrar(newsletterEmail);

				TempData["MSG_S"] = "E-mail cadastrado! Fique atento, agora voçê vai receber promoções no seu e-mail!";

				return RedirectToAction(nameof(Index));
			}
			else
			{
				return View();
			}
		}
		public IActionResult Categoria()
		{
			return View();
		}
		public IActionResult Contato()
		{
			return View();
		}

		public IActionResult ContatoAcao()
		{
			try
			{
				Contato contato = new Contato();

				contato.Nome = HttpContext.Request.Form["nome"];
				contato.Email = HttpContext.Request.Form["email"];
				contato.Texto = HttpContext.Request.Form["texto"];

				var listaMensagens = new List<ValidationResult>();
				var contexto = new ValidationContext(contato);
				bool isValid = Validator.TryValidateObject(contato, contexto, listaMensagens, true);

				if (isValid)
				{
					_gerenciarEmail.EnviarContatoPorEmail(contato);
					ViewData["MSG_S"] = "Mensagem de contato enviada com sucesso!";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var texto in listaMensagens)
					{
						sb.Append(texto.ErrorMessage + "<br/>");
					}

					ViewData["MSG_E"] = sb.ToString();
					ViewData["CONTATO"] = contato;
				}
			}
			catch (Exception e)
			{

				ViewData["MSG_E"] = "Opps! Tivemos um erro, tente novamente mais tarde!";
			}
			
			return View("Contato");
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login([FromForm]Cliente cliente)
		{
			Cliente clienteDB = _repositoryCliente.Login(cliente.Email, cliente.Senha);
			if (clienteDB != null)
			{
				_loginCliente.Login(clienteDB);

				return new RedirectResult(Url.Action(nameof(Painel)));
			}
			else
			{
				ViewData["MSG_E"] = "Usuario não encontrado, verifique os campos digitados!";
				return View();
			}
		}

		[HttpGet]
		[ClienteAutorizacao]
		public IActionResult Painel()
		{
			return new ContentResult() { Content = "Aqui é o Painel" };	
		}

		[HttpGet]
		public IActionResult CadastroCliente()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CadastroCliente([FromForm] Cliente cliente)
		{
			if (ModelState.IsValid)
			{
				_repositoryCliente.Cadastrar(cliente);

				TempData["MSG_S"] = "Cadastro feito com sucesso!";

				return RedirectToAction(nameof(CadastroCliente));
			}
			return View();
		}

		public IActionResult CarrinhoCompras()
		{
			return View();
		}
	}
}
