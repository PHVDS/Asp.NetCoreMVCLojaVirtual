using AutoMapper;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Seguranca;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers.Base
{
	public class BaseController : Controller
	{
		protected readonly CookieCarrinhoCompra _cookieCarrinhoCompra;
		protected readonly CookieFrete _cookieFrete;
		protected readonly IProdutoRepository _produtoRepository;
		protected readonly IMapper _mapper;
		protected readonly WSCorreiosCalcularFrete _wscorreios;
		protected readonly CalcularPacote _calcularPacote;

		public BaseController(CookieFrete cookieFrete, CalcularPacote calcularPacote, WSCorreiosCalcularFrete wscorreios, IMapper mapper, CookieCarrinhoCompra cookieCarrinhoCompra, IProdutoRepository produtoRepository)
		{
			_cookieCarrinhoCompra = cookieCarrinhoCompra;
			_cookieFrete = cookieFrete;
			_produtoRepository = produtoRepository;
			_mapper = mapper;
			_wscorreios = wscorreios;
			_calcularPacote = calcularPacote;
		}
		protected List<ProdutoItem> CarregarProdutoDB()
		{
			List<ProdutoItem> produtoItemNoCarrinho = _cookieCarrinhoCompra.Consultar();

			List<ProdutoItem> produtoItemCompleto = new List<ProdutoItem>();

			foreach (var item in produtoItemNoCarrinho)
			{
				Produto produto = _produtoRepository.ObterProduto(item.Id);

				ProdutoItem produtoItem = _mapper.Map<ProdutoItem>(produto);
				produtoItem.QuantidadeProdutoCarrinho = item.QuantidadeProdutoCarrinho;

				produtoItemCompleto.Add(produtoItem);
			}

			return produtoItemCompleto;
		}

		protected string GerarHash(object obj)
		{
			return StringMD5.MD5Hash(JsonConvert.SerializeObject(obj));
		}
	}
}
