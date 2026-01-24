// MappingProfile.cs
using AutoMapper;
using Farmaceutica.Core.DTO;
using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Farmaceutica.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
                .ForMember(dest => dest.SubCategoriaNombre, opt => opt.MapFrom(src => src.SubCategoria.Nombre));
        }
    }
}