﻿using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Cliente.Controllers
{
	[Area("Cliente")]
	public class HomeController : Controller
	{
		private readonly LoginCliente _loginCliente;
		private readonly IClienteRepository _repositoryCliente;
		private readonly IEnderecoEntregaRepository _enderecoEntregaRepository;

		public HomeController(IEnderecoEntregaRepository enderecoEntregaRepository, LoginCliente loginCliente, IClienteRepository repositoryCliente)
		{
			_enderecoEntregaRepository = enderecoEntregaRepository;
			_loginCliente = loginCliente;
			_repositoryCliente = repositoryCliente;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login([FromForm]Models.Cliente cliente, string returnUrl = null)
		{
			Models.Cliente clienteDB = _repositoryCliente.Login(cliente.Email, cliente.Senha);
			if (clienteDB != null)
			{
				_loginCliente.Login(clienteDB);

				if (returnUrl == null)
				{
					//se der erro coloca RedirectToAction
					return new RedirectToActionResult("Index", "Home", new { area = "" });
				}
				else
				{
					return LocalRedirectPermanent(returnUrl);
				}
			}
			else
			{
				ViewData["MSG_E"] = "Usuario não encontrado, verifique o e-mail e senha se estão corretos!";
				return View();
			}
		}

		[HttpGet]
		public IActionResult Sair()
		{
			_loginCliente.Logout();

			return RedirectToAction("Index", "Home", new { area = "" });
		}

		[HttpGet]
		public IActionResult CadastroCliente()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CadastroCliente([FromForm]Models.Cliente cliente, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				_repositoryCliente.Cadastrar(cliente);
				_loginCliente.Login(cliente);

				TempData["MSG_S"] = "Cadastro feito com sucesso!";

				if (returnUrl == null)
				{
					return RedirectToAction("Index", "Home", new { area = "" });
				}
				else
				{
					return LocalRedirectPermanent(returnUrl);
				}
			}
			return View();
		}

		[HttpGet]
		public IActionResult CadastroEnderecoEntrega()
		{
			return View();
		}
		
		[HttpPost]
		public IActionResult CadastroEnderecoEntrega([FromForm] EnderecoEntrega enderecoEntrega, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				enderecoEntrega.ClienteId = _loginCliente.GetCliente().Id;
				_enderecoEntregaRepository.Cadastrar(enderecoEntrega);

				if (returnUrl == null)
				{

				}
				else
				{
					return LocalRedirectPermanent(returnUrl);
				}
			}
			return View();
		}
		
	}
}
