# Proto

## local

```bash
docker build -t proto-image .
docker run --rm proto-image --name proto
```
## ECS

```bash
# ECRにログイン
aws ecr get-login-password --region {your-region} | docker login --username AWS --password-stdin {your-account-id}.dkr.ecr.{your-region}.amazonaws.com

# リポジトリを作成（初回のみ）
aws ecr create-repository --repository-name mydotnetapp

# タグ付けしてプッシュ
docker tag mydotnetapp:latest {your-account-id}.dkr.ecr.{your-region}.amazonaws.com/mydotnetapp:latest
docker push {your-account-id}.dkr.ecr.{your-region}.amazonaws.com/mydotnetapp:latest
```
