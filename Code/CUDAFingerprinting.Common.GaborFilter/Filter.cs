﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUDAFingerprinting.Common.GaborFilter
{
    class Filter
    {
        public double[,] Matrix;

        public Filter(int size, double angle)
        {
            Matrix = new double[size, size];

            var aCos = Math.Cos(angle);
            var aSin = Math.Sin(angle);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Matrix[i, j] = Math.Exp(-0.5*(Math.Pow(i * aCos, 2) / 16 + Math.Pow(j * aSin, 2) / 16)) * Math.Cos(2 * Math.PI * aCos / 9);
                }
            }
        }

        public void WriteMatrix()
        {
            int size = Matrix.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write("| {0:##.###} ", Matrix[i,j]);
                }

                Console.WriteLine("|");
            }
        }
    }
}