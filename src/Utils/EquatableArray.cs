using System;
using System.Collections;
using System.Collections.Generic;
using RaptorOS.Utils.Extensions;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils;

public struct EquatableArray<T>(T[] array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
{
    public EquatableArray(int capacity)
        : this(new T[capacity == 0 ? 1 : capacity])
    {
    }

    public EquatableArray()
        : this(capacity: 4)
    {
    }

    private T[] _array = array;

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) =>
        left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) =>
        !(left == right);

    public static implicit operator EquatableArray<T>(T[] array) => new(array);

    public static implicit operator EquatableArray<T>(List<T> list) => new([.. list]);

    public readonly ref T this[int index] => ref _array[index];

    public int Capacity => _array.Length;

    public int Count { get; private set; }

    public readonly bool Equals(EquatableArray<T> other) =>
        Count == other.Count && _array.AsSpan().SequenceEqual(other._array);

    public readonly override bool Equals(object? obj) =>
        obj is EquatableArray<T> other && Equals(other);

    public readonly override int GetHashCode()
    {
        HashCode hashCode = new();
        for (int i = 0; i < Count; i++)
            hashCode.Add(_array[i]);

        return hashCode.ToHashCode();
    }

    public readonly IEnumerator<T> GetEnumerator() => new EquatableArrayEnumerator<T>(this);

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        if (Count == Capacity)
            Array.Resize(ref _array, Capacity * 2 );
        
        _array[Count] = item;
        Count++;
    }


    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
            Add(item);
    }

    public int IndexOf(T item)
    {
        Logger.LogInfo("IndexOf start");
    
        for (int i = 0; i < Count; i++)
        {
            Logger.LogInfo($"rComparing index {i}");

            if (item is not IEquatable<T> equatable  || !equatable.Equals(_array[i])) continue;
            Logger.LogInfo($"Found match at index {i}");
            return i;
        }
    
        Logger.LogInfo("Item not found");
        return -1;
    }
}

internal class EquatableArrayEnumerator<T>(EquatableArray<T> array) : IEnumerator<T>
{
    private int _index;

    public bool MoveNext()
    {
        if (_index >= array.Count)
            return false;

        Current = array[_index];
        _index++;
        return true;
    }

    public void Reset() => _index = 0;

    public T Current { get; private set; }

    object? IEnumerator.Current => Current!;

    public void Dispose()
    {
    }
}