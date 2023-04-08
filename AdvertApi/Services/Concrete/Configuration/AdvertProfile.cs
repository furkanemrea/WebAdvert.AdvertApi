using AdvertApi.Models;
using AutoMapper;

namespace AdvertApi.Services.Concrete.Configuration
{
    public class AdvertProfile:Profile  
    {

        public AdvertProfile()
        {
            CreateMap<AdvertModel,AdvertDbModel>();
        }

    }
}
