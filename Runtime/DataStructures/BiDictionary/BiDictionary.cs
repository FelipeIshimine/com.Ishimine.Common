﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DataStructures.BiDictionary
{
    public class BiDictionary<TKey, TValue> : IBiDictionary<TKey, TValue>
    {
        [ShowInInspector] private readonly Dictionary<TKey, TValue> _forward = new Dictionary<TKey, TValue>();
        [ShowInInspector] private readonly Dictionary<TValue, TKey> _backward = new Dictionary<TValue, TKey>();

        public Dictionary<TKey, TValue> KeyMap => _forward;
        public Dictionary<TValue, TKey> ValueMap => _backward;

        public int Count() => _forward.Count;

        public TValue this[TKey key]
        {
            get => KeyMap[key];
            set => Add(key,value);
        }

        public TKey this[TValue key]
        {
            get => ValueMap[key];
            set => Add(value,key);
        }
        
        public void Add(TKey key, TValue value)
        {
            _forward.Add(key, value);
            _backward.Add(value, key);
        }

        public bool ContainsKey(TKey key) => _forward.ContainsKey(key);

        public bool ContainsValue(TValue value) => _backward.ContainsKey(value);

        public bool RemoveKey(TKey key) => _backward.Remove(_forward[key]) && _forward.Remove(key);

        public bool RemoveValue(TValue value)
        {
            var key = _backward[value];

            bool a = _forward.Remove(key);
            bool b = _backward.Remove(value);

            Debug.Log($"A:{a} B:{b}");

            return a && b;
            //return key != null && _forward.Remove(key) && _reverse.Remove(value);
        }

        public IEnumerator<KeyValuePair<TValue, TKey>> GetValueEnumerator() => _backward.GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _forward.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _forward.GetEnumerator();

        public void Clear()
        {
            _forward.Clear();
            _backward.Clear();
        }

        public bool TryGetValue(TKey key, out TValue value) => _forward.TryGetValue(key, out value);
        public bool TryGetValue(TValue key, out TKey value) => _backward.TryGetValue(key, out value);
    }
}
