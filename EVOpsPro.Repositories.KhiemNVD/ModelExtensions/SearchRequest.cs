using System;

namespace EVOpsPro.Repositories.KhiemNVD.ModelExtensions
{
    public class SearchRequest
    {
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }

    public class ReminderSearchRequest : SearchRequest
    {
        public int? UserAccountId { get; set; }
        public int? ReminderTypeId { get; set; }
        public string? VehicleVin { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public int? MinDueKm { get; set; }
        public int? MaxDueKm { get; set; }
        public string? Keyword { get; set; }
    }
}
