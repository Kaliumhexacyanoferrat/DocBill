using DocBill.Model;
using DocBill.ViewModels;
using GenHTTP.Api.Content;
using GenHTTP.Api.Content.Templating;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Basics;
using GenHTTP.Modules.Controllers;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Razor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DocBill.Controllers
{

    public class BillController
    {
        private const int PAGE_SIZE = 10;

        public IHandlerBuilder Index(int page, string status)
        {
            if (page < 1) page = 1;

            using var context = Database.Create();

            IQueryable<Bill> query = context.Bills
                                            .Include(b => b.Issuer);

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<PaymentStatus>(status, true, out var paymentStatus))
                {
                    query = query.Where(a => a.Status == paymentStatus);
                }
            }

            var total = query.Count();

            var records = query.Skip((page - 1) * PAGE_SIZE)
                               .Take(PAGE_SIZE)
                               .OrderByDescending(b => b.Created)
                               .ToList();

            records.ForEach(b => b.Status = DetermineStatus(b));

            var pages = (total + PAGE_SIZE - 1) / PAGE_SIZE;

            var paged = new PagedList<Bill>(records, page, pages, total);

            return ModRazor.Page(Resource.FromAssembly("Bill.List.cshtml"), (r, h) => new ViewModel<PagedList<Bill>>(r, h, paged))
                           .Title("Rechnungen");
        }

        public IHandlerBuilder? Details([FromPath] int id)
        {
            using var context = Database.Create();

            var bill = context.Bills
                              .Include(b => b.Issuer)
                              .Where(c => c.ID == id)
                              .FirstOrDefault();

            if (bill == null)
            {
                return null;
            }

            bill.Status = DetermineStatus(bill);

            return ModRazor.Page(Resource.FromAssembly("Bill.Details.cshtml"), (r, h) => new ViewModel<Bill>(r, h, bill))
                           .Title($"Rechnung {bill.Number}");
        }

        public IHandlerBuilder? Create([FromPath] int issuerID)
        {
            using var context = Database.Create();

            var issuer = context.Issuers
                                .Where(i => i.ID == issuerID)
                                .FirstOrDefault();

            if (issuer == null)
            {
                return null;
            }

            return ModRazor.Page(Resource.FromAssembly("Bill.Create.cshtml"), (r, h) => new ViewModel<Issuer>(r, h, issuer))
                           .Title("Neue Rechnung");
        }

        [ControllerAction(RequestMethod.POST)]
        public IHandlerBuilder? Create([FromPath] int issuerID, Bill bill)
        {
            using var context = Database.Create();

            var issuer = context.Issuers
                                .Where(i => i.ID == issuerID)
                                .FirstOrDefault();

            if (issuer == null)
            {
                return null;
            }

            bill.Issuer = issuer;
            bill.Status = PaymentStatus.Open;

            bill.BillingDate = UnknownToUtc(bill.BillingDate);
            bill.DueDate = UnknownToUtc(bill.DueDate);

            bill.Created = DateTime.UtcNow;
            bill.Modified = DateTime.UtcNow;

            context.Bills.Add(bill);

            context.SaveChanges();

            return Redirect.To($"/bills/details/{bill.ID}/", true);
        }

        public IHandlerBuilder? Edit([FromPath] int id)
        {
            using var context = Database.Create();

            var bill = context.Bills
                              .Include(b => b.Issuer)
                              .Where(b => b.ID == id)
                              .FirstOrDefault();

            if (bill == null)
            {
                return null;
            }

            return ModRazor.Page(Resource.FromAssembly("Bill.Editor.cshtml"), (r, h) => new ViewModel<Bill>(r, h, bill))
                           .AddUsing("System.Globalization")
                           .Title($"Rechnung bearbeiten");
        }

        [ControllerAction(RequestMethod.POST)]
        public IHandlerBuilder? Edit([FromPath] int id, Bill bill)
        {
            using var context = Database.Create();

            var existing = context.Bills
                                  .Where(b => b.ID == id)
                                  .FirstOrDefault();

            if (existing == null)
            {
                return null;
            }

            existing.Modified = DateTime.UtcNow;
            existing.Number = bill.Number.Trim();
            existing.DueDate = UnknownToUtc(bill.DueDate);
            existing.BillingDate = UnknownToUtc(bill.BillingDate);
            existing.Amount = bill.Amount;

            context.SaveChanges();

            return Redirect.To($"/bills/details/{id}/", true);
        }

        private static DateTime UnknownToUtc(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static PaymentStatus DetermineStatus(Bill bill)
        {
            if ((bill.Status == PaymentStatus.Open) && (bill.DueDate < DateTime.UtcNow))
            {
                return PaymentStatus.Due;
            }

            return bill.Status;
        }

    }

}
