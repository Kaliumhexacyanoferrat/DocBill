using DocBill;
using DocBill.Infrastructure;
using GenHTTP.Engine;
using GenHTTP.Modules.Practices;

Migrations.Perform();

var project = Project.Create();

return Host.Create()
           .Handler(project)
           .Defaults()
           .Console()
#if DEBUG
           .Development()
#endif
           .Run();