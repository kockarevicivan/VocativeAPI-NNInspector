using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using ORI.NeuralNetworks.BusinessLogic;
using ORI.NeuralNetworks.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace ORI.NeuralNetworks
{
    [Serializable]
    public class NeuralNetworkWrapper
    {
        private BackpropagationNetwork _genitiveNetwork;
        private BackpropagationNetwork _dativeOrLocativeNetwork;
        private BackpropagationNetwork _accussativeNetwork;
        private BackpropagationNetwork _vocativeNetwork;
        private BackpropagationNetwork _instrumentalNetwork;

        private double[] errorList;

        public NeuralNetworkWrapper()
        {
            errorList = new double[Constants.Cycles];

            InitializeGenitiveNetwork();
            InitializeDativeOrLocativeNetwork();
            InitializeAccussativeNetwork();
            InitializeVocativeNetwork();
            InitializeInstrumentalNetwork();
        }

        #region [Initialization]
        private void InitializeGenitiveNetwork()
        {
            SigmoidLayer inputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);
            SigmoidLayer hiddenLayer = new SigmoidLayer(Constants.NeuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);

            new BackpropagationConnector(inputLayer, hiddenLayer);
            new BackpropagationConnector(hiddenLayer, outputLayer);
            _genitiveNetwork = new BackpropagationNetwork(inputLayer, outputLayer);
            _genitiveNetwork.SetLearningRate(Constants.LearningRate);
        }

        private void InitializeDativeOrLocativeNetwork()
        {
            SigmoidLayer inputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);
            SigmoidLayer hiddenLayer = new SigmoidLayer(Constants.NeuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);

            new BackpropagationConnector(inputLayer, hiddenLayer);
            new BackpropagationConnector(hiddenLayer, outputLayer);
            _dativeOrLocativeNetwork = new BackpropagationNetwork(inputLayer, outputLayer);
            _dativeOrLocativeNetwork.SetLearningRate(Constants.LearningRate);
        }

        private void InitializeAccussativeNetwork()
        {
            SigmoidLayer inputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);
            SigmoidLayer hiddenLayer = new SigmoidLayer(Constants.NeuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);

            new BackpropagationConnector(inputLayer, hiddenLayer);
            new BackpropagationConnector(hiddenLayer, outputLayer);
            _accussativeNetwork = new BackpropagationNetwork(inputLayer, outputLayer);
            _accussativeNetwork.SetLearningRate(Constants.LearningRate);
        }

        private void InitializeVocativeNetwork()
        {
            SigmoidLayer inputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);
            SigmoidLayer hiddenLayer = new SigmoidLayer(Constants.NeuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);

            new BackpropagationConnector(inputLayer, hiddenLayer);
            new BackpropagationConnector(hiddenLayer, outputLayer);
            _vocativeNetwork = new BackpropagationNetwork(inputLayer, outputLayer);
            _vocativeNetwork.SetLearningRate(Constants.LearningRate);
        }

        private void InitializeInstrumentalNetwork()
        {
            SigmoidLayer inputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);
            SigmoidLayer hiddenLayer = new SigmoidLayer(Constants.NeuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(Constants.InterfaceNeuronCount);

            new BackpropagationConnector(inputLayer, hiddenLayer);
            new BackpropagationConnector(hiddenLayer, outputLayer);
            _instrumentalNetwork = new BackpropagationNetwork(inputLayer, outputLayer);
            _instrumentalNetwork.SetLearningRate(Constants.LearningRate);
        }
        #endregion [Initialization]

        #region [Training]
        public void InitialTraining()
        {
            string[] names = File.ReadAllLines(
                System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(
                        System.Reflection.Assembly.GetExecutingAssembly().Location,
                        @"..\..\..\database\initial_training.txt"
                    )
               ), Encoding.Default);

            Thread t = new Thread(new ThreadStart(() =>
            {
                Train(names);
            }));
            t.Start();
        }

        public void TrainOnConcreteNouns(List<NounModel> names)
        {
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.progressBar.Value = 0, DispatcherPriority.Background);

            TrainingSet genitiveSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet dativeLocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet accussativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet vocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet instrumentalSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);

            double step = 100.0 / names.Count;

            foreach (var n in names)
            {
                genitiveSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n.Nominative).ToArray(), EncodingHelper.EncodeInput(n.Genitive).ToArray()));
                dativeLocativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n.Nominative).ToArray(), EncodingHelper.EncodeInput(n.Dative).ToArray()));
                accussativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n.Nominative).ToArray(), EncodingHelper.EncodeInput(n.Accussative).ToArray()));
                vocativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n.Nominative).ToArray(), EncodingHelper.EncodeInput(n.Vocative).ToArray()));
                instrumentalSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n.Nominative).ToArray(), EncodingHelper.EncodeInput(n.Instrumental).ToArray()));

                Thread t1 = new Thread(new ThreadStart(() =>
                {
                    TrainGenitive(genitiveSet);
                }));
                t1.Start();

                Thread t2 = new Thread(new ThreadStart(() =>
                {
                    TrainDativeOrLocative(dativeLocativeSet);
                }));
                t2.Start();

                Thread t3 = new Thread(new ThreadStart(() =>
                {
                    TrainAccussative(accussativeSet);
                }));
                t3.Start();

                Thread t4 = new Thread(new ThreadStart(() =>
                {
                    TrainVocative(vocativeSet);
                }));
                t4.Start();

                Thread t5 = new Thread(new ThreadStart(() =>
                {
                    TrainInstrumental(instrumentalSet);
                }));
                t5.Start();

                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                t5.Join();

                genitiveSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                dativeLocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                accussativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                vocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                instrumentalSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);

                Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.progressBar.Value += step, DispatcherPriority.Background);
            }

            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.submitBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.initialTrainingBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.trainBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.indicatorLabel.Content = "Ready", DispatcherPriority.Background);
        }

        private void Train(string[] names)
        {
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.progressBar.Value = 0, DispatcherPriority.Background);

            TrainingSet genitiveSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet dativeLocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet accussativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet vocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
            TrainingSet instrumentalSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);

            double step = 100.0 / names.Length;

            foreach (var n in names)
            {
                genitiveSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n).ToArray(), EncodingHelper.EncodeInput(CaseHelper.GetGenitive(n)).ToArray()));
                dativeLocativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n).ToArray(), EncodingHelper.EncodeInput(CaseHelper.GetDativeOrLocative(n)).ToArray()));
                accussativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n).ToArray(), EncodingHelper.EncodeInput(CaseHelper.GetAccussative(n)).ToArray()));
                vocativeSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n).ToArray(), EncodingHelper.EncodeInput(CaseHelper.GetVocative(n)).ToArray()));
                instrumentalSet.Add(new TrainingSample(EncodingHelper.EncodeInput(n).ToArray(), EncodingHelper.EncodeInput(CaseHelper.GetInstrumental(n)).ToArray()));

                Thread t1 = new Thread(new ThreadStart(() =>
                {
                    TrainGenitive(genitiveSet);
                }));
                t1.Start();

                Thread t2 = new Thread(new ThreadStart(() =>
                {
                    TrainDativeOrLocative(dativeLocativeSet);
                }));
                t2.Start();

                Thread t3 = new Thread(new ThreadStart(() =>
                {
                    TrainAccussative(accussativeSet);
                }));
                t3.Start();

                Thread t4 = new Thread(new ThreadStart(() =>
                {
                    TrainVocative(vocativeSet);
                }));
                t4.Start();

                Thread t5 = new Thread(new ThreadStart(() =>
                {
                    TrainInstrumental(instrumentalSet);
                }));
                t5.Start();

                t1.Join();
                t2.Join();
                t3.Join();
                t4.Join();
                t5.Join();

                genitiveSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                dativeLocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                accussativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                vocativeSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);
                instrumentalSet = new TrainingSet(Constants.InterfaceNeuronCount, Constants.InterfaceNeuronCount);

                Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.progressBar.Value += step, DispatcherPriority.Background);
            }

            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.submitBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.initialTrainingBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.trainBtn.IsEnabled = true, DispatcherPriority.Background);
            Commons.mainWindow.progressBar.Dispatcher.Invoke(() => Commons.mainWindow.indicatorLabel.Content = "Ready", DispatcherPriority.Background);
        }

        private void TrainGenitive(TrainingSet set)
        {
            double max = 0d;

            _genitiveNetwork.EndEpochEvent += delegate (object network, TrainingEpochEventArgs args)
            {
                errorList[args.TrainingIteration] = _genitiveNetwork.MeanSquaredError;
                max = Math.Max(max, _genitiveNetwork.MeanSquaredError);
            };

            _genitiveNetwork.Learn(set, Constants.Cycles);
        }

        private void TrainDativeOrLocative(TrainingSet set)
        {
            double max = 0d;

            _dativeOrLocativeNetwork.EndEpochEvent += delegate (object network, TrainingEpochEventArgs args)
            {
                errorList[args.TrainingIteration] = _dativeOrLocativeNetwork.MeanSquaredError;
                max = Math.Max(max, _dativeOrLocativeNetwork.MeanSquaredError);
            };

            _dativeOrLocativeNetwork.Learn(set, Constants.Cycles);
        }

        private void TrainAccussative(TrainingSet set)
        {
            double max = 0d;

            _accussativeNetwork.EndEpochEvent += delegate (object network, TrainingEpochEventArgs args)
            {
                errorList[args.TrainingIteration] = _accussativeNetwork.MeanSquaredError;
                max = Math.Max(max, _accussativeNetwork.MeanSquaredError);
            };

            _accussativeNetwork.Learn(set, Constants.Cycles);
        }

        private void TrainVocative(TrainingSet set)
        {
            double max = 0d;

            _vocativeNetwork.EndEpochEvent += delegate (object network, TrainingEpochEventArgs args)
            {
                errorList[args.TrainingIteration] = _vocativeNetwork.MeanSquaredError;
                max = Math.Max(max, _vocativeNetwork.MeanSquaredError);
            };

            _vocativeNetwork.Learn(set, Constants.Cycles);
        }

        private void TrainInstrumental(TrainingSet set)
        {
            double max = 0d;

            _instrumentalNetwork.EndEpochEvent += delegate (object network, TrainingEpochEventArgs args)
            {
                errorList[args.TrainingIteration] = _instrumentalNetwork.MeanSquaredError;
                max = Math.Max(max, _instrumentalNetwork.MeanSquaredError);
            };

            _instrumentalNetwork.Learn(set, Constants.Cycles);
        }
        #endregion [Training]

        #region [Testing]
        public ResponseModel TestMultiple(string[] names)
        {
            ResponseModel model = new ResponseModel();
            model.Nouns = new List<NounModel>();

            foreach (var n in names)
            {
                model.Nouns.Add(TestSingle(n));
            }

            return model;
        }

        public NounModel TestSingle(string name)
        {
            List<double> inputs = EncodingHelper.EncodeInput(name);

            NounModel model = new NounModel()
            {
                Nominative = name,
                Genitive = EncodingHelper.DecodeOutput(_genitiveNetwork.Run(inputs.ToArray())).Trim('\0'),
                Dative = EncodingHelper.DecodeOutput(_dativeOrLocativeNetwork.Run(inputs.ToArray())).Trim('\0'),
                Accussative = EncodingHelper.DecodeOutput(_accussativeNetwork.Run(inputs.ToArray())).Trim('\0'),
                Vocative = EncodingHelper.DecodeOutput(_vocativeNetwork.Run(inputs.ToArray())).Trim().Trim('\0'),
                Instrumental = EncodingHelper.DecodeOutput(_instrumentalNetwork.Run(inputs.ToArray())).Trim('\0'),
                Locative = EncodingHelper.DecodeOutput(_dativeOrLocativeNetwork.Run(inputs.ToArray())).Trim('\0')
            };

            return model;
        }
        #endregion [Testing]
    }
}