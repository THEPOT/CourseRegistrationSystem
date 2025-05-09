using System;
using System.Collections.Generic;

namespace CDQTSystem_API.Payload.Response
{
    public class ScholarshipReviewResponse
    {
        public Guid TermId { get; set; }
        public string TermName { get; set; }
        public List<ScholarshipRecipientResponse> AwardedStudents { get; set; }
        public List<ScholarshipRecipientResponse> NotAwardedStudents { get; set; }
    }
} 