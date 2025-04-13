using System;
using System.Collections.Generic;

namespace CDQTSystem_API.Payload.Request
{
    public class CourseOfferingUpdateRequest
    {
        public List<ScheduleDetail> Schedules { get; set; }
        public int Capacity { get; set; }
        public Guid InstructorId { get; set; }
        public Guid? ClassroomId { get; set; }
    }

}
