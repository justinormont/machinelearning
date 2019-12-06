using System;

namespace Microsoft.ML.AutoML.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ImageClassificationExperiment.Run();

                Console.Clear();

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex}");
            }

            Console.ReadLine();
        }
    }
}
