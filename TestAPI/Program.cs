namespace TestAPI
{
	using System.Net.Http.Json;
	using System.Collections.Concurrent;
	using System.Text.Json;

	class Program
	{
		private static readonly HttpClient client = new HttpClient();
		private static readonly int MaxRetries = 3;
		private static readonly int RetryDelayMs = 1000;

		static async Task Main(string[] args)
		{
			const string baseUrl = "http://localhost:5003/api/v1";
			const string loginUrl = $"{baseUrl}/auth/login";
			const string registerUrl = $"{baseUrl}/CourseRegistrations/register";
			const string courseOfferingId = "7aa29c63-6aef-4d3a-a277-3f116e32fe92";
			const int startStudentNumber = 2;
			const int endStudentNumber = 299;

			// Configure HttpClient
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

			var results = new ConcurrentBag<(bool success, string studentEmail, string message, TimeSpan responseTime)>();
			var semaphore = new SemaphoreSlim(10); // Limit concurrent requests

			Console.WriteLine("Starting course registration test...");
			var sw = System.Diagnostics.Stopwatch.StartNew();

			var tasks = new List<Task>();
			for (int i = startStudentNumber; i <= endStudentNumber; i++)
			{
				await semaphore.WaitAsync(); // Wait for a slot to be available

				var studentEmail = $"student{i}@university.com";
				var task = ProcessStudentRegistrationAsync(
					loginUrl,
					registerUrl,
					studentEmail,
					"Password123!",
					courseOfferingId,
					results,
					i,
					semaphore
				);
				tasks.Add(task);

				// Small delay between starting new tasks
				await Task.Delay(100);
			}

			await Task.WhenAll(tasks);

			sw.Stop();

			// Analyze results
			var successCount = results.Count(r => r.success);
			var failureCount = results.Count(r => !r.success);
			var avgResponseTime = results.Average(r => r.responseTime.TotalMilliseconds);

			Console.WriteLine("\nTest Results:");
			Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
			Console.WriteLine($"Total requests: {endStudentNumber - startStudentNumber + 1}");
			Console.WriteLine($"Successful registrations: {successCount}");
			Console.WriteLine($"Failed registrations: {failureCount}");
			Console.WriteLine($"Success rate: {(double)successCount / (endStudentNumber - startStudentNumber + 1) * 100:F2}%");
			Console.WriteLine($"Average response time: {avgResponseTime:F2}ms");

			// Group and display error messages
			var errors = results.Where(r => !r.success)
							  .GroupBy(r => r.message)
							  .OrderByDescending(g => g.Count())
							  .Take(5);
			if (errors.Any())
			{
				Console.WriteLine("\nSample error messages:");
				foreach (var error in errors)
				{
					Console.WriteLine($"- {error.Key} (occurred {error.Count()} times)");
				}
			}

			// Check if specific error for "course full" exists
			var courseFull = results.Where(r => !r.success && r.message.Contains("full", StringComparison.OrdinalIgnoreCase));
			if (courseFull.Any())
			{
				Console.WriteLine($"\nCourse full errors: {courseFull.Count()}");
				Console.WriteLine($"First course full error at student: {courseFull.Min(r => int.Parse(r.studentEmail.Replace("student", "").Replace("@university.com", "")))}");
			}
		}

		static async Task ProcessStudentRegistrationAsync(
			string loginUrl,
			string registerUrl,
			string email,
			string password,
			string courseOfferingId,
			ConcurrentBag<(bool success, string studentEmail, string message, TimeSpan responseTime)> results,
			int index,
			SemaphoreSlim semaphore)
		{
			var sw = new System.Diagnostics.Stopwatch();
			try
			{
				// Login with retry
				LoginResponse loginResult = null;
				for (int attempt = 0; attempt < MaxRetries; attempt++)
				{
					try
					{
						var loginResponse = await client.PostAsJsonAsync(loginUrl, new
						{
							Email = email,
							Password = password
						});

						if (loginResponse.IsSuccessStatusCode)
						{
							var loginContent = await loginResponse.Content.ReadAsStringAsync();
							loginResult = JsonSerializer.Deserialize<LoginResponse>(loginContent, new JsonSerializerOptions
							{
								PropertyNameCaseInsensitive = true
							});
							break;
						}
						else
						{
							var errorContent = await loginResponse.Content.ReadAsStringAsync();
							Console.WriteLine($"Student {index}: Login attempt {attempt + 1} failed - {errorContent}");
						}

						if (attempt < MaxRetries - 1)
						{
							await Task.Delay(RetryDelayMs);
						}
					}
					catch (Exception ex) when (attempt < MaxRetries - 1)
					{
						Console.WriteLine($"Student {index}: Login exception {attempt + 1} - {ex.Message}");
						await Task.Delay(RetryDelayMs);
					}
				}

				if (loginResult == null)
				{
					results.Add((false, email, "Login failed after multiple attempts", TimeSpan.Zero));
					Console.WriteLine($"Student {index}: Login failed after multiple attempts");
					return;
				}

				// Register for course with retry
				sw.Start();
				for (int attempt = 0; attempt < MaxRetries; attempt++)
				{
					try
					{
						// Create registration request using proper formatting
						var registrationRequest = new
						{
							courseOfferingId = courseOfferingId
						};

						using var request = new HttpRequestMessage(HttpMethod.Post, registerUrl);
						request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

						// Serialize manually to ensure proper format
						var json = JsonSerializer.Serialize(registrationRequest);
						request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

						// For debugging
						Console.WriteLine($"Student {index}: Sending request with body: {json}");

						var registrationResponse = await client.SendAsync(request);
						var content = await registrationResponse.Content.ReadAsStringAsync();

						if (registrationResponse.IsSuccessStatusCode)
						{
							sw.Stop();
							results.Add((true, email, "Success", sw.Elapsed));
							Console.WriteLine($"Student {index}: Registration successful");
							return;
						}

						if (attempt == MaxRetries - 1)
						{
							sw.Stop();
							results.Add((false, email, $"Registration failed: {content}", sw.Elapsed));
							Console.WriteLine($"Student {index}: Registration failed - {content}");
						}
						else
						{
							Console.WriteLine($"Student {index}: Registration attempt {attempt + 1} failed - {content}");
							await Task.Delay(RetryDelayMs);
						}
					}
					catch (Exception ex)
					{
						if (attempt == MaxRetries - 1)
						{
							sw.Stop();
							results.Add((false, email, $"Error: {ex.Message}", sw.Elapsed));
							Console.WriteLine($"Student {index}: Error - {ex.Message}");
						}
						else
						{
							Console.WriteLine($"Student {index}: Registration exception {attempt + 1} - {ex.Message}");
							await Task.Delay(RetryDelayMs);
						}
					}
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore slot
				if (sw.IsRunning) sw.Stop();
			}
		}

		class LoginResponse
		{
			public string AccessToken { get; set; }
			public string RefreshToken { get; set; }
			public string FullName { get; set; }
			public string Role { get; set; }
		}
	}
}