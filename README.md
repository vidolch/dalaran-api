# Dalaran

Mock API Generator API

### Prerequisites

.NET Core 2.2

## Running the tests

The tests should run from Visual Studio

## Ironclad configuration

In order to configure Iconclad

```
dotnet run -- apis add dalaran_api 512bitpassword -d "Dalaran Web API" -c role -c email -c username -v

dotnet run -- clients add server dalaran_api -n "Web API (Dalaran)" -s 512bitpassword -a openid -a profile -a dalaran_api -a dalaran_api:write -v

dotnet run -- clients add website dalaran_ui -n "Dalaran SPA" -c http://localhost:5051 -r http://localhost:5051/signin-oidc -r http://localhost:5051/home/index -r http://localhost:5051/ -l http://localhost:5051/index.html -a openid -a profile -a email -a role -a dalaran_api -q -v

dotnet run -- clients add console dalaran_console -n "Dalaran Console" -r http://127.0.0.1 -a openid -a profile -a email -a dalaran_api -o -k  -p -q -v
```