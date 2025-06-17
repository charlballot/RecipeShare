using BenchmarkDotNet.Running;
using RecipeShare.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RecipeShareBenchmark>();
    }
}