```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
11th Gen Intel Core i5-11400H 2.70GHz (Max: 2.69GHz), 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.301
  [Host]     : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NTRUNJ : .NET 9.0.6 (9.0.625.26613), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=5  WarmupCount=3  

```
| Method                          | Mean | Error |
|-------------------------------- |-----:|------:|
| GetAllRecipes                   |   NA |    NA |
| GetRecipesWithDietaryFilter     |   NA |    NA |
| GetRecipesWithCookingTimeFilter |   NA |    NA |
| GetRecipesWithTextSearch        |   NA |    NA |
| GetSingleRecipe                 |   NA |    NA |
| CreateRecipe                    |   NA |    NA |

Benchmarks with issues:
  RecipeShareBenchmark.GetAllRecipes: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
  RecipeShareBenchmark.GetRecipesWithDietaryFilter: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
  RecipeShareBenchmark.GetRecipesWithCookingTimeFilter: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
  RecipeShareBenchmark.GetRecipesWithTextSearch: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
  RecipeShareBenchmark.GetSingleRecipe: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
  RecipeShareBenchmark.CreateRecipe: Job-NTRUNJ(IterationCount=5, WarmupCount=3)
