nuget SetApiKey %NUGET_KEY%
nuget push TSQL.Parser.2.5.0.snupkg -Source https://api.nuget.org/v3/index.json
nuget push TSQL.Parser.2.5.0.nupkg -Source https://api.nuget.org/v3/index.json
pause