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
            Patient.FirstName = name;
            Patient.SurName = surname;
            int parsedAge;
            if (int.TryParse(age, out parsedAge))
            {
                Patient.Age = parsedAge;
            }
            else
            {
                Patient.Age = -1;
            }
            int parsedHeight;
            if (int.TryParse(height, out parsedHeight))
            {
                Patient.Height = parsedHeight;
            }
            else
            {
                Patient.Height = -1;
            }
            int parsedWeight;
            if (int.TryParse(weight, out parsedWeight))
            {
                Patient.Weight = parsedWeight;
            }
            else
            {
                Patient.Weight = -1;
            }
            Patient.Pesel = pesel;
            Patient.Gender = gender;

            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            conn.UpdatePatient(Patient);
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
                conn.InsertEkg(openFileDialog.SafeFileName, new DateTime(), Patient);
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
            List<string> ecgFilenames = conn.GetPatientEcgFilenames(Patient);
            string filename = ecgFilenames.First();
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
            var patientId = Patient.Id;
            Wywiad wywiad = new Wywiad(patientId);
            wywiad.ShowDialog();
        }

        private void DiagnoseButton_Click(object sender, RoutedEventArgs e)
        {
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute(@"cd " + Directory.GetCurrentDirectory() + "/../../");

            DBHandler conn = new DBHandler();
            conn.InitializeConnection();
            List<string> ecgFilenames = conn.GetPatientEcgFilenames(Patient);
            string filename = ecgFilenames.First();
            conn.CloseConnection();

            if (filename != null)
            {
                object result;
                matlab.Feval("ECG_diagnosis", 5, out result, filename, 500.0);
                object[] res = result as object[];

                double[][] ecgMoments = new double[4][];
                for (int i = 1; i < 5; i++)
                {
                    if (res[i] is System.Reflection.Missing)
                    {
                        ecgMoments[i - 1] = new double[0];
                    }
                    else
                    {
                        double[,] moments = (double[,]) res[i];
                        ecgMoments[i - 1] = moments.Cast<double>().ToArray();
                    }
                }

                EcgCharacteristics ecgCharacteristics =
                    new EcgCharacteristics((double)res[0], ecgMoments[0], ecgMoments[1], ecgMoments[2], ecgMoments[3]);
            }
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
