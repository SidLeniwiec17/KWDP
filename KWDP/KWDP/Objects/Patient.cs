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
        public int Gender { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Ecg_Id { get; set; }

        public Patient()
        {

        }

        public Patient(string fName, string sName, int age, string pesel, int gender)
        {
            FirstName = fName;
            SurName = sName;
            Age = age;
            Pesel = pesel;
            Gender = gender;
        }

        internal string ToSqlString()
        {
            string values = "";
            values += "'" + FirstName + "', ";
            values += "'" + SurName + "', ";
            values += Age + ", ";
            values += Gender + ", ";
            values += "'" + Pesel + "', ";
            values += Height + ", ";
            values += Weight + ", ";
            values += Ecg_Id;
            return values;
        }
        
        internal string ToSqlUpdateString()
        {
            string values = "";
            values += "name = '" + FirstName + "', ";
            values += "surname = '" + SurName + "', ";
            values += "age = " + Age + ", ";
            values += "sex = " + Gender + ", ";
            values += "pesel = '" + Pesel + "', ";
            values += "height = " + Height + ", ";
            values += "weight = " + Weight + ", ";
            values += "ecg_id  = " + Ecg_Id;
            return values;
        }
    }
}
