using LojaVirtual.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Login
{
	public class LoginCliente
	{
		private string Key = "Login.Cliente";
		private readonly Sessao.Sessao _sessao;
		public LoginCliente(Sessao.Sessao sessao)
		{
			_sessao = sessao;
		}

		public void Login(Cliente cliente)
		{
			//Serializar
			string clienteJSONString = JsonConvert.SerializeObject(cliente);
			_sessao.Cadastrar(Key, clienteJSONString);
		}

		public Cliente GetCliente()
		{
			//Deserializar
			string clienteJSONString = _sessao.Consultar(Key);
			return JsonConvert.DeserializeObject<Cliente>(clienteJSONString);
		}

		public void Logout()
		{
			_sessao.RemoverTodos();
		}
	}
}
