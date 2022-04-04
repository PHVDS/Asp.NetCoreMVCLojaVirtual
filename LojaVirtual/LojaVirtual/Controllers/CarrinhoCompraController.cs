using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class CarrinhoCompraController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
