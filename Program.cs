using NvhLibCSharp.Interop;
using NvhLibCSharp.Utils;

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
            var asData = Nvh.AveragedSpectrum(signal, 4096, 0.15, Format.Rms, Average.Energy, Window.Hanning, Weight.A);
            var freqResolution = 1.0 / (signal.DeltaTime * 4096 * 2);
            for (int i = 0; i < asData.Length; i++)
            {
                Console.WriteLine($"{i * freqResolution:F6}\t{asData[i]:F6}");
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
        }
    }
}
