using AdvertApi.Models;

namespace AdvertApi.Services.Abstract
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel advertModel);
        Task Confirm(ConfirmAdvertModel confirmAdvertModel);
        Task<bool> CheckHealthAsync();

    }
}
