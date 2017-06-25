using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORI.NeuralNetworks
{
    public static class EncodingHelper
    {
        public static List<double> EncodeInput(string input)
        {
            List<double> retVal = new List<double>();

            foreach (char c in input)
            {
                string binaryString = NormalizeCharacter(c);

                foreach (var bc in binaryString)
                {
                    if (bc == '0')
                        retVal.Add(0);
                    else
                        retVal.Add(1);
                }
            }

            for (int i = retVal.Count; i < Constants.InterfaceNeuronCount; i++)
            {
                retVal.Add(0);
            }

            return retVal;
        }

        public static string DecodeOutput(double[] output)
        {
            StringBuilder final = new StringBuilder();
            StringBuilder small = new StringBuilder();

            int count = 0;
            for (int i = 0; i < output.Count(); i++)
            {
                small.Append(Math.Round(output[i]));
                count++;

                if (count == 16)
                {
                    final.Append((char)Convert.ToInt32(small.ToString(), 2));
                    small.Clear();
                    count = 0;
                }
            }

            return final.ToString();
        }

        public static string NormalizeCharacter(char c)
        {
            return Convert.ToString(c, 2).PadLeft(Constants.BitsPerLetter, '0');
        }
    }
}
