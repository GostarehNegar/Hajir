REM dotnet publish -o .\.build\net5.0 -f net5.0 -c release src\Projects\Tamin\Tamin.Crm1420.Server
REM dotnet publish -o .\.build\net461 -f net461 -c release src\Projects\Tamin\Tamin.Crm1420.Server
REM dotnet publish -o .\.build\GN.Dynamic\net461 -f net461 -c release -r win-x64 --self-contained true src\\GN.Dynamic.Service
dotnet publish -o .\.build\Hajir\Hajir.Crm.Xrm.Service -f net461 -c release -r win-x64 --self-contained true src\Hajir.Crm.Xrm.Service
dotnet publish -o .\.build\Hajir\Hajir.Crm.Infrastructure.Service -c release -r win-x64 --self-contained true src\Hajir.Crm.Infrastructure.Service
REM dotnet publish -o .\.build\GN.Dynamic -f net5.0 -c release -r win-x64 --self-contained true src\Applications\GN.Dynamic.ServiceManager 
