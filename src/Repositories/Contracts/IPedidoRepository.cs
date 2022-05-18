using LojaVirtual.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace LojaVirtual.Repositories.Contracts
{
	public interface IPedidoRepository
	{
		void Cadastrar(Pedido pedido);
		void Atualizar(Pedido pedido);
		Pedido ObterPedido(int Id);
		IPagedList<Pedido> ObterTodosPedidos(int? pagina, int clienteId);
	}
}
