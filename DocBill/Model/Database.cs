using System;

using Microsoft.EntityFrameworkCore;

namespace DocBill.Model
{

    public class Database : DbContext
    {
        private static DbContextOptions<Database>? _Options;

        #region Factory

        public static string ConnectionString
        {
            get
            {
                var server = Environment.GetEnvironmentVariable("DOCBILL_DB_HOST") ?? "localhost";
                var db = Environment.GetEnvironmentVariable("DOCBILL_DB_DATABASE") ?? "docbill";
                var user = Environment.GetEnvironmentVariable("DOCBILL_DB_USER") ?? "docbill";
                var password = Environment.GetEnvironmentVariable("DOCBILL_DB_PASSWORD") ?? "docbill";

                return $"Server={server};Database={db};User Id={user};Password={password}";
            }
        }

        public static Database Create()
        {
            return new Database(_Options ??= GetOptions(true));
        }

        private static DbContextOptions<Database> GetOptions(bool tracking)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Database>();

            optionsBuilder.UseNpgsql(ConnectionString);

            return optionsBuilder.Options;
        }

#pragma warning disable CS8618

        private Database(DbContextOptions options) : base(options) { }

#pragma warning restore CS8618

        #endregion

        #region Entities

        public DbSet<Issuer> Issuers { get; set; }

        public DbSet<Bill> Bills { get; set; }

        #endregion


    }

}
