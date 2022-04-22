using LojaVirtual.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace LojaVirtual.Repositories.Contracts
{
	public interface IEnderecoEntregaRepository
	{
		void Cadastrar(EnderecoEntrega enderecoEntrega);
		void Atualizar(EnderecoEntrega enderecoEntrega);
		void Excluir(int Id);
		EnderecoEntrega ObterEnderecoEntrega(int Id);
		IList<EnderecoEntrega> ObterTodosEnderecoEntregaCliente(int ClienteId);
	}
}
