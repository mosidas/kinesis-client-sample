var consumer = new KinesisConsumer.KinesisConsumer(Amazon.RegionEndpoint.APNortheast1, "my-stream");
await consumer.RunAsync();
