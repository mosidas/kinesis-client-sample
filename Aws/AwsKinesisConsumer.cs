
using System.Text;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Proto.Domain;

namespace Proto.Aws;

public class AwsKinesisConsumer(string accessKey, string secretKey, Amazon.RegionEndpoint region, string streamName, IDataRecorder dataRecorder)
{
  private readonly AmazonKinesisClient _client = new(accessKey, secretKey, region);
  private readonly string _streamName = streamName;
  private readonly IDataRecorder _dataRecorder = dataRecorder;

  public async Task Consume()
  {
    var shards = await GetShards();
    foreach (var shard in shards)
    {
      var shardIterator = await GetShardIterator(shard);
      await foreach (var records in GetRecords(shardIterator))
      {
        _dataRecorder.Record(records);
      }
    }
  }

  private async IAsyncEnumerable<IEnumerable<string>> GetRecords(string shardIterator)
  {
    while (!string.IsNullOrEmpty(shardIterator))
    {
      var request = new GetRecordsRequest
      {
        ShardIterator = shardIterator,
        Limit = 100
      };

      var response = await _client.GetRecordsAsync(request);
      var records = response.Records;
      var ret = records.Select(r => Encoding.UTF8.GetString(r.Data.ToArray()));
      yield return ret;

      shardIterator = response.NextShardIterator;
      // prevent throttling.
      await Task.Delay(100);
    }
  }

  private async Task<List<string>> GetShards()
  {
    var request = new DescribeStreamRequest
    {
      StreamName = _streamName
    };

    var response = await _client.DescribeStreamAsync(request);
    return response.StreamDescription.Shards.Select(s => s.ShardId).ToList();
  }

  private async Task<string> GetShardIterator(string shard)
  {
    var request = new GetShardIteratorRequest
    {
      StreamName = _streamName,
      ShardId = shard,
      ShardIteratorType = ShardIteratorType.TRIM_HORIZON
    };

    var response = await _client.GetShardIteratorAsync(request);
    return response.ShardIterator;

  }
}
