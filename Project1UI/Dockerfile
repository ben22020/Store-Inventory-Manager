FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# The sdk image has all the build tools, 
# and for that reason it's a lot bigger than the aspnet image
# which only contains runtime components. 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["", "Project1UI/"]
RUN dotnet restore "Project1UI/Project1UI.csproj"
COPY . .
WORKDIR "/src/Project1UI"
RUN dotnet build "Project1UI.csproj" -c Release -o /app/build

# The final stage starts again from base, 
# and includes the COPY --from=publish to copy the published output to the final image. 
# This process makes it possible for the final image to be a lot smaller, 
# since it doesn't need to include all of the build tools that were in the sdk image.
FROM build AS publish
RUN dotnet publish "Project1UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project1UI.dll"]