// <copyright file="MathematicsExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Common.Extensions
{
    using Unity.Mathematics;

    /// <summary>
    /// Extensions for types within the <see cref="Unity.Mathematics"/> namespace.
    /// </summary>
    public static class MathematicsExtensions
    {
        /// <summary>
        /// Floor a <see cref="float"/> to an integer.
        /// </summary>
        /// <param name="v">The <see cref="float"/> to floor.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int FloorToInt(this float v) => (int)math.floor(v);

        /// <summary>
        /// Floor a <see cref="float2"/> to integers.
        /// </summary>
        /// <param name="f2">The <see cref="float2"/> to floor.</param>
        /// <returns>A <see cref="int2"/>.</returns>
        public static int2 FloorToInt(this float2 f2) => (int2)math.floor(f2);

        /// <summary>
        /// Floor a <see cref="float3"/> to integers.
        /// </summary>
        /// <param name="f3">The <see cref="float3"/> to floor.</param>
        /// <returns>A <see cref="int3"/>.</returns>
        public static int3 FloorToInt(this float3 f3) => (int3)math.floor(f3);

        /// <summary>
        /// Returns the euler angles of the right axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The right axis in euler angles.</returns>
        public static float3 Right(this quaternion q) => new float3x3(q).c0;

        /// <summary>
        /// Returns the euler angles of the up axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The up axis in euler angles.</returns>
        public static float3 Up(this quaternion q) => new float3x3(q).c1;

        /// <summary>
        /// Returns the euler angles of the forward axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The forward axis in euler angles.</returns>
        public static float3 Forward(this quaternion q) => new float3x3(q).c2;
    }
}
