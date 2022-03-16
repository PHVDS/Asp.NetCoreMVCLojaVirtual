using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	public class HomeController : Controller
	{
		private readonly IColaboradorRepository _repositoryColaborador;
		private readonly LoginColaborador _loginColaborador;

		public HomeController(IColaboradorRepository colaboradorRepository, LoginColaborador loginColaborador)
		{
			_repositoryColaborador = colaboradorRepository;
			_loginColaborador = loginColaborador;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login([FromForm] Models.Colaborador colaborador)
		{
			Models.Colaborador colaboradorDB = _repositoryColaborador.Login(colaborador.Email, colaborador.Senha);
			if (colaboradorDB != null)
			{
				_loginColaborador.Login(colaboradorDB);

				return new RedirectResult(Url.Action(nameof(Painel)));
			}
			else
			{
				ViewData["MSG_E"] = "Usuario não encontrado, verifique os campos digitados!";
				return View();
			}
		}

		[ColaboradorAutorizacao]
		public IActionResult Logout()
		{
			_loginColaborador.Logout();
			return RedirectToAction("Login","Home");
		}

		[HttpGet]
		public IActionResult RecuperarSenha()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CadastrarNovaSenha()
		{
			return View();
		}

		[ColaboradorAutorizacao]
		public IActionResult Painel()
		{
			return View();
		}
	}
}
