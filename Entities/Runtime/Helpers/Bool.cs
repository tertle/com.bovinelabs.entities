// <copyright file="Bool.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Burst currently does not support <see cref="bool"/> so this is a simple wrapper that acts like bool.
    /// </summary>
    [Serializable]
    public struct Bool : IEquatable<Bool>
    {
        /// <summary>
        /// The value of the Bool. Should only be used by serializer.
        /// </summary>
        [FormerlySerializedAs("Value")]
        [SerializeField]
        private byte value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bool"/> struct.
        /// </summary>
        /// <param name="value">The default value.</param>
        public Bool(bool value)
        {
            this.value = value ? (byte)1 : (byte)0;
        }

        public static implicit operator Bool(bool b)
        {
            return new Bool(b);
        }

        public static implicit operator bool(Bool b)
        {
            return b.value != 0;
        }

        public static bool operator ==(Bool left, Bool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bool left, Bool right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(Bool other)
        {
            return this.value == other.value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj != null && obj is Bool b && this.Equals(b);
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Can't be readonly due to required inspector support support")]
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return (this.value != 0).ToString();
        }
    }
}