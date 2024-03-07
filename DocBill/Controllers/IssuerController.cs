using DocBill.Model;
using DocBill.ViewModels;
using GenHTTP.Api.Content;
using GenHTTP.Api.Content.Templating;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Basics;
using GenHTTP.Modules.Controllers;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Razor;
using System;
using System.Linq;

namespace DocBill.Controllers
{

    #region View Models

    public record class IssuerList(PagedList<Issuer> List);

    public record class IssuerDetails(Issuer Issuer);

    #endregion

    public class IssuerController
    {

        private const int PAGE_SIZE = 10;

        public IHandlerBuilder Index(int page)
        {
            if (page < 1) page = 1;

            using var context = Database.Create();

            IQueryable<Issuer> query = context.Issuers;

            var total = query.Count();

            var records = query.Skip((page - 1) * PAGE_SIZE)
                               .Take(PAGE_SIZE)
                               .OrderBy(u => u.Name)
                               .ToList();

            var pages = (total + PAGE_SIZE - 1) / PAGE_SIZE;

            var paged = new PagedList<Issuer>(records, page, pages, total);

            var list = new IssuerList(paged);

            return ModRazor.Page(Resource.FromAssembly("Issuer.List.cshtml"), (r, h) => new ViewModel<IssuerList>(r, h, list))
                           .Title("Rechnungssteller");
        }

        public IHandlerBuilder Create()
        {
            return ModRazor.Page(Resource.FromAssembly("Issuer.Create.cshtml"))
                           .Title("Rechnungssteller anlegen");
        }

        [ControllerAction(RequestMethod.POST)]
        public IHandlerBuilder Create(Issuer issuer)
        {
            using var context = Database.Create();

            issuer.Created = DateTime.UtcNow;
            issuer.Modified = DateTime.UtcNow;

            context.Issuers.Add(issuer);

            context.SaveChanges();

            return Redirect.To($"/issuers/", true);
        }

        public IHandlerBuilder? Details([FromPath] int id)
        {
            using var context = Database.Create();

            var issuer = context.Issuers
                                .Where(c => c.ID == id)
                                .FirstOrDefault();

            if (issuer == null)
            {
                return null;
            }

            var viewModel = new IssuerDetails(issuer);

            return ModRazor.Page(Resource.FromAssembly("Issuer.Details.cshtml"), (r, h) => new ViewModel<IssuerDetails>(r, h, viewModel))
                           .Title($"{issuer.Name}");
        }

        public IHandlerBuilder? Edit([FromPath] int id)
        {
            using var context = Database.Create();

            var issuer = context.Issuers
                                .Where(c => c.ID == id)
                                .FirstOrDefault();

            if (issuer == null)
            {
                return null;
            }

            var viewModel = new IssuerDetails(issuer);

            return ModRazor.Page(Resource.FromAssembly("Issuer.Editor.cshtml"), (r, h) => new ViewModel<IssuerDetails>(r, h, viewModel))
                           .Title($"{issuer.Name}");
        }

        [ControllerAction(RequestMethod.POST)]
        public IHandlerBuilder? Edit([FromPath] int id, Issuer issuer)
        {
            using var context = Database.Create();

            var existing = context.Issuers
                                  .Where(c => c.ID == id)
                                  .FirstOrDefault();

            if (existing == null)
            {
                return null;
            }

            existing.Name = issuer.Name.Trim();
            existing.Modified = DateTime.UtcNow;

            context.SaveChanges();

            return Redirect.To($"{{controller}}/details/{id}/", true);
        }

    }

}
