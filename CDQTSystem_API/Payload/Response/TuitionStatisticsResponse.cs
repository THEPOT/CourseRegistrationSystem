using System;
using System.Collections.Generic;

namespace CDQTSystem_API.Payload.Response
{
    public class TuitionStatisticsResponse
    {
        public int TotalStudents { get; set; }
        public int PaidStudents { get; set; }
        public int UnpaidStudents { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public List<ProgramTuitionStatistic> ProgramStatistics { get; set; }
    }

    public class ProgramTuitionStatistic
    {
        public string ProgramName { get; set; }
        public int StudentCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
    }
} 