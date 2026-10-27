// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace ImageDull.ImageSharp.Metadata.Profiles.Icc
{
    /// <summary>
    /// Provides methods to read ICC data types
    /// </summary>
    internal sealed partial class IccDataReader
    {
        /// <summary>
        /// The data that is read
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// The current reading position
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="IccDataReader"/> class.
        /// </summary>
        /// <param name="data">The data to read</param>
        public IccDataReader(byte[] data)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Gets the length in bytes of the raw data
        /// </summary>
        public int DataLength => this.data.Length;

        /// <summary>
        /// Sets the reading position to the given value
        /// </summary>
        /// <param name="index">The new index position</param>
        public void SetIndex(int index)
        {
            this.currentIndex = Numerics.Clamp(index, 0, this.data.Length);
        }

        /// <summary>
        /// Returns the current <see cref="currentIndex"/> without increment and adds the given increment
        /// </summary>
        /// <param name="increment">The value to increment <see cref="currentIndex"/></param>
        /// <returns>The current <see cref="currentIndex"/> without the increment</returns>
        private int AddIndex(int increment)
        {
            int tmp = this.currentIndex;
            this.currentIndex += increment;
            return tmp;
        }

        /// <summary>
        /// Ensures that the remaining data is large enough to hold the declared number of elements.
        /// Must be called before allocating memory based on a count read from the profile,
        /// so that a small malformed profile can't request a huge allocation.
        /// </summary>
        /// <param name="count">The declared number of elements.</param>
        /// <param name="minElementSize">The minimum size in bytes of a single element.</param>
        /// <exception cref="InvalidIccProfileException">The remaining data is too small.</exception>
        private void EnsureAvailable(long count, int minElementSize)
        {
            long remaining = Math.Max(0, this.data.Length - (long)this.currentIndex);
            if (count < 0 || count > remaining / minElementSize)
            {
                throw new InvalidIccProfileException("The declared data exceeds the size of the profile.");
            }
        }

        /// <summary>
        /// Gets the number of array elements of a tag with the given size, validating the size
        /// against the available data.
        /// </summary>
        /// <param name="size">The size of the tag in bytes, including the 8 byte tag header.</param>
        /// <param name="elementSize">The size in bytes of a single element.</param>
        /// <returns>The number of elements.</returns>
        private uint GetArrayCount(uint size, int elementSize)
        {
            if (size < 8)
            {
                throw new InvalidIccProfileException($"Invalid tag size of {size}.");
            }

            uint count = (size - 8) / (uint)elementSize;
            this.EnsureAvailable(count, elementSize);
            return count;
        }

        /// <summary>
        /// Calculates the 4 byte padding and adds it to the <see cref="currentIndex"/> variable
        /// </summary>
        private void AddPadding()
        {
            this.currentIndex += this.CalcPadding();
        }

        /// <summary>
        /// Calculates the 4 byte padding
        /// </summary>
        /// <returns>the number of bytes to pad</returns>
        private int CalcPadding()
        {
            int p = 4 - (this.currentIndex % 4);
            return p >= 4 ? 0 : p;
        }

        /// <summary>
        /// Gets the bit value at a specified position
        /// </summary>
        /// <param name="value">The value from where the bit will be extracted</param>
        /// <param name="position">Position of the bit. Zero based index from left to right.</param>
        /// <returns>The bit value at specified position</returns>
        private bool GetBit(byte value, int position)
        {
            return ((value >> (7 - position)) & 1) == 1;
        }
    }
}
