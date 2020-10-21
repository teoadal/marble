using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Marble.Benchmarks.Common
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class Dictionaries
    {
        [Params(10, 100, 1000)] 
        public int Count;

        private KeyValuePair<Type, int>[] _data;

        private ConcurrentDictionary<Type, int> _concurrent;
        private Dictionary<Type, int> _regular;

        [GlobalSetup]
        public void Init()
        {
            _data = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Take(Count)
                .Select(type => new KeyValuePair<Type, int>(type, type.GetHashCode() % 100))
                .ToArray();

            _concurrent = new ConcurrentDictionary<Type, int>();
            _regular = new Dictionary<Type, int>();
        }

        [Benchmark(Baseline = true)]
        public int Concurrent()
        {
            var sum = 0;
            foreach (var (key, value) in _data)
            {
                sum += _concurrent.GetOrAdd(key, value);
            }

            return sum;
        }

        [Benchmark]
        public int Regular()
        {
            var dictionary = _regular;
            var sum = 0;
            foreach (var (key, value) in _data)
            {
                if (dictionary.TryGetValue(key, out var exists))
                {
                    sum += exists;
                    continue;
                }

                var lockTaken = false;
                Monitor.Enter(dictionary, ref lockTaken);

                dictionary[key] = value;

                if (lockTaken) Monitor.Exit(dictionary);

                sum += value;
            }

            return sum;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _concurrent.Clear();
            _concurrent = new ConcurrentDictionary<Type, int>();

            _regular.Clear();
            _regular = new Dictionary<Type, int>();
        }
    }
}