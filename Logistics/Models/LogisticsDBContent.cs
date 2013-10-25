using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Logistics.Models
{
    public class LogisticsDBContent:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }

        public DbSet<Contact> Contacts { get; set; }
    }
}