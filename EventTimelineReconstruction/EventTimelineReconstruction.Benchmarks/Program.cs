using BenchmarkDotNet.Running;
using EventTimelineReconstruction.Benchmarks;

BenchmarkRunner.Run<EventsImporterBenchmarks>();
BenchmarkRunner.Run<WorkSaverBenchmarks>();
BenchmarkRunner.Run<WorkLoaderBenchmarks>();
BenchmarkRunner.Run<HighLevelEventsBenchmarks>();
BenchmarkRunner.Run<LowLevelEventsBenchmarks>();
BenchmarkRunner.Run<HighLevelArtefactsBenchmarks>();
BenchmarkRunner.Run<LowLevelArtefactsBenchmarks>();