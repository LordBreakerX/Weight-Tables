using System.Collections.Generic;
using UnityEngine;

namespace LordBreakerX.Tables
{
    [System.Serializable]
    public class WeightTable<T> where T : class
    {
        [SerializeField]
        private List<WeightedEntry<T>> _weightedEntries = new List<WeightedEntry<T>>();

        [SerializeField]
        private int _totalWeight;

        public bool HasEntries { get { return _weightedEntries.Count > 0; } }

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

        public void RemoveEntry(WeightedEntry<T> weightedEntry)
        {
            if (weightedEntry != null && _weightedEntries.Contains(weightedEntry))
            {
                _weightedEntries.Remove(weightedEntry);
                _totalWeight -= weightedEntry.Weight;
            }
        }

        protected virtual void UpdateTotalWeight()
        {
            _totalWeight = 0;
            List<WeightedEntry<T>> entries = GetEntries(); 

            foreach (WeightedEntry<T> entry in entries)
            {
                _totalWeight += entry.Weight;
            }
        }

        public virtual List<WeightedEntry<T>> GetEntries()
        {
            return _weightedEntries;
        }

        public virtual T GetRandomEntry()
        {
            int weight = Random.Range(0, _totalWeight + 1);
            List<WeightedEntry<T>> entries = GetEntries();

            foreach (WeightedEntry<T> entry in entries)
            {
                if (weight <= entry.Weight)
                {
                    OnEntrySelected(entry.Value);
                    return entry.Value;
                }

                weight -= entry.Weight;
            }
            return null;
        }

        public void RemoveEntry(T entryValue)
        {
            WeightedEntry<T> entryToRemove = null;

            foreach(WeightedEntry<T> entry in _weightedEntries)
            {
                if (entry.Value == entryValue)
                {
                    entryToRemove = entry;
                    break;
                }
            }

            if (entryToRemove != null) RemoveEntry(entryToRemove);
        }

        protected virtual void OnEntrySelected(T value) { }
    }
}
