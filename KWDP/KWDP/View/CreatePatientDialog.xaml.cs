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
using KWDP.Objects;

namespace KWDP.View
{
    /// <summary>
    /// Interaction logic for CreatePatientDialog.xaml
    /// </summary>
    public partial class CreatePatientDialog : Window
    {
        private ObservableCollection<Patient> patientsList;
        private bool isEditing;
        private int Index;

        public CreatePatientDialog()
        {
            InitializeComponent();
        }

        public CreatePatientDialog(ObservableCollection<Patient> patientsList)
        {
            InitializeComponent();
            this.Title = "Dodaj Pacjenta";
            this.Width = 350;
            this.Height = 250;
            this.patientsList = patientsList;
            this.isEditing = false;
        }

        public CreatePatientDialog(ObservableCollection<Patient> patientsList, int index)
        {
            InitializeComponent();
            this.Title = "Dodaj Pacjenta";
            this.Width = 350;
            this.Height = 250;
            this.patientsList = patientsList;
            this.isEditing = true;
            this.Index = index;

            var patient = patientsList[Index];
            NameTextBox.Text = patient.FirstName;
            SurNameTextBox.Text = patient.SurName;
            AgeTextBox.Text = patient.Age.ToString();
            PeselTextBox.Text = patient.Pesel;

            PeselTextBox.IsEnabled = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewPatient();
        }

        private void AddNewPatient()
        {
            var name = NameTextBox.Text;
            var surname = SurNameTextBox.Text;
            var age = AgeTextBox.Text;
            var pesel = PeselTextBox.Text;
            if (string.IsNullOrEmpty(pesel))
                return;

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
            patient.Pesel = pesel;

            if (patientsList.Count == 0 || (isEditing || (patientsList.All(x => x.Pesel != patient.Pesel))))
            {
                if (!isEditing)
                {
                    patientsList.Add(patient);
                }
                else
                {
                    patientsList[Index] = patient;
                }
                this.Close();
            }
            else MessageBox.Show(Msg.DISTINCT_PATIENTS);            
        }
    }
}
