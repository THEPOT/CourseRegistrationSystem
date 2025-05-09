using System.Collections.Generic;

namespace CDQTSystem_API.Payload.Response
{
    public class ServiceRequestStatisticsResponse
    {
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
        public List<ServiceRequestTypeStat> ByType { get; set; }
        public List<ServiceRequestStatusStat> ByStatus { get; set; }
    }

    public class ServiceRequestTypeStat
    {
        public string Type { get; set; }
        public int Count { get; set; }
    }

    public class ServiceRequestStatusStat
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
} 