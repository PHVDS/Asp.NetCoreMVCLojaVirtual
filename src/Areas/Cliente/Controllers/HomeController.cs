using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Seguranca;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Areas.Cliente.Controllers
{
	[Area("Cliente")]
	public class HomeController : Controller
	{
		private readonly GerenciarEmail _gerenciarEmail; 
		private readonly LoginCliente _loginCliente;
		private readonly IClienteRepository _repositoryCliente;

        public HomeController(GerenciarEmail gerenciarEmail, LoginCliente loginCliente, IClienteRepository repositoryCliente)
		{
			_loginCliente = loginCliente;
			_repositoryCliente = repositoryCliente;
			_gerenciarEmail = gerenciarEmail;
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
                if (clienteDB.Situacao == SituacaoConstant.Desativado)
                {
                    ViewData["MSG_E"] = Mensagem.MSG_E017;
                    return View();
                }
                else
                {
                    _loginCliente.Login(clienteDB);

                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    else
                    {
                        return LocalRedirectPermanent(returnUrl);
                    }
                }
                
			}
			else
			{
				ViewData["MSG_E"] = Mensagem.MSG_E016;
				return View();
			}
		}

		[HttpGet]
		public IActionResult Recuperar()
		{
			return View();
		}

        [HttpPost]
        public IActionResult Recuperar([FromForm]Models.Cliente cliente)
		{
			ModelState.Remove("Nome");
			ModelState.Remove("Nascimento");
			ModelState.Remove("Sexo");
			ModelState.Remove("CPF");
			ModelState.Remove("Telefone");
			ModelState.Remove("CEP");
			ModelState.Remove("Estado");
			ModelState.Remove("Cidade");
			ModelState.Remove("Bairro");
			ModelState.Remove("Endereco");
			ModelState.Remove("Complemento");
			ModelState.Remove("Numero");
			ModelState.Remove("Senha");
			ModelState.Remove("ConfirmacaoSenha");

			if (ModelState.IsValid)
			{
				var clienteDoBancoDados = _repositoryCliente.ObterClientePorEmail(cliente.Email);

				if (clienteDoBancoDados != null)
				{
					string idCrip = Base64Cipher.Base64Encode(clienteDoBancoDados.Id.ToString());
                    _gerenciarEmail.EnviarLinkResetarSenha(clienteDoBancoDados, idCrip);
					
					TempData["MSG_S"] = Mensagem.MSG_S005;

					ModelState.Clear();
				}
				else
				{
					TempData["MSG_E"] = Mensagem.MSG_E014;
				}
			}
			return View();
		}

		[HttpGet]
		public IActionResult CriarSenha(string id)
		{
			try
			{
                var idClienteDeCrip = Base64Cipher.Base64Decode(id);

                if (!int.TryParse(idClienteDeCrip, out int idCliente))
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
        public IActionResult CriarSenha([FromForm] Models.Cliente cliente, string id)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Nascimento");
            ModelState.Remove("Sexo");
            ModelState.Remove("CPF");
            ModelState.Remove("Email");
            ModelState.Remove("Telefone");
            ModelState.Remove("CEP");
            ModelState.Remove("Estado");
            ModelState.Remove("Cidade");
            ModelState.Remove("Bairro");
            ModelState.Remove("Endereco");
            ModelState.Remove("Complemento");
            ModelState.Remove("Numero");

            if (ModelState.IsValid)
            {
                try
                {
                    var idClienteDeCrip = Base64Cipher.Base64Decode(id);

                    if (!int.TryParse(idClienteDeCrip, out int idCliente))
                    {
                        TempData["MSG_E"] = Mensagem.MSG_E015;
                        return View();
                    }
                    var clienteDB = _repositoryCliente.ObterCliente(idCliente);
                    if (clienteDB != null)
                    {
                        clienteDB.Senha = cliente.Senha;

                        _repositoryCliente.Atualizar(clienteDB);
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

        [HttpGet]
		public IActionResult Sair()
		{
			_loginCliente.Logout();

			return RedirectToAction("Index", "Home", new { area = "" });
		}
	}
}
