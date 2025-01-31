﻿using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Libraries.Validacao
{
    public class SlugCategoriaUnicoAttribute : ValidationAttribute
    {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			ICategoriaRepository _categoriaRepository = (ICategoriaRepository)validationContext.GetService(typeof(ICategoriaRepository));
			Categoria categoria = (Categoria)validationContext.ObjectInstance;

			if (categoria.Id == 0)
			{
				Categoria categoriaDB = _categoriaRepository.ObterCategoria(categoria.Slug);
				if (categoriaDB == null)
				{
					return ValidationResult.Success;
				}
				else
				{
					return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
				}
			}
			else
			{
				Categoria categoriaDB = _categoriaRepository.ObterCategoria(categoria.Slug);
				if (categoriaDB == null)
				{
					return ValidationResult.Success;
				}
				else if (categoriaDB.Id == categoria.Id)
				{
					return ValidationResult.Success;
				}
				else
				{
					return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
				}
			}
		}
	}
}
