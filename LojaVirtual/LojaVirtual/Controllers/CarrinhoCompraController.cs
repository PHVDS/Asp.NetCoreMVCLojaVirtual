using AutoMapper;
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
	public class CarrinhoCompraController : Controller
	{
		private readonly CarrinhoCompra _carrinhoCompra;
		private readonly IProdutoRepository _produtoRepository;
		private readonly IMapper _mapper;
		private readonly WSCorreiosCalcularFrete _wscorreios;
		private readonly CalcularPacote _calcularPacote;
		public CarrinhoCompraController(CalcularPacote calcularPacote, WSCorreiosCalcularFrete wscorreios, IMapper mapper, CarrinhoCompra carrinhoCompra, IProdutoRepository produtoRepository)
		{
			_carrinhoCompra = carrinhoCompra;
			_produtoRepository = produtoRepository;
			_mapper = mapper;
			_wscorreios = wscorreios;
			_calcularPacote = calcularPacote;
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
				_carrinhoCompra.Cadastrar(item);

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
				_carrinhoCompra.Atualizar(item);
				return Ok(new { mensagem = Mensagem.MSG_S001 });
			}
		}

		public IActionResult RemoverItem(int id)
		{
			_carrinhoCompra.Remover(new ProdutoItem() { Id = id });
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
				lista.Add(valorPAC);
				lista.Add(valorSEDEX);
				lista.Add(valorSEDEX10);

				return Ok(lista);
			}
			catch (Exception e)
			{

				return BadRequest(e);
			}
		}

		private List<ProdutoItem> CarregarProdutoDB()
		{
			List<ProdutoItem> produtoItemNoCarrinho = _carrinhoCompra.Consultar();

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
	}
}
