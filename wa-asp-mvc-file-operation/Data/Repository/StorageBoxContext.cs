using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using wa_asp_mvc_file_operation.Data.Models;

namespace wa_asp_mvc_file_operation.Data.Repository
{
    public class StorageBoxContext : DbContext
    {
        private DbSet<CloudUser> users;

        class InitializeContext : DropCreateDatabaseIfModelChanges<StorageBoxContext>
        {
            protected override void Seed(StorageBoxContext context)
            {
                //base.Seed(context);
            }
        }

        public StorageBoxContext() : base("StorageBoxConnectionString")
        {
            Database.SetInitializer(new InitializeContext());
        }

        public DbSet<CloudUser> Users { get => users; set => users = value; }
    }
}