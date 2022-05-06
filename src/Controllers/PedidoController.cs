using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class PedidoController : Controller
	{
		private readonly IPedidoRepository _pedidoRepository;

		public PedidoController(IPedidoRepository pedidoRepository) 
		{
			_pedidoRepository = pedidoRepository;
		}
		public IActionResult Index(int id)
		{
			Pedido pedido = _pedidoRepository.ObterPedido(id);
			return View(pedido);
		}
	}
}
