using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP.Objects
{
    class EcgCharacteristics
    {
        public double AverageHeartRate { get; set; }
        public double[] BradycardiaMoments { get; set; }
        public double[] TachycardiaMoments { get; set; }
        public double[] PvcMoments { get; set; }
        public double[] PacMoments { get; set; }

        public EcgCharacteristics(double averageHeartRate,
            double[] bradycardiaMoments,
            double[] tachycardiaMoments,
            double[] pvcMoments,
            double[] pacMoments)
        {
            AverageHeartRate = averageHeartRate;
            BradycardiaMoments = bradycardiaMoments;
            TachycardiaMoments = tachycardiaMoments;
            PvcMoments = pvcMoments;
            PacMoments = pacMoments;
        }
    }
}
