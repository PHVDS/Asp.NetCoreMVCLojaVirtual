﻿using LojaVirtual.Database;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace LojaVirtual.Repositories
{
	public class ColaboradorRepository : IColaboradorRepository
	{
		private readonly LojaVirtualContext _banco;
		private readonly IConfiguration _conf;
		public ColaboradorRepository(LojaVirtualContext banco, IConfiguration configuration) 
		{
			_banco = banco;
			_conf = configuration;
		}
		public void Atualizar(Colaborador colaborador)
		{
			_banco.Update(colaborador);
			_banco.Entry(colaborador).Property(a => a.Senha).IsModified = false;
			_banco.SaveChanges();
		}

		public void AtualizarSenha(Colaborador colaborador)
		{
			_banco.Update(colaborador);
			_banco.Entry(colaborador).Property(a => a.Nome).IsModified = false;
			_banco.Entry(colaborador).Property(a => a.Email).IsModified = false;
			_banco.Entry(colaborador).Property(a => a.Tipo).IsModified = false;
			_banco.SaveChanges();
		}

		public void Cadastrar(Colaborador colaborador)
		{
			_banco.Add(colaborador);
			_banco.SaveChanges();
		}

		public void Excluir(int Id)
		{
			Colaborador colaborador = ObterColaborador(Id);
			_banco.Remove(colaborador);
			_banco.SaveChanges();
		}

		public Colaborador Login(string Email, string Senha)
		{
			Colaborador colaborador = _banco.Colaboradores.Where(m => m.Email == Email && m.Senha == Senha).FirstOrDefault();
			return colaborador;
		}

		public Colaborador ObterColaborador(int Id)
		{
			return _banco.Colaboradores.Find(Id);
		}

		public List<Colaborador> ObterColaboradorPorEmail(string email)
		{
			return _banco.Colaboradores.Where(a => a.Email == email).AsNoTracking().ToList();
		}

		public IPagedList<Colaborador> ObterTodosColaboradores(int? pagina)
		{
			int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");

			int NumeroPagina = pagina ?? 1;
			return _banco.Colaboradores.Where(a => a.Tipo != ColaboradorTipoConstant.Gerente).ToPagedList<Colaborador>(NumeroPagina, RegistroPorPagina);
		}
	}
}
