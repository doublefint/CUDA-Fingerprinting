﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CUDAFingerprinting.Common;
using System.Diagnostics;
using CUDAFingerprinting.FeatureExtraction.Minutiae;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUDAFingerprinting.FeatureExtraction.Tests
{
    [TestClass]
    public class FengMinutiaDescriptorTest
    {
        private static List<Minutia> readMinutiae(string s)
        {
            float[] mas = s.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
            
            int n = (int)mas[0];

            List<Minutia> mins = new List<Minutia>();
            Minutia m = new Minutia();
            
            for (int i = 1; i <= 3*n; i +=3)
            {
                m.X = (int)mas[i];
                m.Y = (int)mas[i + 1];
                m.Angle = mas[i + 2];

                if (m.Angle > Math.PI)
                {
                    m.Angle -= 2 * (float)Math.PI; 
                }
                else if (m.Angle < -Math.PI)
                {
                    m.Angle += 2 * (float)Math.PI; 
                }

                mins.Add(m);
            }

            return mins;
        }

        public static void getImgSize(Bitmap map, ref int heigth, ref int width)
        {
            int[,] img = ImageHelper.LoadImage<int>(map);
            heigth = img.GetLength(1);
            width = img.GetLength(0);
        }
        [TestMethod, Description("Compare two handwriting descriptors")]
        public void DescriptorsCompareTestOneToOne()
        {
            int i;
            float s;
            float eps = 0.05F;
            float check = 1.0F;
            int radius = 10;
            int height = 20;
            int width = 20;
            const int line = 5;
            List<Minutia> ms1 = new List<Minutia>();
            List<Minutia> ms2 = new List<Minutia>();
            Minutia c1, c2;
            c1.X = 0;
            c1.Y = 0;
            c1.Angle = 0.0F;
            c2.X = 6;
            c2.Y = 6;
            c2.Angle = (float)Math.PI;
            for (i = 0; i < line; i++)
            {
                Minutia m1 = new Minutia();
                Minutia m2 = new Minutia();
                m1.X =1 + i + c1.X;
                m1.Y =1 + i + c1.Y;
                m1.Angle = 0.5F * (float)Math.PI;
                m2.X =-1 - i + c2.X;
                m2.Y =-1 - i + c2.Y;
                m2.Angle = 1.5F * (float)Math.PI;
                ms1.Add(m1);
                ms2.Add(m2);
                
            }
            Descriptor desc1 = new Descriptor(ms1, c1);
            Descriptor desc2 = new Descriptor(ms2, c2);

            s = FengMinutiaDescriptor.MinutiaCompare(desc1, desc2, radius, height, width);
            Assert.IsTrue(Math.Abs(s - check) < eps);
        }

        private bool cmp(List<Minutia> mins1, List<Minutia> mins2, bool expected)
        {
            int heigth = 0;
            int width = 0;
            getImgSize(Resources.minutia1_11, ref heigth, ref width);

            int radius = 70;
            float eps = 0.01F;


            List<Descriptor> desc1 = DescriptorBuilder.BuildDescriptors(mins1, radius);
            List<Descriptor> desc2 = DescriptorBuilder.BuildDescriptors(mins2, radius);
            bool flag = expected;
            var s = FengMinutiaDescriptor.DescriptorsCompare(desc1, desc2, radius, heigth, width);


            System.IO.StreamWriter write = new System.IO.StreamWriter("D:\\test1.txt");

            for (int i = 0; i < s.GetLength(0); ++i)
            {
                for (int j = 0; j < s.GetLength(1); ++j)
                {
                    write.Write("{0:0.00}", Math.Round(s[i, j], 2));
                    write.Write(" ");
                    //cmp in progress
                }
                write.WriteLine();
            }

            var res = MinutiaeMatching.MatchMinutiae(s, mins1, mins2);
            
            foreach (List<Tuple<int, int>> subList in res)
            {
                write.WriteLine(subList.Count);
                foreach (Tuple<int, int> item in subList)
                {
                    write.Write("(" + item.Item1 + ", " + item.Item2 + ") ");
                }
                write.WriteLine();
            }

            write.Close();
            return flag == expected;
        }

        [TestMethod, Description("Compare set of descriptors with itself")]
        public void DescriptorsCompareTestManyToManyOneFingerprint()
        {
            bool cmpres;
            List<Minutia> mins1 = readMinutiae(Resources.minutia1_1);
            List<Minutia> mins2 = readMinutiae(Resources.minutia1_1);
            cmpres = cmp(mins1, mins2, true);
            Assert.IsTrue(cmpres);
        }
        [TestMethod, Description("Compare sets of descriptors from different fingers")]
        public void DescriptorsCompareTestManyToManyDifferentFingers()
        {
            bool cmpres;
            List<Minutia> mins1 = readMinutiae(Resources.minutia1_1);
            List<Minutia> mins2 = readMinutiae(Resources.minutia2_2);
            cmpres = cmp(mins1, mins2, false);
            Assert.IsTrue(cmpres);
        }

        [TestMethod, Description("Compare sets of descriptors from different fingerprints of one finger")]
        public void DescriptorsCompareTestManyToManyDifferentFingerprints1()
        {
            bool cmpres;
            List<Minutia> mins1 = readMinutiae(Resources.minutia2_6);
            List<Minutia> mins2 = readMinutiae(Resources.minutia2_2);
            cmpres = cmp(mins1, mins2, true);
            Assert.IsTrue(cmpres);
        }

        [TestMethod, Description("Compare sets of descriptors from different fingerprints of one finger")]
        public void DescriptorsCompareTestManyToManyDifferentFingerprints2()
        {
            bool cmpres;
            List<Minutia> mins1 = readMinutiae(Resources.minutia2_6);
            List<Minutia> mins2 = readMinutiae(Resources.minutia2_3);
            cmpres = cmp(mins1, mins2, true);
            Assert.IsTrue(cmpres);
        }
    }
}