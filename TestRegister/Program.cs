namespace TestRegister
{
	using System.Net.Http.Json;
	using System.Collections.Concurrent;

	class Program
	{
		static readonly string[] FirstNames = { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
		static readonly string[] LastNames = { "An", "Binh", "Cuong", "Dung", "Em", "Feng", "Giang", "Hung", "Ian", "Khanh" };
		static readonly string[] MiddleNames = { "Van", "Thi", "The", "Minh", "Duc", "Hoang", "Thanh", "Quoc", "Dinh", "Hong" };
		static readonly Guid[] MajorIds = {
		Guid.Parse("50000000-0000-0000-0000-000000000001"),
		Guid.Parse("50000000-0000-0000-0000-000000000002")
	};

		static async Task Main(string[] args)
		{
			const string apiUrl = "http://localhost:5003/api/v1/auth/register";
			const int numberOfStudents = 300;

			var client = new HttpClient();
			var results = new ConcurrentBag<(bool success, string email, string message)>();
			var random = new Random();

			Console.WriteLine($"Starting registration of {numberOfStudents} students...");
			var sw = System.Diagnostics.Stopwatch.StartNew();

			var tasks = new List<Task>();
			for (int i = 0; i < numberOfStudents; i++)
			{
				var studentNumber = i + 1;
				var firstName = FirstNames[random.Next(FirstNames.Length)];
				var middleName = MiddleNames[random.Next(MiddleNames.Length)];
				var lastName = LastNames[random.Next(LastNames.Length)];
				var fullName = $"{firstName} {middleName} {lastName}";
				var email = $"student{studentNumber}@university.com";
				var majorId = MajorIds[random.Next(MajorIds.Length)];

				var task = RegisterStudentAsync(client, apiUrl, new
				{
					FullName = fullName,
					Email = email,
					Password = "Password123!",
					MajorId = majorId,
					ImageUrl = "https://example.com/default-avatar.jpg",
					Role = "Student"
				}, results, studentNumber);

				tasks.Add(task);

				// Add a small delay between requests to prevent overwhelming the server
				await Task.Delay(100);
			}

			await Task.WhenAll(tasks);

			sw.Stop();

			// Analyze results
			var successCount = results.Count(r => r.success);
			var failureCount = results.Count(r => !r.success);

			Console.WriteLine("\nRegistration Results:");
			Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
			Console.WriteLine($"Successful registrations: {successCount}");
			Console.WriteLine($"Failed registrations: {failureCount}");
			Console.WriteLine($"Success rate: {(double)successCount / numberOfStudents * 100:F2}%");

			// Display some error messages if any
			var errors = results.Where(r => !r.success)
							  .GroupBy(r => r.message)
							  .Take(5);
			if (errors.Any())
			{
				Console.WriteLine("\nSample error messages:");
				foreach (var error in errors)
				{
					Console.WriteLine($"- {error.Key} (occurred {error.Count()} times)");
				}
			}

			// Save successful emails to file for later use
			var successfulEmails = results.Where(r => r.success)
										.Select(r => $"{r.email},Password123!")
										.ToList();
			await File.WriteAllLinesAsync("successful_students.csv", successfulEmails);
			Console.WriteLine("\nSuccessful student credentials saved to 'successful_students.csv'");
		}

		static async Task RegisterStudentAsync(
			HttpClient client,
			string apiUrl,
			object request,
			ConcurrentBag<(bool success, string email, string message)> results,
			int index)
		{
			try
			{
				var response = await client.PostAsJsonAsync(apiUrl, request);
				var content = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					results.Add((true, request.GetType().GetProperty("Email").GetValue(request).ToString(), "Success"));
					Console.WriteLine($"Registration {index}: Success");
				}
				else
				{
					results.Add((false, request.GetType().GetProperty("Email").GetValue(request).ToString(), content));
					Console.WriteLine($"Registration {index}: Failed - {content}");
				}
			}
			catch (Exception ex)
			{
				results.Add((false, request.GetType().GetProperty("Email").GetValue(request).ToString(), ex.Message));
				Console.WriteLine($"Registration {index}: Error - {ex.Message}");
			}
		}
	}
}
