using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KWDP.Objects
{
    /// <summary>
    /// Interaction logic for Wywiad.xaml
    /// </summary>
    public partial class Wywiad : Window
    {
        List<DbQuestion> Questions;
        List<DbAnswer> Answers;
        int PatientID;

        public Wywiad(int patientId)
        {
            InitializeComponent();
            PatientID = patientId;
            Questions = new List<DbQuestion>();
            Answers = new List<DbAnswer>();
            InitAnswers();
            InitList();
        }

        public void InitList()
        {
            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            Questions = conn.GetAllQuestions();
            conn.CloseConnection();
            QuestionsListView.ItemsSource = Questions;

            for (int i = 0; i < Questions.Count; i++)
            {
                var question = (DbQuestion)QuestionsListView.Items[i];
                for (int j = 0; j < Answers.Count; j++)
                {
                    if (Answers.ElementAt(j).QuestionId == question.Id)
                    {
                        var answer = Answers.ElementAt(j).Answer;
                        if (!answer.Equals(""))
                        {
                            Questions[i].Type = 1;
                            //zmiana koloru
                        }
                    }
                }
            }
            QuestionsListView.ItemsSource = Questions;
        }

        public void InitAnswers()
        {
            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            Answers = conn.GetPatientAnswers(PatientID);
            if (Answers.Count < 1)
            {
                List<DbAnswer> answers = PrepareEmptyAnswers();
                //tworze 16 roznych rekordow patient_answer
                for (int i = 0; i < answers.Count; i++)
                {
                    conn.InitializePatientAnswer(answers.ElementAt(i));
                }
                //po sprawedzeniu jest 16 IDENTYCZNYCH rekordow patient_answer
                Answers = conn.GetPatientAnswers(PatientID);
            }
            conn.CloseConnection();
        }

        public List<DbAnswer> PrepareEmptyAnswers()
        {
            List<DbAnswer> answers = new List<DbAnswer>();
            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            List<DbQuestion> questions = conn.GetAllQuestions();
            conn.CloseConnection();
            for (int i = 0; i < questions.Count; i++)
            {
                DbAnswer tempAnsw = new DbAnswer("", PatientID, questions[i].Id);
                answers.Add(tempAnsw);
            }    
            return answers;
        }

    private void WrocButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ZapiszButton_Click(object sender, RoutedEventArgs e)
    {
        DBHandler conn = new DBHandler();
        conn.InitializeConnection();
        conn.UpdatePatientAnswers(Answers);
        conn.CloseConnection();

        this.Close();
    }

    private void QuestionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectetquestion = (DbQuestion)QuestionsListView.SelectedItem;
        if (selectetquestion != null)
        {
            mainQuestion.Text = "";
            helperText.Text = "";
            CheckBoxTak.IsChecked = false;
            CheckBoxNie.IsChecked = false;

            mainQuestion.Text = selectetquestion.Content;
            helperText.Text = selectetquestion.Description;
            string answer = "";
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers.ElementAt(i).QuestionId == selectetquestion.Id)
                {
                    answer = Answers.ElementAt(i).Answer;
                }
            }

            if (answer.Equals("0") || answer.ToLower().Equals("false") || answer.ToLower().Equals("nie"))
            {
                CheckBoxNie.IsChecked = true;
            }
            else if (answer.Equals("1") || answer.ToLower().Equals("true") || answer.ToLower().Equals("tak"))
            {
                CheckBoxTak.IsChecked = true;
            }
        }
        else
        {
            mainQuestion.Text = "";
            helperText.Text = "";
            CheckBoxTak.IsChecked = false;
            CheckBoxNie.IsChecked = false;
        }
    }

    private void CheckBoxTak_Checked(object sender, RoutedEventArgs e)
    {
        var selectetquestion = (DbQuestion)QuestionsListView.SelectedItem;
        if (selectetquestion != null)
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers.ElementAt(i).QuestionId == selectetquestion.Id)
                {
                    Answers.ElementAt(i).Answer = "tak";
                }
            }
        }
        Questions[QuestionsListView.SelectedIndex].Type = 1;
        QuestionsListView.ItemsSource = Questions;
    }

    private void CheckBoxNie_Checked(object sender, RoutedEventArgs e)
    {
        var selectetquestion = (DbQuestion)QuestionsListView.SelectedItem;
        if (selectetquestion != null)
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers.ElementAt(i).QuestionId == selectetquestion.Id)
                {
                    Answers.ElementAt(i).Answer = "nie";
                }
            }
        }
        Questions[QuestionsListView.SelectedIndex].Type = 1;
        QuestionsListView.ItemsSource = Questions;
    }
}
}
