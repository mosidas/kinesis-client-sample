using Amazon;
using Proto.Aws;
using Proto.Domain;

// TODO: Replace the following values with your own.
string accessKey = "access-key";
string secretKey = "secret-key";
var region = RegionEndpoint.USEast1;
string streamName = "stream-name";

IDataRecorder dataRecorder = new DataRecorder();

var consumer = new AwsKinesisConsumer(accessKey, secretKey, region, streamName, dataRecorder);
while (true)
{
  try
  {
    await consumer.Consume();
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Error: {ex}");
    // wait for 5 seconds before retrying.
    await Task.Delay(5000);
  }
}

