using LojaVirtual.Libraries.Login;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Cliente.Controllers
{
	public class PedidoController : Controller
	{
		private readonly LoginCliente _loginCliente;
		private readonly IPedidoRepository _pedidoRepository;

		public PedidoController(LoginCliente loginCliente, IPedidoRepository pedidoRepository)
		{
			_loginCliente = loginCliente;
			_pedidoRepository = pedidoRepository;
		}
		public IActionResult Index(int? pagina)
		{
			Models.Cliente cliente = _loginCliente.GetCliente();
			var pedidos = _pedidoRepository.ObterTodosPedidos(pagina, cliente.Id);
			return View(pedidos);
		}
	}
}
