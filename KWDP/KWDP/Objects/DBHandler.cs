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
                string sql = "insert into patient (name, surname, age, sex, pesel, height, weight, ecg_id) values (" + values + ")";
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
                patient.Ecg_Id = (int)reader["ecg_id"];
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
                    tempPatient.Ecg_Id = int.Parse(reader["ecg_id"].ToString());
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
                    questions.Add(tempPquestiont);
                }
            }
            return questions;
        }

        internal void UpdatePatient(Patient patient)
        {
            if (isOpen)
            {
                string values = patient.ToSqlUpdateString();
                string sql = "update patient set " + values + " where pesel ='" + patient.Pesel + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        internal void RemovePatient(Patient patient)
        {
            if (isOpen)
            {
                string sql = "delete from patient where pesel ='" + patient.Pesel + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }

        internal void InsertEkg(int ecg_id, string filename)
        {
            if (isOpen)
            {
                try
                {
                    string sql = "insert into ecg (id, ecg)" + "values ('" + ecg_id + "', " + "'" + filename + "')";
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                catch( Exception ex)
                {
                    MessageBox.Show("ECG juz w bazie !");
                }
            }
        }

        internal string GetEcg(int ecg_id)
        {
            string filename = null;

            if (isOpen)
            {
                string sql = "select ecg from ecg where id = '" + ecg_id + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                
                while (reader.Read())
                {
                    filename = reader["ecg"].ToString();
                }
            }
            return filename;
        }
    }
}
