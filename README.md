# KinesisConsumer

## create kinesis stream

```bash
# create
aws kinesis create-stream --stream-name {name} --shard-count {number} --region {region}
# example
aws kinesis create-stream --stream-name my-stream --shard-count 1 --region ap-northeast-1
```

```bash
# confirm
aws kinesis describe-stream --stream-name {name}
# example
aws kinesis describe-stream --stream-name my-stream
```

```bash
# list
aws kinesis list-streams
```
## create api gateway

```bash
# create
aws apigateway create-rest-api --name {name} --region {region}
# example
aws apigateway create-rest-api --name my-api --region ap-northeast-1
```
