namespace CourseRegistration_API.Helpers
{
	public class ScheduleHelper
	{
		public bool HasScheduleConflict(string schedule1, string schedule2)
		{
			if (string.IsNullOrEmpty(schedule1) || string.IsNullOrEmpty(schedule2))
				return false;

			try
			{
				// Parse schedules
				var slots1 = ParseSchedule(schedule1);
				var slots2 = ParseSchedule(schedule2);

				// Check for any overlap between time slots
				foreach (var slot1 in slots1)
				{
					foreach (var slot2 in slots2)
					{
						if (slot1.Day == slot2.Day &&
							((slot1.StartTime <= slot2.StartTime && slot2.StartTime < slot1.EndTime) ||
							 (slot2.StartTime <= slot1.StartTime && slot1.StartTime < slot2.EndTime)))
						{
							return true; // Conflict found
						}
					}
				}

				return false; // No conflict
			}
			catch
			{
				// If parsing fails, fall back to simple string comparison for safety
				return schedule1 == schedule2;
			}
		}
		private List<TimeSlot> ParseSchedule(string schedule)
		{
			var result = new List<TimeSlot>();

			// Example format: "Mon,Wed 09:00-10:30; Fri 13:00-15:00"
			var sections = schedule.Split(';');

			foreach (var section in sections)
			{
				var trimmed = section.Trim();
				if (string.IsNullOrEmpty(trimmed)) continue;

				var parts = trimmed.Split(' ');
				if (parts.Length < 2) continue;

				var days = parts[0].Split(',');
				var times = parts[1].Split('-');

				if (times.Length != 2) continue;

				var startTime = ParseTime(times[0]);
				var endTime = ParseTime(times[1]);

				foreach (var day in days)
				{
					result.Add(new TimeSlot
					{
						Day = NormalizeDay(day.Trim()),
						StartTime = startTime,
						EndTime = endTime
					});
				}
			}

			return result;
		}

		private string NormalizeDay(string day)
		{
			// Convert day abbreviations to standard format
			day = day.ToLower();
			if (day.StartsWith("mon")) return "Monday";
			if (day.StartsWith("tue")) return "Tuesday";
			if (day.StartsWith("wed")) return "Wednesday";
			if (day.StartsWith("thu")) return "Thursday";
			if (day.StartsWith("fri")) return "Friday";
			if (day.StartsWith("sat")) return "Saturday";
			if (day.StartsWith("sun")) return "Sunday";
			return day; // Return as is if not recognized
		}

		private TimeSpan ParseTime(string time)
		{
			// Handle different time formats: 9:00, 09:00, 9:00AM, etc.
			time = time.Trim().ToUpper();

			// Remove AM/PM and adjust hours if PM
			bool isPM = time.EndsWith("PM");
			if (isPM || time.EndsWith("AM"))
			{
				time = time.Substring(0, time.Length - 2).Trim();
			}

			// Parse HH:MM format
			var parts = time.Split(':');
			int hours = int.Parse(parts[0]);
			int minutes = parts.Length > 1 ? int.Parse(parts[1]) : 0;

			// Adjust for PM
			if (isPM && hours < 12)
			{
				hours += 12;
			}

			return new TimeSpan(hours, minutes, 0);
		}

		// Class to represent a time slot
		private class TimeSlot
		{
			public string Day { get; set; }
			public TimeSpan StartTime { get; set; }
			public TimeSpan EndTime { get; set; }
		}
	}
}
