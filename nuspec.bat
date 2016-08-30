@echo off

SET ProjectDirectory="C:\Users\carlos\Dropbox\Proyectos\Cfe\"
SET Nuget=C:\Users\carlos\Desktop\src\.nuget\NuGet.exe
SET NugetDirectory=D:\Nuget\
SET Package=Cfe.1.0.0.0.nupkg

CD %ProjectDirectory%

echo Creando paquete
%Nuget% spec -f >null
%Nuget% pack Cfe.csproj>null
echo --------------------------------------------------------

echo Copiando paquete al repositorio local
copy %Package% %NugetDirectory%%Package% /Y >null
echo --------------------------------------------------------

echo Copiando paquete al repositorio remoto.
%Nuget% push %Package% -s http://10.55.56.217/nuget/ -ApiKey 1BF3941CC8BB6BB4967CDE3DC2D17 >null
echo --------------------------------------------------------