using AutoMapper;
using LojaVirtual.Controllers.Base;
using LojaVirtual.Libraries.CarrinhoCompra;
using LojaVirtual.Libraries.Gerenciador.Frete;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Models.ProdutoAgregador;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
	public class CarrinhoCompraController : BaseController
	{
		public CarrinhoCompraController(CookieValorPrazoFrete cookieValorPrazoFrete, CalcularPacote calcularPacote, WSCorreiosCalcularFrete wscorreios, IMapper mapper, CookieCarrinhoCompra cookieCarrinhoCompra, IProdutoRepository produtoRepository) 
			: base(cookieValorPrazoFrete, calcularPacote, wscorreios, mapper, cookieCarrinhoCompra, produtoRepository)
		{ 
		
		}
		public IActionResult Index()
		{
			List<ProdutoItem> produtoItemCompleto = CarregarProdutoDB();
			return View(produtoItemCompleto);
		}

		public IActionResult AdicionarItem(int id)
		{
			Produto produto = _produtoRepository.ObterProduto(id);
			if (produto == null)
			{

				return View("NaoExisteItem");
			}
			else
			{
				var item = new ProdutoItem() { Id = id, QuantidadeProdutoCarrinho = 1};
				_cookieCarrinhoCompra.Cadastrar(item);

				return RedirectToAction(nameof(Index));
			}
		}

		public IActionResult AlterarQuantidade(int id, int quantidade)
		{
			Produto produto = _produtoRepository.ObterProduto(id);
			if (quantidade < 1)
			{
				return BadRequest(new { mensagem = Mensagem.MSG_E007 });
			}
			else if (quantidade > produto.Quantidade)
			{
				return BadRequest(new { mensagem = Mensagem.MSG_E008 });
			}
			else
			{
				var item = new ProdutoItem() { Id = id, QuantidadeProdutoCarrinho = quantidade };
				_cookieCarrinhoCompra.Atualizar(item);
				return Ok(new { mensagem = Mensagem.MSG_S001 });
			}
		}

		public IActionResult RemoverItem(int id)
		{
			_cookieCarrinhoCompra.Remover(new ProdutoItem() { Id = id });
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> CalcularFrete(int cepDestino)
		{
			try
			{
				List<ProdutoItem> produtos = CarregarProdutoDB();
				List<Pacote> pacotes = _calcularPacote.CalcularPacotesDeProdutos(produtos);

				ValorPrazoFrete valorPAC = await _wscorreios.CalcularFrete(cepDestino.ToString(), TipoFreteConstant.PAC, pacotes);
				ValorPrazoFrete valorSEDEX = await _wscorreios.CalcularFrete(cepDestino.ToString(), TipoFreteConstant.SEDEX, pacotes);
				ValorPrazoFrete valorSEDEX10 = await _wscorreios.CalcularFrete(cepDestino.ToString(), TipoFreteConstant.SEDEX10, pacotes);

				List<ValorPrazoFrete> lista = new List<ValorPrazoFrete>();

				if (valorPAC != null) lista.Add(valorPAC);
				if (valorSEDEX != null) lista.Add(valorSEDEX);
				if (valorSEDEX10 != null) lista.Add(valorSEDEX10);

				_cookieValorPrazoFrete.Cadastrar(lista);

				return Ok(lista);
			}
			catch (Exception e)
			{
				_cookieValorPrazoFrete.Remover();
				return BadRequest(e);
			}
		}
	}
}
