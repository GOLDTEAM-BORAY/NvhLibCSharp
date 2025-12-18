using NvhLibCSharp.Interop;
using NvhLibCSharp.Options;
using NvhLibCSharp.Utils;
using System.Xml.Linq;

namespace NvhLibCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Nvh.LoadLicense("D:\\测试\\LIC-20251114-3685ebd9.lic");

            var samples = LoadData.Double("D:\\source\\NvhLibCSharp\\SampleData\\sound_signal_0.txt");
            var rpmValues = LoadData.Double("D:\\source\\NvhLibCSharp\\SampleData\\speed_0.txt");

            var signal = new Signal(samples, 1.0 / 51200.0);
            var rpm = new Rpm(rpmValues, 1.0 / 51200.0);

#if false
            var oaData = Nvh.OverallLevelSpectral(signal, 4096, 0.15, 2e-5, Window.Hanning, Weight.A, Scale.Linear, out var oaTimeAxis);
            for (int i = 0; i < oaTimeAxis.Length; i++)
            {
                Console.WriteLine($"{oaTimeAxis[i]:F6}\t{oaData[i]:F6}");
            }
#endif

#if false
            var otsData = Nvh.OrderSection(signal, rpm, 4096, 14.0, 0.5, 1000, 4000, 25, 2e-5, Format.Rms, Window.Hanning, Weight.A, Scale.Linear, RpmTrigger.Up, out var otsRpmAxis);
            for (int i = 0; i < otsRpmAxis.Length; i++)
            {
                Console.WriteLine($"{otsRpmAxis[i]:F6}\t{otsData[i]:F6}");
            }
#endif

#if false
            var calcOpt = new SpectraCalcOptions(calcType: Enums.SpectraCalcType.Resolution, calcValue: 6.25);
            var stepOpt = new SpectraStepOptions(stepType: Enums.SpectraStepType.Increment, stepValue: 0.15);
            var scaleOpt = new ScaleOptions(Scale.Db, 2e-5);
            var asData = Nvh.AveragedSpectrum(signal, calcOpt, stepOpt, scaleOpt, Format.Rms, Average.Energy, Window.Hanning, Weight.A);
            var freqResolution = 6.25;
            for (int i = 0; i < asData.Length; i++)
            {
                Console.WriteLine($"{i * freqResolution:F6}\t{asData[i]:F6}");
            }
#endif

#if false
            var tfmData = Nvh.TimeFrequencyMap(signal, 4096, 0.15, 2e-5, Format.Rms, Window.Hanning, Weight.A, Scale.Linear, out var tfmTimeAxis, out var tfmFreqAxis);
            for (int i = 0; i < tfmTimeAxis.Length; i++)
            {
                for (int j = 0; j < tfmFreqAxis.Length; j++)
                {
                    Console.WriteLine($"{tfmTimeAxis[i]:F6}\t{tfmFreqAxis[j]:F6}\t{tfmData[i, j]:F6}");
                }
            }
#endif

#if false
            var rfmData = Nvh.RpmFrequencyMap(signal, rpm, 4096, 1000, 4000, 25, 2e-5, Format.Rms, Window.Hanning, Weight.A, Scale.Linear, RpmTrigger.Up, out var rfmRpmAxis, out var rfmFreqAxis);
            for (int i = 0; i < samples.Length; i++) 
            {
                for (int j = 0; j < rfmFreqAxis.Length; j++)
                {
                    Console.WriteLine($"{rfmRpmAxis[i]:F6}\t{rfmFreqAxis[j]:F6}\t{rfmData[i, j]:F6}");
                }
            }
#endif

#if false
            var romData = Nvh.RpmOrderMap(signal, rpm, 32.0, 0.25, 600, 4000, 25, 2e-5, Format.Rms, Window.Hanning, Weight.A, Scale.Linear, out var romRpmAxis, out var romOrderAxis);
            for (int i = 0; i < romRpmAxis.Length; i++)
            {
                for (int j = 0; j < romOrderAxis.Length; j++)
                {
                    Console.WriteLine($"{romRpmAxis[i]:F6}\t{romOrderAxis[j]:F6}\t{romData[i, j]:F6}");
                }
            }
#endif

#if false
            var heData = Nvh.HilbertEnvelope(signal);
            var timeResolution = signal.DeltaTime;
            for (int i = 0; i < heData.Length; i++)
            {
                Console.WriteLine($"{i * timeResolution:F6}\t{heData[i]:F6}");
            }
#endif

#if true
            var simpleSamples = LoadData.Double("SampleData/simple.txt");
            var simpleSignal = new Signal(simpleSamples, 1.0 / 1280.0);
            var frequencyAxis = MathUtils.Logspace(Math.Log10(1.0), Math.Log10(1280.0 / 2.0), 50);

            var tf = Nvh.MorletWaveletTransform(simpleSignal, [..frequencyAxis], 5, out var timeAxis);

            for (int i = 0; i < tf.GetLength(1); i++)
            {
                Console.WriteLine($"{timeAxis[i]:F6}\t{tf[0, i]:F6}");
            }
#endif

#if false
            var simpleSamples = LoadData.Double("SampleData/simple.txt");
            var simpleSignal = new Signal(simpleSamples, 1.0 / 1280.0);

            var tf = Nvh.LmsMorletWaveletTransform(simpleSignal, 10, 1000, 100, out var timeAxis, out var freqAxis);

            for (int i = 0; i < tf.GetLength(1); i++)
            {
                Console.WriteLine($"{timeAxis[i]:F6}\t{tf[0, i]:F6}");
            }
#endif

#if false
            var simpleModedSamples = LoadData.Double("SampleData/simple_modulated.txt");
            var simpleModedSignal = new Signal(simpleModedSamples, 1.0 / 1000.0);

            var frequencyAxis = MathUtils.Linspace(1, 100, 100);
            var tf = Nvh.ModulationSpectrumAnalysis(simpleModedSignal, [ ..frequencyAxis], out var modulationDepth, out var modulationFreq);

            for (int i = 0; i < modulationFreq.Length; i++)
            {
                Console.WriteLine($"{modulationFreq[i]:F6}\t{modulationDepth[i]:F6}");
            }
#endif
        }
    }
}
