using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Cliente.Controllers
{
	public class PedidoController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
