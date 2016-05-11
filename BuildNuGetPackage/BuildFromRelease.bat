@echo off

%~d0
cd "%~p0"

del *.nu*
del *.dll
del *.pdb
del *.xml

copy ..\ProductiveRage.ReactRouting\bin\Release\ProductiveRage.ReactRouting.dll > nul
copy ..\ProductiveRage.ReactRouting\bin\Release\ProductiveRage.ReactRouting.xml > nul

copy ..\ProductiveRage.ReactRouting.nuspec > nul
..\packages\NuGet.CommandLine.3.4.3\tools\nuget pack -NoPackageAnalysis ProductiveRage.ReactRouting.nuspec