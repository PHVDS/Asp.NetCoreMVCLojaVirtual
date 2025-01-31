﻿using LojaVirtual.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.CarrinhoCompra
{
	public class CookieFrete
	{
		private readonly string Key = "Carrinho.ValorFrete";
		private readonly Cookie.Cookie _cookie;

		public CookieFrete(Cookie.Cookie cookie)
		{
			_cookie = cookie;
		}

		public void Cadastrar(Frete frete)
		{
			List<Frete> Lista;
			if (_cookie.Existe(Key))
			{
				Lista = Consultar();
				var ItemLocalizado = Lista.SingleOrDefault(a => a.CEP == frete.CEP);

				if (ItemLocalizado == null)
				{
					Lista.Add(frete);
				}
				else
				{
					ItemLocalizado.CodigoCarrinho = frete.CodigoCarrinho;
					ItemLocalizado.ListaValores = frete.ListaValores;
				}
			}
			else
			{
				Lista = new List<Frete>
				{
					frete
				};
			}
			Salvar(Lista);
		}
		public void Atualizar(Frete frete)
		{
			var Lista = Consultar();
			var ItemLocalizado = Lista.SingleOrDefault(a => a.CEP == frete.CEP);

			if (ItemLocalizado != null)
			{
				ItemLocalizado.CodigoCarrinho = frete.CodigoCarrinho;
				ItemLocalizado.ListaValores = frete.ListaValores;
				Salvar(Lista);
			}
		}
		public void Remover(Frete frete)
		{
			var Lista = Consultar();
			var ItemLocalizado = Lista.SingleOrDefault(a => a.CEP == frete.CEP);

			if (ItemLocalizado != null)
			{
				Lista.Remove(ItemLocalizado);
				Salvar(Lista);
			}
		}
		public List<Frete> Consultar()
		{
			if (_cookie.Existe(Key))
			{
				string valor = _cookie.Consultar(Key);
				return JsonConvert.DeserializeObject<List<Frete>>(valor);
			}
			else
			{
				return new List<Frete>();
			}
		}

		public void Salvar(List<Frete> Lista)
		{
			string Valor = JsonConvert.SerializeObject(Lista);
			_cookie.Cadastrar(Key, Valor);
		}

		public bool Existe(string Key)
		{
			if (_cookie.Existe(Key))
			{
				return false;
			}
			return true;
		}
		public void RemoverTodos()
		{
			_cookie.Remover(Key);
		}
	}
}
