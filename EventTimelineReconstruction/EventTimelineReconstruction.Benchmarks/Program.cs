using BenchmarkDotNet.Running;
using EventTimelineReconstruction.Benchmarks;

//BenchmarkRunner.Run<EventsImporterBenchmarks>();
//BenchmarkRunner.Run<ParallelFullBenchmark>();
BenchmarkRunner.Run<WorkSaverBenchmarks>();