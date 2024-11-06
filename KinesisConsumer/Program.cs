// コンテナIDを取得
var containerId = Environment.GetEnvironmentVariable("CONTAINER_ID") ?? "0";

var consumer = new KinesisConsumer.KinesisConsumer(Amazon.RegionEndpoint.APNortheast1, "kinesis-sample", containerId);
await consumer.RunAsync();
