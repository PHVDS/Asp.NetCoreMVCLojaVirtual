﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LojaVirtual.Models;
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

			CreateMap<Transaction, Pedido>()
				.ForMember(dest => dest.ClienteId, opt => opt.MapFrom(origem => int.Parse(origem.Customer.Id)))
				.ForMember(dest => dest.TransactionId, opt => opt.MapFrom(origem => origem.Id))
				.ForMember(dest => dest.FreteEmpresa, opt => opt.MapFrom(origem => "ECT - Correios"))
				.ForMember(dest => dest.FormaPagamento, opt => opt.MapFrom(origem => (origem.PaymentMethod == 0) ? "Cartão de Crédito" : "Boleto"))
				.ForMember(dest => dest.DadosTransaction, opt => opt.MapFrom(origem => JsonConvert.SerializeObject(origem)))
				.ForMember(dest => dest.DataRegistro, opt => opt.MapFrom(origem => DateTime.Now));
		}
	}
}
