# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/Core/CursorProject0.Core.csproj", "src/Core/"]
COPY ["src/ChatUI/ChatUI.csproj", "src/ChatUI/"]
RUN dotnet restore "src/ChatUI/ChatUI.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "src/ChatUI/ChatUI.csproj" -c Release -o /app/build
RUN dotnet publish "src/ChatUI/ChatUI.csproj" -c Release -o /app/publish

# Final stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY src/ChatUI/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80 
