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

		public IActionResult Visualizar(int id)
		{
			Models.Cliente cliente = _loginCliente.GetCliente();

			Pedido pedido =  _pedidoRepository.ObterPedido(id);

			if (pedido.ClienteId != cliente.Id)
			{
				return new ContentResult()
				{
					Content = "Você não tem permissão!"
				};
			}

			return View(pedido);
		}
	}
}
