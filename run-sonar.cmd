SET SONAR_LOGIN=%1

SonarQube.Scanner.MSBuild begin /k:MarWac_Merlin /n:"Merlin" /v:"1.0.0-alpha" ^
    /d:"sonar.host.url=https://sonarqube.com" ^
    /d:"sonar.organization=wachulski-github" ^
    /d:"sonar.login=%SONAR_LOGIN%" ^
    /d:"sonar.cs.dotcover.reportsPaths=dotCover.html"
MSBuild ./MarWac.Merlin.sln /t:Rebuild /p:Configuration=Release
dotCover analyze dotCover.xml
SonarQube.Scanner.MSBuild end /d:"sonar.login=%SONAR_LOGIN%"