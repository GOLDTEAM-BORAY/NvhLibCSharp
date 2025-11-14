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

            var oaData = Nvh.OverallLevelSpectral(signal, 4096, 0.15, 2e-5, Window.Hanning, Weight.A, Scale.Linear, out var oaTimeAxis);
            for (int i = 0; i < oaTimeAxis.Length; i++)
            {
                Console.WriteLine($"{oaTimeAxis[i]:F6}\t{oaData[i]:F6}");
            }

            var otsData = Nvh.OrderSection(signal, rpm, 4096, 14.0, 0.5, 1000, 4000, 25, 2e-5, Format.Rms, Window.Hanning, Weight.A, Scale.Linear, RpmTrigger.Up, out var otsRpmAxis);
            for (int i = 0; i < otsRpmAxis.Length; i++)
            {
                Console.WriteLine($"{otsRpmAxis[i]:F6}\t{otsData[i]:F6}");
            }
        }
    }
}
