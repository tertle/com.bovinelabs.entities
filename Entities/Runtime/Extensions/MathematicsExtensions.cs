// <copyright file="MathematicsExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Unity.Mathematics;
    using UnityEngine.Assertions;

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

        /// <summary>
        /// Gets the position from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The position.</returns>
        public static float3 GetPosition(this float4x4 matrix) => matrix.c3.xyz;

        // TODO this only works when scale = 1, need better solution
        /// <summary>
        /// Gets the rotation from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The rotation.</returns>
        public static quaternion GetRotation(this float4x4 matrix)
        {
            var scale = GetScale(matrix);
            var inverted = Invert(scale);
            matrix = matrix.ScaleBy(inverted);
            return new quaternion(matrix);
        }

        /// <summary>
        /// Gets the scale from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The lossy scale.</returns>
        public static float3 GetScale(this float4x4 matrix) => new float3(
            math.length(matrix.c0.xyz),
            math.length(matrix.c1.xyz),
            math.length(matrix.c2.xyz));

        /// <summary>
        /// Gets the squared scale from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The lossy scale.</returns>
        public static float3 GetScaleSqr(this float4x4 matrix) => new float3(
            math.lengthsq(matrix.c0.xyz),
            math.lengthsq(matrix.c1.xyz),
            math.lengthsq(matrix.c2.xyz));

        /// <summary>
        /// Scales a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The scaled matrix.</returns>
        public static float4x4 ScaleBy(this float4x4 matrix, float3 scale)
        {
            return math.mul(matrix, float4x4.Scale(scale));
        }

        /// <summary>
        /// Gets the largest value of a scale from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The largest scale.</returns>
        public static float LargestScale(this float4x4 matrix)
        {
            var scaleSqr = matrix.GetScaleSqr();
            float largestScaleSqr = math.cmax(scaleSqr);
            return math.sqrt(largestScaleSqr);
        }

        /// <summary>
        /// Transforms a position by this matrix.
        /// </summary>
        /// <remarks>
        /// Mimics <see cref="UnityEngine.Matrix4x4.MultiplyPoint3x4"/>.
        /// </remarks>
        /// <param name="matrix">The matrix.</param>
        /// <param name="point">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Using same naming as Matrix4x4.MultiplyPoint3x4")]
        public static float3 MultiplyPoint3x4(this float4x4 matrix, float3 point)
        {
            return math.mul(matrix, new float4(point, 1.0f)).xyz;
        }

        /// <summary>
        /// Safely inverts a vector. 0 returns 0.
        /// </summary>
        /// <param name="f">The vector to invert.</param>
        /// <returns>The inverse of <see cref="f"/>.</returns>
        public static float3 InvertSafe(float3 f)
        {
            var x = Math.Abs(f.x) < float.Epsilon ? 0 : 1 / f.x;
            var y = Math.Abs(f.y) < float.Epsilon ? 0 : 1 / f.y;
            var z = Math.Abs(f.z) < float.Epsilon ? 0 : 1 / f.z;

            return new float3(x, y, z);
        }

        /// <summary>
        /// Inverts a vector.
        /// </summary>
        /// <param name="f">The vector to invert.</param>
        /// <returns>The inverse of <see cref="f"/>.</returns>
        public static float3 Invert(float3 f)
        {
            Assert.IsTrue(Math.Abs(f.x) > float.Epsilon);
            Assert.IsTrue(Math.Abs(f.y) > float.Epsilon);
            Assert.IsTrue(Math.Abs(f.z) > float.Epsilon);

            return new float3(1 / f.x, 1 / f.y, 1 / f.z);
        }
    }
}