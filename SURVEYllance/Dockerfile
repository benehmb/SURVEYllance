# Use the ASP.NET 5.0 image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

# Set the working directory for any further instructions
WORKDIR /app

# Expose ports 80 and 443 for external connections
EXPOSE 80
EXPOSE 443

# Use the .NET SDK 5.0 image as the build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Set the working directory for the build image
WORKDIR /src

# Copy the SURVEYllance project files to the build image
COPY ["SURVEYllance/SURVEYllance.csproj", "SURVEYllance/"]

# Restore the project dependencies
RUN dotnet restore "SURVEYllance/SURVEYllance.csproj"

# Copy the remaining files to the build image
COPY . .

# Set the working directory to the SURVEYllance project
WORKDIR "/src/SURVEYllance"

# Build the SURVEYllance project in release mode and output the result to /app/build
RUN dotnet build "SURVEYllance.csproj" -c Release -o /app/build

# Use the previous build as the starting point for publishing the application
FROM build AS publish

# Publish the SURVEYllance project in release mode to /app/publish
RUN dotnet publish "SURVEYllance.csproj" -c Release -o /app/publish

# Use the base image as the final image
FROM base AS final

# Set the working directory to /app
WORKDIR /app

# Copy the published application files from the previous image to the final image
COPY --from=publish /app/publish .

# Set the ENTRYPOINT to start the SURVEYllance application using the .dll file
ENTRYPOINT ["dotnet", "SURVEYllance.dll"]
