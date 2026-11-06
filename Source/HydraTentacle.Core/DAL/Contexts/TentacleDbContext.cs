using Hydra.DAL.Core;
using Hydra.DAL.Contexts;
using Hydra.MailManagement.Entities;
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

        //Hydra.MailManagement modülünün tabloları. Entity'ler kütüphanede yaşıyor, şema kararı
        //(silme davranışı, index'ler) burada — tüketen uygulamanın DbContext'inde. Modül kendi
        //DbContext'ini getirmiyor; böylece mailler uygulamanın ÖZ veritabanında, aynı işlem
        //(transaction) sınırları içinde duruyor.
        //
        //DbSet adları tekil: Hydra'nın TableService'i tablo adını controller/entity adından
        //üretiyor ("Mail" controller → "Mail" tablosu). Çoğul isim ("Mails") ham SQL tarafında
        //tabloyu bulunamaz hale getirirdi.
        public DbSet<Mail> Mail { get; set; }
        public DbSet<MailTemplate> MailTemplate { get; set; }
        public DbSet<MailAttachment> MailAttachment { get; set; }
        public DbSet<BaseObjectMail> BaseObjectMail { get; set; }

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

            ConfigureMailManagement(modelBuilder);
        }

        /// <summary>
        /// Mail modülünün ilişkileri.
        ///
        /// Üç bağ var ve üçü de farklı davranmalı:
        /// <list type="number">
        /// <item><b>Mail → MailTemplate</b> (çok-bir, opsiyonel): <c>Restrict</c>. Bir şablon
        /// silinince ondan üretilmiş binlerce mailin gitmesi kabul edilemez — gönderim kaydı
        /// denetim (audit) verisidir. Şablon önce mailleri boşa çıkarılmadan silinemez.</item>
        /// <item><b>MailAttachment → Mail</b> (bir-çok): <c>Cascade</c>. Ek, mailin parçasıdır;
        /// maili olmayan ek anlamsız ve üstelik byte[] taşıdığı için yer kaplar.</item>
        /// <item><b>BaseObjectMail → Mail</b> (bir-çok): <c>Cascade</c>. Aynı gerekçe — bağ
        /// kaydının maili yoksa hiçbir şeyi bağlamıyordur.</item>
        /// </list>
        ///
        /// <c>BaseObjectMail.EntityId</c> için FK YOK ve olamaz: hedef tablo çalışma anında
        /// <c>EntityType</c> ile belli olur (Request, SystemUser, ...). Bunun bedeli, hedef
        /// kayıt silindiğinde bağın öksüz kalması; karşılığında modül hiçbir uygulamanın
        /// tablolarını tanımak zorunda kalmıyor. Sorgulanabilir kalması için ikili index var.
        /// </summary>
        private static void ConfigureMailManagement(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mail>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.MailTemplate)
                      .WithMany(t => t.Mails)
                      .HasForeignKey(x => x.MailTemplateId)
                      .OnDelete(DeleteBehavior.Restrict);

                //Gönderilmemişleri / hataya düşenleri tarayan sorgular (RetryFailed, dashboard
                //sayaçları) bu kolonu filtreliyor.
                entity.HasIndex(x => x.Status);

                entity.HasIndex(x => x.SentDate);
            });

            modelBuilder.Entity<MailTemplate>(entity =>
            {
                entity.HasKey(x => x.Id);

                //Kod bu entity'nin gerçek anahtarı: MailTemplateService.GetByCodeAsync tek kayıt
                //bekliyor. Tekillik veritabanında zorlanmazsa iki "WELCOME_USER" yazılabilir ve
                //hangisinin gönderildiği rastgele olurdu.
                entity.HasIndex(x => x.Code).IsUnique();
            });

            modelBuilder.Entity<MailAttachment>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Mail)
                      .WithMany(m => m.Attachments)
                      .HasForeignKey(x => x.MailId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BaseObjectMail>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Mail)
                      .WithMany(m => m.BaseObjectMails)
                      .HasForeignKey(x => x.MailId)
                      .OnDelete(DeleteBehavior.Cascade);

                //"Bu Request'e hangi mailler gitti?" sorusunun indeksi.
                entity.HasIndex(x => new { x.EntityType, x.EntityId });
            });
        }
    }
}
