using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP.Objects
{
    public class Patient
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public int Age { get; set; }
        public string Pesel { get; set; }

        public Patient()
        {

        }

        public Patient(string fName, string sName, int age, string pesel)
        {
            FirstName = fName;
            SurName = sName;
            Age = age;
            Pesel = pesel;
        }
    }
}
