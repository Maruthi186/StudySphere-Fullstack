# --- Stage 1: The "Build" stage ---
# We use the official .NET 8 SDK (Software Development Kit) to build our app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all the project files into the 'src' directory
COPY . .

# Tell dotnet to restore all NuGet packages
# We do this in a separate step to speed up future builds
RUN dotnet restore "StudySphereApi.csproj"

# Publish the app in "Release" mode (optimized)
RUN dotnet publish "StudySphereApi.csproj" -c Release -o /app/publish

# --- Stage 2: The "Final" stage ---
# Now we use the smaller .NET 8 "runtime" image, which is all we need to RUN the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# This is the "Start Command"
# It tells Docker to run our app when the container starts
ENTRYPOINT ["dotnet", "StudySphereApi.dll"]