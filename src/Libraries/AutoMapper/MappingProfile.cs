using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LojaVirtual.Models;
using LojaVirtual.Models.ProdutoAgregador;

namespace LojaVirtual.Libraries.AutoMapper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Produto, ProdutoItem>();
			CreateMap<Cliente, EnderecoEntrega>()
				.ForMember(destino => destino.Id, opt => opt.MapFrom(origem => 0))
				.ForMember(destino => destino.Nome, opt => opt.MapFrom(
					origem => string.Format("Endereço do cliente ({0})", origem.Nome)));
		}
	}
}
