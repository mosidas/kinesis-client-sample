ARG DOTNET_TAG=8.0

# build stage
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_TAG} AS build
WORKDIR /src
COPY KinesisConsumer/*.csproj ./
RUN dotnet restore --use-current-runtime
COPY KinesisConsumer/ ./
RUN dotnet publish --use-current-runtime -o /app


# runtime stage
FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_TAG}
WORKDIR /app
EXPOSE 80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "KinesisConsumer.dll"]
