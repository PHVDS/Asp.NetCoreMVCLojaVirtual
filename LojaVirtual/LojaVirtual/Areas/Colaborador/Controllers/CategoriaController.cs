﻿using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace LojaVirtual.Areas.Colaborador.Controllers
{
	[Area("Colaborador")]
	//[ColaboradorAutorizacao]
	public class CategoriaController : Controller
	{
		private readonly ICategoriaRepository _categoriaRepository;

		public CategoriaController(ICategoriaRepository categoriaRepository)
		{
			_categoriaRepository = categoriaRepository;
		}

		public IActionResult Index(int? pagina)
		{
			var categorias = _categoriaRepository.ObterTodasCategorias(pagina);

			return View(categorias);
		}

		[HttpGet]
		public IActionResult Cadastrar()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Cadastrar([FromForm]Categoria categoria)
		{
			if (ModelState.IsValid)
			{
				_categoriaRepository.Cadastrar(categoria);

				TempData["MSG_S"] = "Registro salvo com sucesso";

				return RedirectToAction(nameof(Index));
			}
			
			return View();
		}

		[HttpGet]
		public IActionResult Atualizar(int id)
		{
			return View();
		}

		[HttpPost]
		public IActionResult Atualizar([FromForm] Categoria categoria)
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
