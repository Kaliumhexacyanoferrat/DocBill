using DocBill.Controllers;
using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Controllers;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.Websites;
using GenHTTP.Themes.AdminLTE;

namespace DocBill
{

    public static class Project
    {

        public static IHandlerBuilder Create()
        {
            var resources = Resources.From(ResourceTree.FromAssembly("Resources"));

            var logo = Content.From(Resource.FromAssembly("eye.png"));

            var theme = Theme.Create()
                             .Title("DocBill")
                             .FooterRight(RenderFooterRight)
                             .Logo(logo);

            var content = Layout.Create()
                                .AddController<IssuerController>("issuers")
                                .AddController<BillController>("bills")
                                .Add("static", resources);

            var menu = Menu.Empty()
                           .Add("{website}", "Übersicht")
                           .Add("/issuers/", "Rechnungssteller")
                           .Add("/bills/", "Rechnungen");

            return Website.Create()
                          .Theme(theme)
                          .Content(content)
                          .Menu(menu)
                          .AddScript("jquery-validate.js", Resource.FromAssembly("jquery.validate.min.js"))
                          .AddStyle("project.css", Resource.FromAssembly("project.css"));
        }

        private static string RenderFooterRight(IRequest request, IHandler handler)
        {
            return "view <a href=\"https://github.com/Kaliumhexacyanoferrat/DocBill\">on GitHub</a>";
        }

    }

}
