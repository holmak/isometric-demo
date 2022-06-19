using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Index2 : IEquatable<Index2>
{
    public int X, Y;

    public Index2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return String.Format("{0}, {1}", X, Y);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        Index2? other = obj as Index2?;
        return other != null && Equals(other.Value);
    }

    public bool Equals(Index2 other)
    {
        return X == other.X && Y == other.Y;
    }

    public static bool operator ==(Index2 a, Index2 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Index2 a, Index2 b)
    {
        return !a.Equals(b);
    }

    public static Index2 operator +(Index2 a, Index2 b)
    {
        return new Index2(a.X + b.X, a.Y + b.Y);
    }

    public static readonly Index2[] NeigborOffsets = new[]
    {
        new Index2(-1, 0),
        new Index2(1, 0),
        new Index2(0, -1),
        new Index2(0, 1),
    };
}
