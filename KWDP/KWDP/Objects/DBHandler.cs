using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KWDP.Objects
{
    public class DBHandler
    {
        SQLiteConnection m_dbConnection;
        bool isOpen;

        public void InitializeConnection()
        {
            string basePath = @"..\..\Objects\\ecg.db";
            m_dbConnection = new SQLiteConnection("Data Source=" + basePath + ";Version=3;");
            m_dbConnection.Open();
            isOpen = true;
        }

        public void CloseConnection()
        {
            m_dbConnection.Close();
            isOpen = false;
        }

        public void AddPatientToTable(Patient patient)
        {
            if (isOpen)
            {
                string values = patient.ToSqlString();
                string sql = "insert into patient (name, surname, age, sex, pesel, height, weight) values (" + values + ")";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public void AddQuestionToTable(DbQuestion question)
        {
            if (isOpen)
            {
                string values = question.ToSqlString();
                string sql = "insert into question (content, description, type) values (" + values + ")";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public Patient GetPatient(string pesel)
        {
            Patient patient = null;
            if (isOpen)
            {
                string sql = "select * from patient where pesel='" + pesel + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                patient = new Patient();
                patient.FirstName = (string)reader["name"];
                patient.SurName = (string)reader["surname"];
                patient.Age = (int)reader["age"];
                patient.Pesel = pesel;
                patient.Gender = (int)reader["sex"];
                patient.Height = (int)reader["height"];
                patient.Weight = (int)reader["weight"];
                patient.Id = (int)reader["id"];
            }
            return patient;
        }

        public List<Patient> GetAllPatients()
        {
            List<Patient> patients = new List<Patient>();
            if (isOpen)
            {
                string sql = "select * from patient";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Patient tempPatient = new Patient();
                    tempPatient.FirstName = reader["name"].ToString();
                    tempPatient.SurName = reader["surname"].ToString();
                    tempPatient.Age = int.Parse(reader["age"].ToString());
                    tempPatient.Pesel = reader["pesel"].ToString();
                    tempPatient.Gender = int.Parse(reader["sex"].ToString());
                    tempPatient.Height = int.Parse(reader["height"].ToString());
                    tempPatient.Weight = int.Parse(reader["weight"].ToString());
                    tempPatient.Id = int.Parse(reader["id"].ToString());
                    patients.Add(tempPatient);
                }
            }
            return patients;
        }

        public List<DbQuestion> GetAllQuestions()
        {
            List<DbQuestion> questions = new List<DbQuestion>();
            if (isOpen)
            {
                string sql = "select * from question";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DbQuestion tempPquestiont = new DbQuestion();
                    tempPquestiont.Content = reader["content"].ToString();
                    tempPquestiont.Description = reader["description"].ToString();
                    tempPquestiont.Type = int.Parse(reader["type"].ToString());
                    tempPquestiont.Id = int.Parse(reader["id"].ToString());
                    questions.Add(tempPquestiont);
                }
            }
            return questions;
        }

        public void UpdatePatient(Patient patient)
        {
            if (isOpen)
            {
                string values = patient.ToSqlUpdateString();
                string sql = "update patient set " + values + " where id = " + patient.Id + "";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public void RemovePatient(Patient patient)
        {
            if (isOpen)
            {
                string sql = "delete from patient where id = " + patient.Id + "";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public void InsertEkg(string filename, DateTime date, Patient patient)
        {
            if (isOpen)
            {
                try
                {
                    string sql = "insert into ecg (ecg, date, patient_id)" + 
                        " values ('" + filename + "', '" + date.ToString("dd-MM-yyyy") + "', " + patient.Id +")" ;
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ECG juz w bazie !");
                }
            }
        }

        public string GetEcg(int ecg_id)
        {
            string filename = null;

            if (isOpen)
            {
                string sql = "select ecg from ecg where id = " + ecg_id + "";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    filename = reader["ecg"].ToString();
                }
            }
            return filename;
        }

        public List<string> GetPatientEcgFilenames(Patient patient)
        {
            List<string> filenames = new List<string>();
            if (isOpen)
            {
                string sql = "select ecg from ecg where patient_id = " + patient.Id;
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    filenames.Add(reader["ecg"].ToString());
                }
            }
            return filenames;
        }

        public List<DbAnswer> GetPatientAnswers(int patientId)
        {
            List<DbAnswer> answers = new List<DbAnswer>();

            if (isOpen)
            {
                DbAnswer tempAnsw = new DbAnswer();
                string sql = "select * from patient_answer where patient_id = " + patientId;
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tempAnsw.Answer = reader["answer"].ToString();
                    tempAnsw.QuestionId = int.Parse(reader["question_id"].ToString());
                    tempAnsw.PatientId = int.Parse(reader["patient_id"].ToString());
                    answers.Add(tempAnsw);
                }
            }
            return answers;
        }

        public void InitializePatientAnswer(DbAnswer answer)
        {
            if (isOpen)
            {
                string values = answer.ToSqlString();
                string sql = "insert into patient_answer (patient_id, question_id, answer) values (" + values + ")";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public void UpdatePatientAnswers(List<DbAnswer> answers)
        {
            if (isOpen)
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    string values = answers[i].ToSqlUpdateString();
                    string sql = "update patient_answer set " + values + " where patient_id = " + answers[i].PatientId + " and question_id = " + answers[i].QuestionId;
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
