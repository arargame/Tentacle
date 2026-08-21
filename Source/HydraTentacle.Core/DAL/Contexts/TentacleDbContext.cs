using Hydra.DAL.Core;
using Hydra.DAL.Contexts;
using HydraTentacle.Core.Models.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraTentacle.Core.DAL.Contexts
{
    public class TentacleDbContext : HydraDbContext
    {

        public TentacleDbContext(DbContextOptions<TentacleDbContext> options) : base(options)
        {
            
        }

        public DbSet<Request> Request { get; set; }
        public DbSet<RequestCategory> RequestCategory { get; set; }
        public DbSet<RequestCategoryResponsiblePosition> RequestCategoryResponsiblePosition { get; set; }

        public DbSet<RequestAttachment> RequestAttachment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //BaseObject konvansiyonuna uy: PK = Id (MainController'ın /{id:guid} rotaları ve
            //generic Repository Id üzerinden çalışır). Aynı kategori+pozisyon ikilisi tekil olsun.
            modelBuilder.Entity<RequestCategoryResponsiblePosition>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<RequestCategoryResponsiblePosition>()
                .HasIndex(x => new { x.RequestCategoryId, x.PositionId })
                .IsUnique();
        }

       
    }
}
