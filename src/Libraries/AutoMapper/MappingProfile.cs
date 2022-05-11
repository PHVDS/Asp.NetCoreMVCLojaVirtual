using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LojaVirtual.Libraries.Json.Resolver;
using LojaVirtual.Libraries.Texto;
using LojaVirtual.Models;
using LojaVirtual.Models.Constants;
using LojaVirtual.Models.ProdutoAgregador;
using Newtonsoft.Json;
using PagarMe;

namespace LojaVirtual.Libraries.AutoMapper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Produto, ProdutoItem>();
			CreateMap<Cliente, EnderecoEntrega>()
				.ForMember(destino => destino.Id, opt => opt.MapFrom(origem => 0))
				.ForMember(destino => destino.Nome, opt => opt.MapFrom(origem => string.Format("Endereço do cliente ({0})", origem.Nome)));

			CreateMap<Transaction, TransacaoPagarMe>();

			CreateMap<TransacaoPagarMe, Pedido>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(origem => 0))
				.ForMember(dest => dest.ClienteId, opt => opt.MapFrom(origem => int.Parse(origem.Customer.ExternalId)))
				.ForMember(dest => dest.TransactionId, opt => opt.MapFrom(origem => origem.Id))
				.ForMember(dest => dest.FreteEmpresa, opt => opt.MapFrom(origem => "ECT - Correios"))
				.ForMember(dest => dest.FormaPagamento, opt => opt.MapFrom(origem => (origem.PaymentMethod == 0) ? MetodoPagamentoConstant.CartaoCredito : MetodoPagamentoConstant.Boleto))
				.ForMember(dest => dest.DadosTransaction, opt => opt.MapFrom(origem => JsonConvert.SerializeObject(origem , new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })))      
				.ForMember(dest => dest.DataRegistro, opt => opt.MapFrom(origem => DateTime.Now))
				.ForMember(dest => dest.ValorTotal, opt => opt.MapFrom(origem => Mascara.ConverterPagarMeIntToDecimal(origem.Amount)));

			CreateMap<List<ProdutoItem>, Pedido>()
				.ForMember(dest => dest.DadosProdutos, opt => opt.MapFrom(origem => JsonConvert.SerializeObject(origem, new JsonSerializerSettings()
				{
					ContractResolver = new ProdutoItemResolver<List<ProdutoItem>>(),
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				})));

			CreateMap<Pedido, PedidoSituacao>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(origem => 0))
				.ForMember(dest => dest.PedidoId, opt => opt.MapFrom(origem => origem.Id))
				.ForMember(dest => dest.Data, opt => opt.MapFrom(origem => DateTime.Now));

			CreateMap<TransactionProduto, PedidoSituacao>()
				.ForMember(dest => dest.Dados, opt => opt.MapFrom(origem => JsonConvert.SerializeObject(origem, new JsonSerializerSettings()
				{
					ContractResolver = new ProdutoItemResolver<List<ProdutoItem>>(),
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				})));
		}
	}
}
