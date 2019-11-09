using System;

namespace Intermedium
{
    /// <summary>
    /// Represents a <see cref="void"/> type (<see langword="Nothing"/> in VB.Net).
    /// </summary>
    public readonly struct VoidUnit : IEquatable<VoidUnit>, IComparable<VoidUnit>, IComparable
    {
        /// <summary>
        /// Represents the single instance of <see cref="VoidUnit"/> type.
        /// </summary>
        public static VoidUnit Value { get; } = new VoidUnit();

        /// <summary>
        /// Compares the current instance with another object of the <see cref="VoidUnit"/> type
        /// and returns an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes sort order as obj.
        /// Greater than zero This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(VoidUnit other) => default;

        /// <summary>
        /// Compares the current instance with another object of the <see cref="object"/> type
        /// and returns an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes sort order as obj.
        /// Greater than zero This instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object other) => default;

        /// <summary>
        /// Indicates whether the current instance is equal to another
        /// object of the <see cref="VoidUnit"/> type.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// true if the current instance is equal to the other parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(VoidUnit other) => true;

        /// <summary>
        /// Indicates whether the current instance is equal to another
        /// object of the <see cref="object"/> type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current instance is equal to the other parameter;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object other) => other is VoidUnit;

        /// <summary>
        /// Returns the hash code for this <see cref="VoidUnit"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => default;

        /// <summary>
        /// Returns a string that represents the current <see cref="VoidUnit"/> instance.
        /// </summary>
        /// <returns>A string that represents the current <see cref="VoidUnit"/> instance.</returns>
        public override string ToString() => $"{nameof(Intermedium)}.{nameof(VoidUnit)}";

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator ==(VoidUnit left, VoidUnit right) => left.Equals(right);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator !=(VoidUnit left, VoidUnit right) => !(left == right);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator ==(VoidUnit left, object right) => left.Equals(right);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator !=(VoidUnit left, object right) => !(left == right);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator ==(object left, VoidUnit right) => right.Equals(left);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is not equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator !=(object left, VoidUnit right) => !(left == right);

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <(VoidUnit left, VoidUnit right)
        {
            return left.CompareTo(right) < default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <(VoidUnit left, object right)
        {
            return left.CompareTo(right) < default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <(object left, VoidUnit right)
        {
            return right.CompareTo(left) > default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <=(VoidUnit left, VoidUnit right)
        {
            return left.CompareTo(right) <= default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <=(VoidUnit left, object right)
        {
            return left.CompareTo(right) <= default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is less than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator <=(object left, VoidUnit right)
        {
            return right.CompareTo(left) >= default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >(VoidUnit left, VoidUnit right)
        {
            return left.CompareTo(right) > default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >(VoidUnit left, object right)
        {
            return left.CompareTo(right) > default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >(object left, VoidUnit right)
        {
            return right.CompareTo(left) < default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >=(VoidUnit left, VoidUnit right)
        {
            return left.CompareTo(right) >= default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >=(VoidUnit left, object right)
        {
            return left.CompareTo(right) >= default(int);
        }

        /// <summary>
        /// Determines whether the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>
        /// <see keyword="true"/> if the <paramref name="left"/> value
        /// is greater than or equal to the <paramref name="right"/> value;
        /// otherwise, <see keyword="false"/>.
        /// </returns>
        public static bool operator >=(object left, VoidUnit right)
        {
            return right.CompareTo(left) <= default(int);
        }
    }
}
