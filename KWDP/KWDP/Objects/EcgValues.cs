using System;
using System.Globalization;

namespace KWDP.View
{
    internal class EcgValues
    {
        public int SampleNumber { get; set; }
        
        public short Signal { get; set; }

        public static EcgValues FromCsv(string csvLine)
        {
            string[] values = csvLine.Split('\t');

            EcgValues ecg = new EcgValues();
            ecg.SampleNumber = int.Parse(values[0]);
            ecg.Signal = ((short)float.Parse(values[1], CultureInfo.InvariantCulture));
            return ecg;
        }
    }
}