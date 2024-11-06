using System.Text;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace KinesisConsumer;

public class KinesisConsumer(Amazon.RegionEndpoint region, string streamName, string containerId)
{
  private readonly AmazonKinesisClient _client = new(region);
  private readonly string _streamName = streamName;
  private readonly string _containerId = containerId;

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
    var assignedShards = shards.Where((_, index) => index % 2 == int.Parse(_containerId));
    Console.WriteLine($"assignedShards: {string.Join(", ", assignedShards.Select(s => s.ShardId))}");
    // 各シャードの処理を非同期で実行
    var tasks = assignedShards.Select(shard => ProcessShardAsync(shard));

    // すべてのタスクが完了するのを待つ
    await Task.WhenAll(tasks);
  }

  private async Task ProcessShardAsync(Shard shard)
  {
    var shardIterator = await GetShardIteratorAsync(shard.ShardId);
    while (!string.IsNullOrEmpty(shardIterator))
    {
      var getRecordsResponse = await GetRecordsAsync(shardIterator);
      foreach (var record in getRecordsResponse.Records)
      {
        Console.WriteLine($"shardId: {shard.ShardId}, record: {Encoding.UTF8.GetString(record.Data.ToArray())}");
      }
      shardIterator = getRecordsResponse.NextShardIterator;
      await Task.Delay(100);
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
      ShardIteratorType = ShardIteratorType.LATEST
    };
    Console.WriteLine($"START GetShardIteratorAsync shardId: {shardId}");
    var shardIteratorResponse = await _client.GetShardIteratorAsync(shardIteratorRequest);
    Console.WriteLine($"END GetShardIteratorAsync shardId: {shardId}");
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
