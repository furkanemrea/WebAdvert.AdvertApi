using AdvertApi.Models;
using AdvertApi.Services.Abstract;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;

namespace AdvertApi.Services.Concrete
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorageService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> Add(AdvertModel advertModel)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(advertModel);

            dbModel.Id = new Guid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }
            return dbModel.Id;
        }

        public async Task Confirm(ConfirmAdvertModel confirmAdvertModel)
        {
            using (var client = new AmazonDynamoDBClient())
            {

                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDbModel>(confirmAdvertModel.Id);
                    if (record == null)
                    {
                        throw new KeyNotFoundException($"A record with id = {confirmAdvertModel.Id} was not found");
                    }

                    if (confirmAdvertModel.Status == AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);

                    }
                    else
                    {
                        await context.DeleteAsync(record);
                    }
                }
            }
        }
    }
}
