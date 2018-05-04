using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDP.Objects
{
    public class DbAnswer
    {
        public int PatientId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }

        public DbAnswer()
        {

        }

        public DbAnswer(string answer)
        {
            Answer = answer;
        }

        public DbAnswer(string answer, int patientId, int questionId)
        {
            Answer = answer;
            QuestionId = questionId;
            PatientId = patientId;
        }

        internal string ToSqlString()
        {
            string values = "";
            values += PatientId + ", ";
            values += QuestionId + ", ";
            values += "'" + Answer + "'";
            return values;
        }

        internal string ToSqlUpdateString()
        {
            string values = "";
            values += "patient_id = " + PatientId + ", ";
            values += "question_id = " + QuestionId + ", ";
            values += "answer = '" + Answer + "'";
            return values;
        }
    }
}
