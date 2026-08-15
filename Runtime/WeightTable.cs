using System.Collections.Generic;
using UnityEngine;

namespace LordBreakerX.Tables
{
    // Generic Weight Table

    [System.Serializable]
    public sealed class WeightTable<T> : IWeightTable<T> where T : class
    {
        [SerializeField]
        private List<WeightedEntry<T>> _weightedEntries = new List<WeightedEntry<T>>();

        [SerializeField]
        private int _totalWeight;

        public bool HasEntries { get { return _weightedEntries.Count > 0; } }

        public List<WeightedEntry<T>> Entries { get => _weightedEntries; }

        public int TotalWeight { get { return _totalWeight; } }

        public WeightTable()
        {
        }

        public void AddEntry(WeightedEntry<T> weightedEntry)
        {
            if (weightedEntry != null && !_weightedEntries.Contains(weightedEntry))
            {
                _weightedEntries.Add(weightedEntry);
                _totalWeight += weightedEntry.Weight;
            }
        }

        public void AddEntry(T value, int weight)
        {
            WeightedEntry<T> entry = new WeightedEntry<T>(value, weight);
            AddEntry(entry);
        }

        public void RemoveEntry(WeightedEntry<T> weightedEntry)
        {
            if (weightedEntry != null && _weightedEntries.Contains(weightedEntry))
            {
                _weightedEntries.Remove(weightedEntry);
                _totalWeight -= weightedEntry.Weight;
            }
        }

        public T GetRandomEntry()
        {
            int weight = Random.Range(0, _totalWeight + 1);

            foreach (WeightedEntry<T> entry in _weightedEntries)
            {
                if (weight <= entry.Weight)
                {
                    return entry.Value;
                }

                weight -= entry.Weight;
            }
            return null;
        }

        public void UpdateTotalWeight()
        {
            _totalWeight = 0;

            foreach (WeightedEntry<T> entry in _weightedEntries)
            {
                _totalWeight += entry.Weight;
            }
        }
    }

}
