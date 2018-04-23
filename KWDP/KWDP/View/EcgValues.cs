using System;

namespace KWDP.View
{
    internal class EcgValues
    {
        public int SampleNumber { get; set; }

        public float FirstCanal { get; set; }

        public float SecondCanal { get; set; }

        public static EcgValues FromCsv(string csvLine)
        {
            string[] values = csvLine.Split('\t');

            EcgValues ecg = new EcgValues();

            ecg.SampleNumber = int.Parse(values[0]);
            ecg.FirstCanal = float.Parse(values[1]);
            ecg.SecondCanal = float.Parse(values[2]);

            return ecg;
        }
    }
}