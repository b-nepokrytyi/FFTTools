﻿using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace FFTTools
{
    public class BinomialBuilder : BuilderBase, IBuilder
    {
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        public static void GetLongs(long[] array, long x = 1)
        {
            if (array.Length > 0) array[0] = 1;
            for (var i = 1; i < array.Length; i++)
                for (var j = i; j-- > 0;)
                    array[j + 1] += x*array[j];
        }

        public static void GetDoubles(double[] array, double x = 1.0)
        {
            var complex = new Complex[array.Length];
            if (array.Length > 0) complex[0] = Complex.One;
            if (array.Length > 1) complex[1] = x;
            if (array.Length > 0)
            {
                Fourier(complex, FourierDirection.Forward);
                complex = complex.Select(
                    value => Complex.Pow(value, array.Length - 1)/array.Length).ToArray();
                Fourier(complex, FourierDirection.Backward);
            }
            var index = 0;
            foreach (var value in complex) array[index++] = value.Magnitude;
        }

        public Bitmap ToBitmap(Bitmap source)
        {
            throw new NotImplementedException();
        }
    }
}