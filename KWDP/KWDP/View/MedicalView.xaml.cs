using KWDP.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (!Directory.Exists("./ecg"))
                {
                    Directory.CreateDirectory("./ecg");
                }

                File.Copy(openFileDialog.FileName, "./ecg/" + openFileDialog.SafeFileName, true);
                // copy to local folder

                DBHandler conn = new DBHandler();
                conn.InitializeConnection();
                conn.InsertEkg(this.Patient.Ecg_Id, openFileDialog.SafeFileName);
                conn.CloseConnection();

                // copy to local folder
            }

            // insert path to db
            // show path in listview
        }

        private void ViewEkgButton_Click(object sender, RoutedEventArgs e)
        {
            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            string filename = conn.GetEcg(this.Patient.Ecg_Id);
            conn.CloseConnection();

            if (filename != null)
            {
                EcgValues[] values = File.ReadAllLines("./ecg/" + filename)
                                           .Skip(1)
                                           .Select(v => EcgValues.FromCsv(v))
                                           .ToArray();

                var bitmap = this.DrawEcg(values);

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );

                this.EcgImage.Source = bitmapSource;
            }

        }

        private Bitmap DrawEcg(EcgValues[] values)
        {
            float[] firstCanal = values.Select(x => x.FirstCanal).ToArray();

            Bitmap bmp = new Bitmap(firstCanal.Length, 1000);            

            float maxValue = firstCanal.Max();

            var samplesNumber = bmp.Width;
            var maxAmplitude = bmp.Height / 2 - 1;
            var baseLine = bmp.Height / 2;

            var points = new List<PointF>();

            for (int i = 0; i < samplesNumber; i++)
            {
                points.Add(new PointF(i, -firstCanal[i] / maxValue * maxAmplitude + baseLine));
            }

            //var points = firstCanal.Select((v) => new System.Drawing.PointF(
            //    i,
            //    (float)(-v / maxValue) * maxAmplitude) + baseLine)).ToArray();

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawCurve(new Pen(Color.Red, 8), points.ToArray());
            }

            return bmp;

        }

        private void LoadQuestionsButton_Click(object sender, RoutedEventArgs e)
        {
            Wywiad wywiad = new Wywiad();
            wywiad.ShowDialog();
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
