# TODO: Build and test in a container also, see https://www.hanselman.com/blog/NETAndDocker.aspx
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY ./publish .
COPY ./src/Hjerpbakk.DIPSBot.Runner/config.json config.json
ENTRYPOINT ["dotnet", "Hjerpbakk.DIPSBot.Runner.dll"]