using NUnit.Framework;
using UnityEngine;

namespace LordBreakerX.Tables
{
    [System.Serializable]
    public sealed class WeightedEntry<T> where T : class
    {
        [SerializeField]
        private T _value;

        [SerializeField]
        [Min(0)]
        private int _weight;

        public T Value => _value;
        public int Weight => _weight;

        public WeightedEntry(T value, int weight)
        {
            _value = value;
            _weight = weight;
        }

        public WeightedEntry()
        {
            _value = default;
            _weight = 1;
        }

        public static implicit operator WeightedEntry<object>(WeightedEntry<T> entry)
        {
            return new WeightedEntry<object>(entry.Value, entry.Weight);
        }
    }
}
