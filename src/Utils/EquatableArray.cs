using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RaptorOS.Utils;

public struct EquatableArray<T>(T[] array) : IEquatable<EquatableArray<T>>, IEnumerable<T>
{
    public EquatableArray(int size)
        : this(new T[size]) { }

    public EquatableArray()
        : this([]) { }

    private T[] _array = array;
    private int _count = 0;

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) =>
        left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) =>
        !(left == right);

    public static implicit operator EquatableArray<T>(T[] array) => new(array);

    public static implicit operator EquatableArray<T>(List<T> list) => new([.. list]);

    public readonly ref T this[int index] => ref _array[index];

    public readonly int Length => _array.Length;

    public readonly bool Equals(EquatableArray<T> other) =>
        _array.Length == other._array.Length && _array.AsSpan().SequenceEqual(other._array);

    public override readonly bool Equals(object? obj) =>
        obj is EquatableArray<T> other && Equals(other);

    public override readonly int GetHashCode() => _array.GetHashCode();

    public readonly IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        if (_count == _array.Length)
            Array.Resize(ref _array, _count * 2);

        _array[_count] = item;
        _count++;
    }

    public readonly bool All(Func<T, bool> predicate) => _array.Take(_count).All(predicate);
}
