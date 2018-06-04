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
using ECGConversion.SCP;
using ECGConversion;
using ECGConversion.ECGSignals;

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

        private int bitmapWidth = 500;

        public EcgProvider EProvider = new EcgProvider(null);

        public MedicalView()
        {
            InitializeComponent();
            this.EProvider.offset = this.bitmapWidth;
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
            openFileDialog.Filter = "csv files (*.csv)|*.csv|scp files (*.SCP)|*.SCP|all files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!Directory.Exists("./ecg"))
                {
                    Directory.CreateDirectory("./ecg");
                }

                string filename = string.Empty;

                if (Path.GetExtension(openFileDialog.FileName) == ".SCP")
                {

                    filename = Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".csv";

                    var secondLead = GetSecondLead(openFileDialog.FileName);

                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(("./ecg/" + filename)))
                    {
                        file.WriteLine("nr signal");
                        for (int i = 0; i < secondLead.Length; i++)
                        {
                            file.WriteLine(i + 1 + " " + secondLead[i]);
                        }

                    }
                }

                else
                {
                    File.Copy(openFileDialog.FileName, "./ecg/" + openFileDialog.SafeFileName, true);

                    filename = openFileDialog.SafeFileName;
                }

                DBHandler conn = new DBHandler();
                conn.InitializeConnection();
                conn.InsertEkg(filename, new DateTime(), Patient);
                conn.CloseConnection();

                // copy to local folder


                
            }
        }

        short[] GetSecondLead(string fileName)
        {
            IECGFormat format = null;

            var fmt = "SCP-ECG";

            IECGReader reader = ECGConverter.Instance.getReader(fmt);
            ECGConfig cfg = ECGConverter.Instance.getConfig(fmt);

            format = reader.Read(fileName, 0, cfg);

            Signals _CurrentSignal;

            format.Signals.getSignals(out _CurrentSignal);

            if (_CurrentSignal != null)
            {
                for (int i = 0, en = _CurrentSignal.NrLeads; i < en; i++)
                {
                    ECGTool.NormalizeSignal(_CurrentSignal[i].Rhythm, _CurrentSignal.RhythmSamplesPerSecond);
                }
            }

            Signals sig = _CurrentSignal.CalculateTwelveLeads();

            return sig.GetLeads()[1].Rhythm;
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

                this.EProvider.Signal = values.Select(x => x.Signal).ToArray();

                this.bitmapWidth = 500;
                this.EProvider.offset = this.bitmapWidth;

                var bitmap = this.DrawEcg(this.EProvider.GetSignal(0));

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );

                this.EcgImage.Source = bitmapSource;
            }

        }

        private Bitmap DrawEcg(short[] values)
        {  
            Bitmap bmp = new Bitmap(this.bitmapWidth, 300);

            float maxValue = values.Max();

            float minValue = values.Min();

            float diff = maxValue - minValue;

            var samplesNumber = bmp.Width;
            var maxAmplitude = bmp.Height;// / 2 - 1;
            var baseLine = bmp.Height;// / 2;

            var points = new List<PointF>();

            for (int i = 0; i < samplesNumber; i++)
            {
                points.Add(new PointF(i, -((values[i] - minValue)/ diff) * maxAmplitude + baseLine));
            }            

            using (var g = Graphics.FromImage(bmp))
            {  
                for(int i =0; i<bmp.Width; i+=10)
                {
                    g.DrawLine(new Pen(Color.LightPink, 1), new PointF(i, 0), new PointF(i, bmp.Width -1));
                }

                for (int i = 0; i < bmp.Width; i += 50)
                {
                    g.DrawLine(new Pen(Color.LightPink, 2), new PointF(i, 0), new PointF(i, bmp.Width - 1));
                }


                for (int i = 0; i < bmp.Height; i += 10)
                {
                    g.DrawLine(new Pen(Color.LightPink, 1), new PointF(0, i), new PointF(bmp.Width - 1, i));
                }

                for (int i = 0; i < bmp.Height; i += 50)
                {
                    g.DrawLine(new Pen(Color.LightPink, 2), new PointF(0, i), new PointF(bmp.Width - 1, i));
                }

                g.DrawCurve(new Pen(Color.Red, 4), points.ToArray());
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
            List<DbAnswer> patientAnswers = conn.GetPatientAnswers(Patient.Id);
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
                        double[,] moments = (double[,])res[i];
                        ecgMoments[i - 1] = moments.Cast<double>().ToArray();
                    }
                }

                int[][] questionIds =
                {
                        new int[]{ 1, 2, 5, 8, 11, 15 },
                        new int[]{ 1, 2, 3, 4, 5, 8, 9, 11, 15 },
                        new int[]{ 3, 4, 5, 8, 15 },
                        new int[]{ 1, 3, 5, 8, 11, 15 }
                    };
                double maxScore = 0.0;
                int diagnosis = -1;
                for (int i = 0; i < ecgMoments.Length; i++)
                {
                    double score = 0.0;
                    if (ecgMoments[i].Length > 0)
                    {
                        foreach (DbAnswer answer in patientAnswers)
                        {
                            if (questionIds[i].Contains(answer.QuestionId) && answer.Answer.ToLower().Equals("tak"))
                            {
                                score++;
                            }
                        }
                        score = score * 10 / questionIds[i].Length;
                        if (i <= 1 && Patient.Age >= 40)
                        {
                            score += (Patient.Age - 40) / 10.0;
                        }
                        score += ecgMoments[i].Length;
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;
                        diagnosis = i;
                    }
                }

                EcgCharacteristics ecgCharacteristics =
                    new EcgCharacteristics((double)res[0], ecgMoments[0], ecgMoments[1], ecgMoments[2], ecgMoments[3]);

                this.Diagnoza.Text = ecgCharacteristics.ToString();
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

        private void RighrClick(object sender, RoutedEventArgs e)
        {
            var data = this.EProvider.Right();
            Draw(data);

        }

        private void LeftClick(object sender, RoutedEventArgs e)
        {
            var data = this.EProvider.Left();
            Draw(data);
        }

        private void Draw(short[] data)
        {
            var bitmap = this.DrawEcg(data);

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );

            this.EcgImage.Source = bitmapSource;
        }

        private void PlusClick(object sender, RoutedEventArgs e)
        {
            if (this.bitmapWidth + 100 < this.EProvider.Signal.Length)
            {
                this.bitmapWidth += 100;
                this.EProvider.offset = this.bitmapWidth;
            }

            var data = this.EProvider.GetSignal();
            Draw(data);
        }

        private void MinusClick(object sender, RoutedEventArgs e)
        {
            if (this.bitmapWidth - 100 > 100)
            {
                this.bitmapWidth -= 100;
                this.EProvider.offset = this.bitmapWidth;
            }

            var data = this.EProvider.GetSignal();
            Draw(data);
        }
    }
}
