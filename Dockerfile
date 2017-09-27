FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY ./publish .
COPY ./src/Hjerpbakk.DIPSBot.Runner/config.json config.json
ENTRYPOINT ["dotnet", "Hjerpbakk.DIPSBot.Runner.dll"]