using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	public class PedidoController : Controller
	{
		private readonly IPedidoRepository _pedidoRepository;

		public PedidoController(IPedidoRepository pedidoRepository)
		{
			_pedidoRepository = pedidoRepository;
		}

		public IActionResult Index(int? pagina, string codigoPedido, string cpf)
		{
			var pedidos = _pedidoRepository.ObterTodosPedidos(pagina, codigoPedido, cpf);

			return View(pedidos);
		}
	}
}
