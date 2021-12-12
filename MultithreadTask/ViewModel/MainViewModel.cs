using MultithreadTask.Commands;
using MultithreadTask.Encrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MultithreadTask.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        public MainWindow MainView { get; set; }
        public RelayCommand FromCommand { get; set; }
        public RelayCommand ToCommand { get; set; }
        public RelayCommand CopyCommand { get; set; }
        public RelayCommand ResumeCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public string EncryptedFile { get; set; }

        string fileContentTo = string.Empty;

        string filePathTo = string.Empty;

        string fileContentFrom = string.Empty;


        string filePathFrom = string.Empty;


        public List<string> Allsentences { get; set; }

        public int TextLength { get; set; }

        static object obj = new object();
        public int Count { get; set; } = 0;

        DispatcherTimer dispatcher = new DispatcherTimer();


        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            Count++;
            try
            {
                if (Count < Allsentences.Count())
                {
                    MainView.progressBar.Value = MainView.progressBar.Value + 10;
                    File.AppendAllText(filePathTo, Allsentences[Count]);
                }
                else
                {
                    MainView.progressBar.Value = double.MaxValue;
                    dispatcher.Stop();
                }



            }
            catch (Exception)
            {

            }
        }
        public MainViewModel()
        {
            dispatcher.Interval = TimeSpan.FromSeconds(2);
            dispatcher.Tick += Dispatcher_Tick;

            FromCommand = new RelayCommand((sender) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;



                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        filePathFrom = openFileDialog.FileName;
                        var fileStream = openFileDialog.OpenFile();
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContentFrom = reader.ReadToEnd();
                        }
                    }
                    MainView.FromTxtBx.Text = filePathFrom;
                }

                string text = File.ReadAllText(filePathFrom);
                EncryptedFile = EncryptHelper.Encrypt(text);
                TextLength = EncryptedFile.Length / 100;
            });

            ToCommand = new RelayCommand((sender) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        filePathTo = openFileDialog.FileName;

                        var fileStream = openFileDialog.OpenFile();


                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContentTo = reader.ReadToEnd();
                        }
                    }
                    MainView.ToTxtBx.Text = filePathTo;
                }
            });

            CopyCommand = new RelayCommand((sender) =>
            {
                string newtext = EncryptHelper.Decrypt(EncryptedFile);
                Allsentences = new List<string>();
                foreach (var item in newtext.Split('.'))
                {
                    Allsentences.Add(item);
                }

                MainView.progressBar.Value = 0;



                dispatcher.Start();


            });

            ResumeCommand = new RelayCommand((sender) =>
            {
                try
                {
                    dispatcher.Start();
                }
                catch (Exception)
                {
                }
            });

            PauseCommand = new RelayCommand((sender) =>
            {
                try
                {
                    dispatcher.Stop();
                }
                catch (Exception)
                {
                }
            });
        }
    }
}
