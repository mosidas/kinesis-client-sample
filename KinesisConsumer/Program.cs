

var consumer = new KinesisConsumer.KinesisConsumer(Amazon.RegionEndpoint.APNortheast1, "my-stream");
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
