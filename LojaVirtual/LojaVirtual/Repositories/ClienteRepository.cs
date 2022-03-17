using LojaVirtual.Database;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace LojaVirtual.Repositories
{
	public class ClienteRepository : IClienteRepository
	{
		private readonly IConfiguration _conf;
		private readonly LojaVirtualContext _banco;
		public ClienteRepository(LojaVirtualContext banco, IConfiguration configuration)
		{
			_conf = configuration;
			_banco = banco;
		}
		public void Atualizar(Cliente cliente)
		{
			_banco.Update(cliente);
			_banco.SaveChanges();
		}

		public void Cadastrar(Cliente cliente)
		{
			_banco.Add(cliente);
			_banco.SaveChanges();
		}

		public void Excluir(int Id)
		{
			Cliente cliente = ObterCliente(Id);
			_banco.Remove(cliente);
			_banco.SaveChanges();
		}

		public Cliente Login(string Email, string Senha)
		{
			Cliente cliente = _banco.Clientes.Where(m=>m.Email == Email && m.Senha == Senha).FirstOrDefault();
			return cliente;
		}

		public Cliente ObterCliente(int Id)
		{
			return _banco.Clientes.Find(Id);
		}

		public IPagedList<Cliente> ObterTodosClientes(int? pagina)
		{
			int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");

			int NumeroPagina = pagina ?? 1;
			return _banco.Clientes.ToPagedList<Cliente>(NumeroPagina, RegistroPorPagina);
		}
	}
}
