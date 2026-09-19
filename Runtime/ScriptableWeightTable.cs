using System.Collections.Generic;
using UnityEngine;

namespace LordBreakerX.Tables
{
    public abstract class ScriptableWeightTable<T> : ScriptableObject, IWeightTable<T> where T : class
    {
        [SerializeField]
        private WeightTable<T> _table = new WeightTable<T>();

        public bool HasEntries => _table.HasEntries;

        public List<WeightedEntry<T>> Entries => _table.Entries;

        public int TotalWeight => _table.TotalWeight;

        public void AddEntry(WeightedEntry<T> weightedEntry) => _table.AddEntry(weightedEntry);

        public void AddEntry(T value, int weight) => _table.AddEntry(value, weight);

        public void RemoveEntry(WeightedEntry<T> weightedEntry) => _table.RemoveEntry(weightedEntry);

        public T GetRandomEntry() => _table.GetRandomEntry();

        public void UpdateTotalWeight() => _table.UpdateTotalWeight();
    }

}