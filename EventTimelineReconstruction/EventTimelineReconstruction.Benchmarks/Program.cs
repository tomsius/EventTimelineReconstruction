using BenchmarkDotNet.Running;
using EventTimelineReconstruction.Benchmarks;

// Paziureti ar IEnumerable .Count panaudojimas inicializuoti List dydziui pakeicia greitaveika
BenchmarkRunner.Run<EventsImporterBenchmarks>();
//BenchmarkRunner.Run<ForeachInsideAsyncFullBenchmark>();
//BenchmarkRunner.Run<ParallelFullBenchmark>();
//BenchmarkRunner.Run<WorkSaverBenchmarks>();