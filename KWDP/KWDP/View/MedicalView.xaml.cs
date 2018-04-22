using KWDP.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KWDP.View
{
    /// <summary>
    /// Interaction logic for MedicalView.xaml
    /// </summary>
    public partial class MedicalView : UserControl
    {
        public ObservableCollection<Patient> PatientsList { get; set; }
        public Patient Patient { get; set; }
        public PatientView PatientViewInstance { get; set; }
        public int Index { get; set; }

        public MedicalView()
        {
            InitializeComponent();            
        }

        private void LoadData()
        {
            NameTextBox.Text = Patient.FirstName;
            SurNameTextBox.Text = Patient.SurName;
            AgeTextBox.Text = Patient.Age.ToString();
            PeselTextBox.Text = Patient.Pesel;
            if (Patient.Gender >= 0)
                GenderComboBox.SelectedIndex = Patient.Gender;

            WeightTextBox.Text = Patient.Weight.ToString();
            HeightTextBox.Text = Patient.Height.ToString();
        }

        private void SavePatient()
        {
            var name = NameTextBox.Text;
            var surname = SurNameTextBox.Text;
            var age = AgeTextBox.Text;
            var weight = WeightTextBox.Text;
            var height = HeightTextBox.Text;
            var pesel = PeselTextBox.Text;
            if (string.IsNullOrEmpty(pesel))
                return;
            var gender = GenderComboBox.SelectedIndex;
            var patient = new Patient();
            patient.FirstName = name;
            patient.SurName = surname;
            int parsedAge;
            if (int.TryParse(age, out parsedAge))
            {
                patient.Age = parsedAge;
            }
            else
            {
                patient.Age = -1;
            }
            int parsedHeight;
            if (int.TryParse(height, out parsedHeight))
            {
                patient.Height = parsedHeight;
            }
            else
            {
                patient.Height = -1;
            }
            int parsedWeight;
            if (int.TryParse(weight, out parsedWeight))
            {
                patient.Weight = parsedWeight;
            }
            else
            {
                patient.Weight = -1;
            }
            patient.Pesel = pesel;
            patient.Gender = gender;

            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            conn.UpdatePatient(patient);
            conn.CloseConnection();

        }

        private void LoadEkgButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Msg.TO_BE_CONTINUED);
        }

        private void LoadQuestionsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Msg.TO_BE_CONTINUED);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SavePatient();
            PatientViewInstance.PatientsList = PatientsList;
            PatientViewInstance.LoadPatientsList();
            this.Content = PatientViewInstance;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
