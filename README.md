# KinesisConsumer

## create kinesis stream

```bash
# create
aws kinesis create-stream --stream-name {name} --shard-count {number}
# example
aws kinesis create-stream --stream-name my-stream --shard-count 1
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

