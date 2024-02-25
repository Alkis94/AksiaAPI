using AksiaAPI.Models.Business;
using AksiaAPI.Models.DTOs;
using AksiaAPI.Models.Entities;
using AutoMapper;

namespace AksiaAPI.Models.Mappings
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            MapData();
        }

        private void MapData()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Amount, x => x.MapFrom(src => $"${src.Amount}"));

            CreateMap<TransactionInsertDto, TransactionInsertOrUpdate>();
        }
    }
}
