﻿using System;
using System.Globalization;

namespace KWDP.View
{
    internal class EcgValues
    {
        public int SampleNumber { get; set; }
        
        public float Signal { get; set; }

        public static EcgValues FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(' ');

            EcgValues ecg = new EcgValues();
            ecg.SampleNumber = int.Parse(values[0]);
            ecg.Signal = (float.Parse(values[1], CultureInfo.InvariantCulture));
            return ecg;
        }
    }
}