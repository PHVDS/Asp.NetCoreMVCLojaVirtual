using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	public class HomeController : Controller
	{
		private readonly IColaboradorRepository _repositoryColaborador;
		private readonly LoginColaborador _loginColaborador;
		private readonly IClienteRepository _clienteRepository;
		private readonly IProdutoRepository _produtoRepository;
		private readonly INewsletterRepository _newsletterRepository;
		private readonly IPedidoRepository _pedidoRepository;
		public HomeController(
			IPedidoRepository pedidoRepository, 
			IClienteRepository clienteRepository, 
			IProdutoRepository produtoRepository, 
			INewsletterRepository newsletterRepository, 
			IColaboradorRepository colaboradorRepository, 
			LoginColaborador loginColaborador
		)
		{
			_repositoryColaborador = colaboradorRepository;
			_loginColaborador = loginColaborador;
			_produtoRepository = produtoRepository;
			_newsletterRepository = newsletterRepository;
			_pedidoRepository = pedidoRepository;
			_clienteRepository = clienteRepository;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
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
		[ValidateHttpReferer]
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
			ViewBag.Clientes = _clienteRepository.QuantidadeTotalClientes();
			ViewBag.Newsletter = _newsletterRepository.QuantidadeTotalNewsletters();
			ViewBag.Produtos = _produtoRepository.QuantidadeTotalProdutos();
			ViewBag.NumeroPedidos = _pedidoRepository.QuantidadeTotalPedidos();
			ViewBag.ValorTotalPedidos = _pedidoRepository.ValorTotalPedidos();

			ViewBag.QuantidadeBoletoBancario = _pedidoRepository.QuantidadeTotalBoletoBancario();
			ViewBag.QuantidadeCartaoCredito = _pedidoRepository.QuantidadeTotalCartaoCredito();
			
			return View();
		}

		public IActionResult GerarCSVNewsletter()
		{
			var news = _newsletterRepository.ObterTodosNewsletter();

			StringBuilder sb = new StringBuilder();

			foreach (var email in news)
			{
				sb.AppendLine(email.Email);
			}

			byte[] buffer = Encoding.ASCII.GetBytes(sb.ToString());

			return File(buffer, "text/csv", $"newsletter.csv");
		}
	}
}
