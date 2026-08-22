using System.ComponentModel.DataAnnotations;

namespace HydraTentacle.Core.Constants
{
    // Enum üyelerindeki [Display(Name = "...")] etiketleri, LookupService.GetEnumMemberLabel
    // tarafından okunur ve UI'ın her yerinde (grid hücresi, detay ekranı, create/edit dropdown'ı,
    // liste filtresi) aynı metin gösterilir. Etiket yoksa üyenin kendi adına düşülür.
    //
    // Etiketler enum'un tanımlandığı assembly'de durur; bu yüzden dile özgü etiketleme
    // uygulamanın KENDİ enum'larında yapılır. Ortak Hydra çekirdeğindeki enum'lara
    // (LogType, LogProcessType ...) etiket koyulmaz, üye adlarıyla kalırlar.

    public enum RequestStatus
    {
        [Display(Name = "Open")]
        Open,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Waiting")]
        Waiting,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Cancelled")]
        Cancelled
    }

    public enum RequestPriority
    {
        [Display(Name = "Low")]
        Low,

        [Display(Name = "Normal")]
        Normal,

        [Display(Name = "High")]
        High,

        [Display(Name = "Critical")]
        Critical
    }

    public enum RequestSlaLevel
    {
        [Display(Name = "None")]
        None,

        [Display(Name = "Standard")]
        Standard,

        [Display(Name = "Urgent")]
        Urgent,

        [Display(Name = "Emergency")]
        Emergency
    }
}
