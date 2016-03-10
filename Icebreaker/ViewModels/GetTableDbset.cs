using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Icebreaker.ViewModels
{
    public class GetTableDbset : DbContext
    {
        public DbSet<ViewScoreTable> ViewScoreTable { get; set; }
    }
}