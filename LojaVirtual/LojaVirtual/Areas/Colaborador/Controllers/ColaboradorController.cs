using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	public class ColaboradorController : Controller
	{
		private readonly IColaboradorRepository _colaboradorRepository;

		public ColaboradorController(IColaboradorRepository colaboradorRepository)
		{
			_colaboradorRepository = colaboradorRepository;
		}

		public IActionResult Index(int? pagina)
		{
			return View();
		}

		[HttpGet]
		public IActionResult Cadastrar()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Cadastrar([FromForm] Models.Colaborador colaborador)
		{
			return View();
		}

		[HttpGet]
		public IActionResult Atualizar(int id)
		{
			return View();
		}

		[HttpPost]
		public IActionResult Atualizar([FromForm] Models.Colaborador colaborador, int id)
		{
			return View();
		}

		[HttpGet]
		public IActionResult Excluir(int id)
		{
			return View();
		}
	}
}
