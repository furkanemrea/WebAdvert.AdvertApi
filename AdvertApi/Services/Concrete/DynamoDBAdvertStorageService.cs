using AdvertApi.Models;
using AdvertApi.Services.Abstract;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;

namespace AdvertApi.Services.Concrete
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private string accessKey = string.Empty;
        private string secretKey = string.Empty;
        private readonly AmazonDynamoDBClient _dynamoDBClient;

        public DynamoDBAdvertStorageService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;

            secretKey = _configuration["AWS:DynamoDb:secretKey"];
            accessKey = _configuration["AWS:DynamoDb:accessKey"];

            _dynamoDBClient = new AmazonDynamoDBClient(accessKey, secretKey);
        }

        public async Task<string> Add(AdvertModel advertModel)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(advertModel);

            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

            using (var context = new DynamoDBContext(_dynamoDBClient))
            {
                await context.SaveAsync(dbModel);
            }

            return dbModel.Id;
        } 

        public async Task Confirm(ConfirmAdvertModel confirmAdvertModel)
        {
            using (var context = new DynamoDBContext(_dynamoDBClient))
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

        public async Task<bool> CheckHealthAsync()
        {
            using (var client = new AmazonDynamoDBClient())
            {


                var tableData = await client.DescribeTableAsync("Advert");
                return string.Compare(tableData.Table.TableStatus, "active", true) == 0;

            }
        }
    }
}
