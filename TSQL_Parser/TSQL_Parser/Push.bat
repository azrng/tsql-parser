nuget SetApiKey %NUGET_KEY%
nuget push TSQL.Parser.2.2.2.snupkg -Source https://api.nuget.org/v3/index.json
nuget push TSQL.Parser.2.2.2.nupkg -Source https://api.nuget.org/v3/index.json
pause