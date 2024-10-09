using System.Text;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace KinesisConsumer;

public class KinesisConsumer(Amazon.RegionEndpoint region, string streamName)
{
  private readonly AmazonKinesisClient _client = new(region);
  private readonly string _streamName = streamName;
  public async Task Consume()
  {
    var describeRequest = new DescribeStreamRequest()
    {
      StreamName = _streamName
    };
    var describeResponse = await _client.DescribeStreamAsync(describeRequest);
    List<Shard> shards = describeResponse.StreamDescription.Shards;
    foreach (var shard in shards)
    {
      var shardIteratorRequest = new GetShardIteratorRequest()
      {
        StreamName = _streamName,
        ShardId = shard.ShardId,
        ShardIteratorType = ShardIteratorType.TRIM_HORIZON
      };
      var shardIteratorResponse = await _client.GetShardIteratorAsync(shardIteratorRequest);
      string shardIterator = shardIteratorResponse.ShardIterator;
      while (!string.IsNullOrEmpty(shardIterator))
      {
        var getRecordsRequest = new GetRecordsRequest()
        {
          ShardIterator = shardIterator
        };
        var getRecordsResponse = await _client.GetRecordsAsync(getRecordsRequest);
        foreach (var record in getRecordsResponse.Records)
        {
          Console.WriteLine($"record: {Encoding.UTF8.GetString(record.Data.ToArray())}");
        }
        shardIterator = getRecordsResponse.NextShardIterator;
        await Task.Delay(1000);
      }
    }

  }
}
