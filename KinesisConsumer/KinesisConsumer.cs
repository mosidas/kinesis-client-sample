using System.Text;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace KinesisConsumer;

public class KinesisConsumer(Amazon.RegionEndpoint region, string streamName)
{
  private readonly AmazonKinesisClient _client = new(region);
  private readonly string _streamName = streamName;

  public async Task RunAsync()
  {
    while (true)
    {
      try
      {
        await Consume();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error: {ex}");

        // wait for 5 seconds before retrying.
        await Task.Delay(5000);
      }
    }
  }
  private async Task Consume()
  {
    IEnumerable<Shard> shards = await GetShards();
    foreach (var shard in shards)
    {
      var shardIterator = await GetShardIteratorAsync(shard.ShardId);
      while (!string.IsNullOrEmpty(shardIterator))
      {
        var getRecordsResponse = await GetRecordsAsync(shardIterator);
        foreach (var record in getRecordsResponse.Records)
        {
          Console.WriteLine($"record: {Encoding.UTF8.GetString(record.Data.ToArray())}");
        }
        shardIterator = getRecordsResponse.NextShardIterator;
        await Task.Delay(100);
      }
    }
  }

  private async Task<IEnumerable<Shard>> GetShards()
  {
    var describeRequest = new DescribeStreamRequest()
    {
      StreamName = _streamName
    };
    var describeResponse = await _client.DescribeStreamAsync(describeRequest);
    return describeResponse.StreamDescription.Shards;
  }

  private async Task<string> GetShardIteratorAsync(string shardId)
  {
    var shardIteratorRequest = new GetShardIteratorRequest()
    {
      StreamName = _streamName,
      ShardId = shardId,
      ShardIteratorType = ShardIteratorType.TRIM_HORIZON
    };
    var shardIteratorResponse = await _client.GetShardIteratorAsync(shardIteratorRequest);
    return shardIteratorResponse.ShardIterator;
  }

  private async Task<GetRecordsResponse> GetRecordsAsync(string shardIterator)
  {
    var getRecordsRequest = new GetRecordsRequest()
    {
      ShardIterator = shardIterator
    };
    var getRecordsResponse = await _client.GetRecordsAsync(getRecordsRequest);
    return getRecordsResponse;
  }
}
