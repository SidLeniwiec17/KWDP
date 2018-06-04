using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP
{
    public class EcgProvider
    {
        public short[] Signal { get; set; }

        private int start = 0;

        public int offset = 500;

        public EcgProvider(short[] signal)
        {
            this.Signal = signal;
        }

        public short[] GetSignal(int start = 0)
        {
            this.start = start;            

            var result = new short[offset];

            Array.Copy(this.Signal, start, result, 0, this.offset);

            return result;
        }

        public short[] Right()
        {
            var result = new short[offset];

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

        public short[] Left()
        {
            var result = new short[offset];

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
