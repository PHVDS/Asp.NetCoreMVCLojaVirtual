using LojaVirtual.Database;
using LojaVirtual.Models;
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
	public class PedidoRepository : IPedidoRepository
	{
		private readonly IConfiguration _conf;
		private readonly LojaVirtualContext _banco;

		public PedidoRepository(LojaVirtualContext banco, IConfiguration configuration)
		{
			_banco = banco;
			_conf = configuration;
		}
		public void Atualizar(Pedido pedido)
		{
			_banco.Update(pedido);
			_banco.SaveChanges();
		}

		public void Cadastrar(Pedido pedido)
		{
			_banco.Add(pedido);
			_banco.SaveChanges();
		}

		public Pedido ObterPedido(int Id)
		{
			return _banco.Pedidos.Include(a => a.PedidoSituacoes).Where(a => a.Id == Id).FirstOrDefault();
		}

		public IPagedList<Pedido> ObterTodosProdutos(int? pagina, int clienteId)
		{
			int RegistroPorPagina = _conf.GetValue<int>("RegistroPorPagina");
			int NumeroPagina = pagina ?? 1;

			return _banco.Pedidos.Include(a => a.PedidoSituacoes).ToPagedList<Pedido>(NumeroPagina, RegistroPorPagina);
		}
	}
}
