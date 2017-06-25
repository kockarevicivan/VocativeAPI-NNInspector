using Microsoft.Win32;
using Newtonsoft.Json;
using ORI.NeuralNetworks.Models;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace ORI.NeuralNetworks
{
    public partial class MainWindow : Window
    {
        private NeuralNetworkWrapper _neuralNetworkWrapper;
        private bool _fileSaved = false;
        private string _currentFileName = "Untitled";

        public MainWindow()
        {
            _neuralNetworkWrapper = new NeuralNetworkWrapper();
            Commons.mainWindow = this;

            InitializeComponent();
            
            SetTitle();
        }

        private void new_Click(object sender, RoutedEventArgs e)
        {
            _fileSaved = false;
            _currentFileName = "Untitled";

            SetTitle();

            _neuralNetworkWrapper = new NeuralNetworkWrapper();

            requestRichTextbox.Document.Blocks.Clear();
            requestRichTextbox.Document.Blocks.Add(new Paragraph(new Run("")));
            responseRichTextbox.Document.Blocks.Clear();
            responseRichTextbox.Document.Blocks.Add(new Paragraph(new Run("")));
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_fileSaved)
                {
                    SerializationHelper.SerializeObject<NeuralNetworkWrapper>(_neuralNetworkWrapper, _currentFileName);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
                    saveFileDialog.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, @"..\..\..\database"));

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        SerializationHelper.SerializeObject<NeuralNetworkWrapper>(_neuralNetworkWrapper, saveFileDialog.FileName);
                        _currentFileName = saveFileDialog.FileName;
                        SetTitle();
                        _fileSaved = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
                saveFileDialog.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, @"..\..\..\database"));

                if (saveFileDialog.ShowDialog() == true)
                {
                    SerializationHelper.SerializeObject<NeuralNetworkWrapper>(_neuralNetworkWrapper, saveFileDialog.FileName);
                    _currentFileName = saveFileDialog.FileName;
                    SetTitle();
                    _fileSaved = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Binary File (*.bin)|*.bin";
                openFileDialog.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, @"..\..\..\database"));

                if (openFileDialog.ShowDialog() == true)
                {
                    _neuralNetworkWrapper = SerializationHelper.DeSerializeObject<NeuralNetworkWrapper>(openFileDialog.FileName);
                    _currentFileName = openFileDialog.FileName;
                    SetTitle();
                    _fileSaved = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = new TextRange(requestRichTextbox.Document.ContentStart, requestRichTextbox.Document.ContentEnd).Text;

                RequestModel requestModel = JsonConvert.DeserializeObject<RequestModel>(input);
                ResponseModel responseModel = _neuralNetworkWrapper.TestMultiple(requestModel.Nouns.ToArray());

                responseRichTextbox.Document.Blocks.Clear();
                responseRichTextbox.Document.Blocks.Add(new Paragraph(new Run(JsonConvert.SerializeObject(responseModel, Formatting.Indented))));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            requestRichTextbox.Document.Blocks.Clear();
            requestRichTextbox.Document.Blocks.Add(new Paragraph(new Run("")));
            responseRichTextbox.Document.Blocks.Clear();
            responseRichTextbox.Document.Blocks.Add(new Paragraph(new Run("")));
        }

        private void trainBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = new TextRange(responseRichTextbox.Document.ContentStart, responseRichTextbox.Document.ContentEnd).Text;

                ResponseModel requestModel = JsonConvert.DeserializeObject<ResponseModel>(input);

                indicatorLabel.Content = "Training...";
                submitBtn.IsEnabled = false;
                initialTrainingBtn.IsEnabled = false;
                trainBtn.IsEnabled = false;

                Thread t = new Thread(new ThreadStart(() => {
                    _neuralNetworkWrapper.TrainOnConcreteNouns(requestModel.Nouns);
                }));
                t.Start();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void initialTrainingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                indicatorLabel.Content = "Training...";
                submitBtn.IsEnabled = false;
                initialTrainingBtn.IsEnabled = false;
                trainBtn.IsEnabled = false;

                _neuralNetworkWrapper.InitialTraining();

                submitBtn.IsEnabled = true;
                initialTrainingBtn.IsEnabled = true;
                trainBtn.IsEnabled = true;
                indicatorLabel.Content = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void SetTitle()
        {
            this.Title = "Vocative API NN Inspector - " + _currentFileName;
        }
    }
}
