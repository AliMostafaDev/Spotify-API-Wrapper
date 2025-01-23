This project is built using .NET 8. To run it, clone the repository and configure the appsettings.json file with your Spotify API credentials (ClientId, ClientSecret, and RedirectUri).
Ensure you have the Newtonsoft.Json package installed for data deserialization. You can install it using the .NET CLI (dotnet add package Newtonsoft.Json) or via NuGet in Visual Studio. 
Start the application using Visual Studio or the .NET CLI. 
The API is pre-configured and documented using Swagger, which is integrated into the project.
Use the /api/Spotify/authenticate endpoint to authenticate via Spotify and obtain an access token, which can be used to interact with the Spotify API through the provided endpoints.
