using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.BiDictionary
{
    public class BiDictionary<TKey, TValue> : IBiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _forward = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TValue, TKey> _reverse = new Dictionary<TValue, TKey>();

        public BiDictionary()
        {
            KeyMap = _forward;
            ValueMap = _reverse;
        }

        public Dictionary<TKey, TValue> KeyMap { get; private set; }
        public Dictionary<TValue, TKey> ValueMap { get; private set; }

        public int Count() => _forward.Count;

        public TValue this[TKey key]
        {
            get => KeyMap[key];
            set => KeyMap[key] = value;
        }

        public TKey this[TValue key]
        {
            get => ValueMap[key];
            set => ValueMap[key] = value;
        }
        
        public void Add(TKey key, TValue value)
        {
            if (key != null && value != null)
            {
                _forward.Add(key, value);
                _reverse.Add(value, key);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _reverse.ContainsKey(value);
        }

        public bool RemoveKey(TKey key)
        {
            var value = _forward[key];
            return value != null && _reverse.ContainsKey(value)
                && _reverse.Remove(value) && _forward.Remove(key);
        }

        public bool RemoveValue(TValue value)
        {
            var key = _reverse[value];
            return key != null && _forward.ContainsKey(key)
                && _forward.Remove(key) && _reverse.Remove(value);
        }

        public IEnumerator<KeyValuePair<TValue, TKey>> GetValueEnumerator()
        {
            return _reverse.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public bool TryGetValue(TKey key, out TValue value) => _forward.TryGetValue(key, out value);
        public bool TryGetValue(TValue key, out TKey value) => _reverse.TryGetValue(key, out value);
    }
}
