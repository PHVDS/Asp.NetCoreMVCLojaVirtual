using LojaVirtual.Database;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;
using LojaVirtual.Models.ViewModels;
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
		private readonly INewsletterRepository _repositoryNewsletter;
		private readonly GerenciarEmail _gerenciarEmail;
		private readonly IProdutoRepository _produtoRepository;
		public HomeController(IProdutoRepository produtoRepository, INewsletterRepository repositoryNewsletter, GerenciarEmail gerenciarEmail)
		{
			_repositoryNewsletter = repositoryNewsletter;
			_gerenciarEmail = gerenciarEmail;
			_produtoRepository = produtoRepository;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Index([FromForm]NewsletterEmail newsletterEmail)
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
				Contato contato = new Contato
				{
					Nome = HttpContext.Request.Form["nome"],
					Email = HttpContext.Request.Form["email"],
					Texto = HttpContext.Request.Form["texto"]
				};

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
			catch (Exception)
			{

				ViewData["MSG_E"] = "Opps! Tivemos um erro, tente novamente mais tarde!";
			}
			
			return View("Contato");
		}
	}
}
