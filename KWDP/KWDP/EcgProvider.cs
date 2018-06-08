using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP
{
    public class EcgProvider
    {
        public float[] Signal { get; set; }

        private int start = 0;

        public int offset = 500;

        public EcgProvider(float[] signal)
        {
            this.Signal = signal;
        }

        public float[] GetSignal(int start = 0)
        {
            this.start = start;            

            var result = new float[offset];

            Array.Copy(this.Signal, start, result, 0, this.offset);

            return result;
        }

        public float[] Right()
        {
            var result = new float[offset];

            var newStart = start + offset;

            if(newStart + offset > this.Signal.Length)
            {
                newStart = start;
            }

            else
            {
                this.start = newStart;
            }

            Array.Copy(this.Signal, newStart, result, 0, offset);

            return result;
        }

        public float[] Left()
        {
            var result = new float[offset];

            var newStart = start - offset;

            if (newStart < 0)
            {                
                this.start = 0;
            }

            else
            {
                this.start = newStart;
            }

            Array.Copy(this.Signal, this.start, result, 0, offset);

            return result;
        }





    }
}
