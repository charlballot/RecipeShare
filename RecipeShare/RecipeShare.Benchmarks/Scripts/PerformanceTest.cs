using System.Diagnostics;

namespace RecipeShare.Benchmarks.Scripts
{
    public class PerformanceTest
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task Main(string[] args)
        {
            var baseUrl = "https://localhost:44364";
            var endpoint = $"{baseUrl}/api/recipes";

            Console.WriteLine("Starting 500 sequential GET requests...");
            Console.WriteLine($"Target URL: {endpoint}");

            var stopwatch = Stopwatch.StartNew();
            var latencies = new List<long>();

            for (int i = 0; i < 500; i++)
            {
                var requestStopwatch = Stopwatch.StartNew();

                try
                {
                    var response = await client.GetAsync(endpoint);
                    response.EnsureSuccessStatusCode();

                    requestStopwatch.Stop();
                    latencies.Add(requestStopwatch.ElapsedMilliseconds);

                    if ((i + 1) % 100 == 0)
                    {
                        Console.WriteLine($"Completed {i + 1} requests...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request {i + 1} failed: {ex.Message}");
                }
            }

            stopwatch.Stop();

            // Calculate statistics
            var avgLatency = latencies.Average();
            var minLatency = latencies.Min();
            var maxLatency = latencies.Max();
            var totalTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("\n=== Performance Test Results ===");
            Console.WriteLine($"Total requests: 500");
            Console.WriteLine($"Total time: {totalTime} ms");
            Console.WriteLine($"Average latency: {avgLatency:F2} ms");
            Console.WriteLine($"Min latency: {minLatency} ms");
            Console.WriteLine($"Max latency: {maxLatency} ms");
            Console.WriteLine($"Requests per second: {500.0 / (totalTime / 1000.0):F2}");

            // Calculate percentiles
            var sortedLatencies = latencies.OrderBy(x => x).ToList();
            var p50 = sortedLatencies[sortedLatencies.Count / 2];
            var p95 = sortedLatencies[(int)(sortedLatencies.Count * 0.95)];
            var p99 = sortedLatencies[(int)(sortedLatencies.Count * 0.99)];

            Console.WriteLine($"50th percentile: {p50} ms");
            Console.WriteLine($"95th percentile: {p95} ms");
            Console.WriteLine($"99th percentile: {p99} ms");
        }
    }
}
