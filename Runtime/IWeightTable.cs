using System.Collections.Generic;
using UnityEngine;

namespace LordBreakerX.Tables
{
    public interface IWeightTable<T> where T : class
    {
        public List<WeightedEntry<T>> Entries { get; }

        public bool HasEntries { get; }

        public int TotalWeight { get; }

        public void AddEntry(WeightedEntry<T> weightedEntry);

        public void RemoveEntry(WeightedEntry<T> weightedEntry);

        public void AddEntry(T value, int weight);

        public T GetRandomEntry();

        public void UpdateTotalWeight();
    }

    public interface IWeightTable
    {
        public IReadOnlyList<WeightedEntry<object>> Entries { get; }

        public bool HasEntries { get; }

        public void AddEntry(WeightedEntry<object> weightedEntry);

        public void RemoveEntry(WeightedEntry<object> weightedEntry);

        public void AddEntry(object value, int weight);

        public object GetRandomEntry();

        public void UpdateTotalWeight();
    }
}
