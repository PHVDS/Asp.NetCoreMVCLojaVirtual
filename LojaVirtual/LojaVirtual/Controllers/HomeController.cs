using LojaVirtual.Libraries.Email;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
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

				ContatoEmail.EnviarContatoPorEmail(contato);
				ViewData["MSG_S"] = "Mensagem de contato enviada com sucesso!";
			}
			catch (Exception e)
			{

				ViewData["MSG_E"] = "Opps! Tivemos um erro, tente novamente mais tarde!";
			}
			
			return View("Contato");
		}

		public IActionResult Login()
		{
			return View();
		}

		public IActionResult CadastroCliente()
		{
			return View();
		}

		public IActionResult CarrinhoCompras()
		{
			return View();
		}
	}
}
