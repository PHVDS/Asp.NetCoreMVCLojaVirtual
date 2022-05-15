using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Cliente.Controllers
{
	[Area("Cliente")]
	[ClienteAutorizacao]
	public class ClienteController : Controller
	{
		private readonly LoginCliente _loginCliente;
		private readonly IClienteRepository _clienteRepository;

		public ClienteController(LoginCliente loginCliente, IClienteRepository clienteRepository)
		{
			_loginCliente = loginCliente;
			_clienteRepository = clienteRepository;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Atualizar()
		{
			Models.Cliente cliente = _clienteRepository.ObterCliente(_loginCliente.GetCliente().Id);

			return View(cliente);
		}

		[HttpPost]
		public IActionResult Atualizar(Models.Cliente cliente)
		{
			ModelState.Remove("Senha");
			ModelState.Remove("ConfirmacaoSenha");

			if (ModelState.IsValid)
			{
				cliente.Senha = _loginCliente.GetCliente().Senha;
				_clienteRepository.Atualizar(cliente);

				_loginCliente.Login(cliente);

				TempData["MSG_S"] = Mensagem.MSG_S001;

				return RedirectToAction(nameof(Index));

			}
			return View();
		}

		[HttpGet]
		public IActionResult AtualizarSenha()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AtualizarSenha(Models.Cliente cliente)
		{
			ModelState.Remove("Nome");
			ModelState.Remove("Nascimento");
			ModelState.Remove("Sexo");
			ModelState.Remove("CPF");
			ModelState.Remove("Telefone");
			ModelState.Remove("Email");
			ModelState.Remove("CEP");
			ModelState.Remove("Estado");
			ModelState.Remove("Cidade");
			ModelState.Remove("Bairro");
			ModelState.Remove("Endereco");
			ModelState.Remove("Complemento");
			ModelState.Remove("Numero");
			ModelState.Remove("Situacao");

			if (ModelState.IsValid)
			{
				Models.Cliente clienteDB = _clienteRepository.ObterCliente(_loginCliente.GetCliente().Id);
				clienteDB.Senha = _loginCliente.GetCliente().Senha;
				_clienteRepository.Atualizar(clienteDB);

				_loginCliente.Login(clienteDB);

				TempData["MSG_S"] = Mensagem.MSG_S001;

				return RedirectToAction(nameof(Index));
			}
			return View();
		}
	}
}
