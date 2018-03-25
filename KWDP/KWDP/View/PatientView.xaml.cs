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
    /// Interaction logic for Patient.xaml
    /// </summary>
    public partial class PatientView : UserControl
    {
        public ObservableCollection<Patient> PatientsList { get; set; }
        public CreatePatientDialog CreatePatientDialog { get; set; }

        public PatientView()
        {
            DataContext = this;
            PatientsList = new ObservableCollection<Patient>();

            InitializeComponent();

            LoadPatientsList();
            HideSzczegolySection();
        }

        private void LoadPatientsList()
        {
            var a = new Patient("Jan", "Kowalski", 18, "93827193821");
            var b = new Patient("Adam", "Adamski", 19, "93827193822");
            var c = new Patient("Tomasz", "Nowak", 20, "93827193823");
            PatientsList.Add(a);
            PatientsList.Add(b);
            PatientsList.Add(c);
        }

        private void HideSzczegolySection()
        {
            PodgladNameLabel.Visibility = Visibility.Collapsed;
            PodgladNameLabelContent.Visibility = Visibility.Collapsed;

            PodgladSurNameLabel.Visibility = Visibility.Collapsed;
            PodgladSurNameLabelContent.Visibility = Visibility.Collapsed;

            PodgladAgeLabel.Visibility = Visibility.Collapsed;
            PodgladAgeLabelContent.Visibility = Visibility.Collapsed;

            PodgladPeselLabel.Visibility = Visibility.Collapsed;
            PodgladPeselLabelContent.Visibility = Visibility.Collapsed;
        }

        private void ShowSzczegolySection(Patient patient)
        {
            PodgladNameLabel.Visibility = Visibility.Visible;
            PodgladNameLabelContent.Visibility = Visibility.Visible;
            PodgladNameLabelContent.Content = patient.FirstName;

            PodgladSurNameLabel.Visibility = Visibility.Visible;
            PodgladSurNameLabelContent.Visibility = Visibility.Visible;
            PodgladSurNameLabelContent.Content = patient.SurName;

            PodgladAgeLabel.Visibility = Visibility.Visible;
            PodgladAgeLabelContent.Visibility = Visibility.Visible;
            if(patient.Age < 0)
            {
                PodgladAgeLabelContent.Content = "---";
            }
            else
            {
                PodgladAgeLabelContent.Content = patient.Age.ToString();
            }

            PodgladPeselLabel.Visibility = Visibility.Visible;
            PodgladPeselLabelContent.Visibility = Visibility.Visible;
            PodgladPeselLabelContent.Content = patient.Pesel.ToString();
        }

        private void PatientsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectetPatient = (Patient)PatientsListView.SelectedItem;
            if(selectetPatient != null)
            {
                ShowSzczegolySection(selectetPatient);
            }
            else
            {
                HideSzczegolySection();
            }
        }

        private void NewPatientButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePatientDialog = new CreatePatientDialog(PatientsList);
            CreatePatientDialog.ShowDialog();
        }

        private void DeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PatientsList?.Count > 0)
                {
                    var elementToRemove = (Patient)PatientsListView.SelectedItem;
                    PatientsList.Remove(elementToRemove);
                }
                else
                {
                    MessageBox.Show(Msg.CANNOT_DELETE);
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occured:" + ex.Message);
            }
        }

        private void EditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            var selectetPatient = PatientsListView.SelectedIndex;
            CreatePatientDialog = new CreatePatientDialog(PatientsList, selectetPatient);
            CreatePatientDialog.ShowDialog();
        }

        private void InvestigatePatientButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Msg.TO_BE_CONTINUED);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
