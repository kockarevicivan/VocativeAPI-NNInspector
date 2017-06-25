namespace ORI.NeuralNetworks
{
    public static class Constants
    {
        public static int Cycles = 5000;
        public static int NeuronCount = 15;
        public static int BitsPerLetter = 16;
        public static double LearningRate = 0.25d;
        public static int InterfaceNeuronCount = 20 * BitsPerLetter;//20 letters, 2 bytes per letter
    }
}
