using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Seguranca;
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
        private readonly GerenciarEmail _gerenciarEmail;
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
			LoginColaborador loginColaborador,
            GerenciarEmail gerenciarEmail
        )
		{
			_repositoryColaborador = colaboradorRepository;
			_loginColaborador = loginColaborador;
			_produtoRepository = produtoRepository;
			_newsletterRepository = newsletterRepository;
			_pedidoRepository = pedidoRepository;
			_clienteRepository = clienteRepository;
            _gerenciarEmail = gerenciarEmail;

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
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar([FromForm] Models.Colaborador colaborador)
        {
            var colaboradorDoBancoDados = _repositoryColaborador.ObterColaboradorPorEmail(colaborador.Email);

            if (colaboradorDoBancoDados != null && colaboradorDoBancoDados.Count > 0)
            {
                string idCrip = Base64Cipher.Base64Encode(colaboradorDoBancoDados.First().Id.ToString());
                _gerenciarEmail.EnviarLinkResetarSenha(colaboradorDoBancoDados.First(), idCrip);

                TempData["MSG_S"] = Mensagem.MSG_S005;

                ModelState.Clear();
            }
            else
            {
                TempData["MSG_E"] = Mensagem.MSG_E014;
            }

            return View();
        }

        [HttpGet]
        public IActionResult CriarSenha(string id)
        {
            try
            {
                var idColaboradorDeCrip = Base64Cipher.Base64Decode(id);

                if (!int.TryParse(idColaboradorDeCrip, out int idColaborador))
                {
                    TempData["MSG_E"] = Mensagem.MSG_E015;
                }
            }
            catch (System.FormatException e)
            {
                TempData["MSG_E"] = Mensagem.MSG_E015;
            }

            return View();
        }

        [HttpPost]
        public IActionResult CriarSenha([FromForm] Models.Colaborador colaborador, string id)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                try
                {
                    var idColaboradorDeCrip = Base64Cipher.Base64Decode(id);

                    if (!int.TryParse(idColaboradorDeCrip, out int idColaborador))
                    {
                        TempData["MSG_E"] = Mensagem.MSG_E015;
                        return View();
                    }
                    var colaboradorDB = _repositoryColaborador.ObterColaborador(idColaborador);
                    if (colaboradorDB != null)
                    {
                        colaboradorDB.Senha = colaborador.Senha;

                        _repositoryColaborador.AtualizarSenha(colaboradorDB);
                        TempData["MSG_S"] = Mensagem.MSG_S004;
                    }
                }
                catch (System.FormatException)
                {
                    TempData["MSG_E"] = Mensagem.MSG_E015;
                    return View();
                }
            }
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
