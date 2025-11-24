using NvhLibCSharp.Interop;
using System.Runtime.InteropServices;

namespace NvhLibCSharp
{
    public static class Nvh
    {
        public static void LoadLicense(string licensePath)
        {
            var errCode = NvhInterop.LoadLicense(licensePath);
            Assert(errCode);
        }

        public static double[] OverallLevelSpectral(Signal signal, int spectrumLines, double increment, double referenceValue, Window windowType, Weight weightType, Scale scaleType, out double[] timeAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            int bins = 0;
            int errCode = NvhInterop.OverallLevelSpectral(signal, spectrumLines, increment, referenceValue, (int)windowType, (int)weightType, (int)scaleType, ref dataPtr, ref bins);
            Assert(errCode);

            double[] data = new double[bins];
            Marshal.Copy(dataPtr, data, 0, bins);
            Marshal.FreeCoTaskMem(dataPtr);

            timeAxis = new double[bins];
            for (int i = 0; i < bins; i++)
            {
                timeAxis[i] = i * increment;
            }

            return data;
        }

        public static double[] OrderSection(Signal signal, Rpm rpm, int spectrumLines, double targetOrder, double orderBandwidth, double minRpm, double maxRpm, double rpmStep, double referenceValue, Format formatType, Window windowType, Weight weightType, Scale scaleType, RpmTrigger rpmTriggerType, out double[] rpmAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr rpmAxisPtr = IntPtr.Zero;
            int bins = 0;
            int errCode = NvhInterop.OrderSection(signal, rpm, spectrumLines, targetOrder, orderBandwidth, minRpm, maxRpm, rpmStep, referenceValue, (int)formatType, (int)windowType, (int)weightType, (int)scaleType, (int)rpmTriggerType, ref dataPtr, ref rpmAxisPtr, ref bins);
            Assert(errCode);

            double[] data = new double[bins];
            Marshal.Copy(dataPtr, data, 0, bins);
            Marshal.FreeCoTaskMem(dataPtr);

            rpmAxis = new double[bins];
            Marshal.Copy(rpmAxisPtr, rpmAxis, 0, bins);
            Marshal.FreeCoTaskMem(rpmAxisPtr);

            return data;
        }

        public static double[] AveragedSpectrum(Signal signal, int spectrumLines, double increment, Format formatType, Average averageType, Window windowType, Weight weightType)
        {
            IntPtr dataPtr = IntPtr.Zero;
            int bins = 0;
            int errCode = NvhInterop.AveragedSpectrum(signal, spectrumLines, increment, (int)formatType, (int)averageType, (int)windowType, (int)weightType, ref dataPtr, ref bins);
            Assert(errCode);
            double[] data = new double[bins];
            Marshal.Copy(dataPtr, data, 0, bins);
            Marshal.FreeCoTaskMem(dataPtr);
            return data;
        }

        public static double[,] TimeFrequencyMap(Signal signal, int spectrumLines, double increment, double referenceValue, Format formatType, Window windowType, Weight weightType, Scale scaleType, out double[] timeAxis, out double[] frequencyAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            int timeBins = 0;
            int frequencyBins = 0;
            int errCode = NvhInterop.TimeFrequencyMap(signal, spectrumLines, increment, 0.0, -1.0, referenceValue, (int)formatType, (int)windowType, (int)weightType, (int)scaleType, ref dataPtr, ref timeBins, ref frequencyBins);
            Assert(errCode);
            double[,] data = new double[timeBins, frequencyBins];
            double[] flatData = new double[timeBins * frequencyBins];
            Marshal.Copy(dataPtr, flatData, 0, timeBins * frequencyBins);
            Marshal.FreeCoTaskMem(dataPtr);
            for (int i = 0; i < timeBins; i++)
            {
                for (int j = 0; j < frequencyBins; j++)
                {
                    data[i, j] = flatData[i * frequencyBins + j];
                }
            }
            timeAxis = new double[timeBins];
            for (int i = 0; i < timeBins; i++)
            {
                timeAxis[i] = i * increment;
            }
            frequencyAxis = new double[frequencyBins];
            double frequencyResolution = 1.0 / (signal.DeltaTime * spectrumLines * 2);
            for (int j = 0; j < frequencyBins; j++)
            {
                frequencyAxis[j] = j * frequencyResolution;
            }
            return data;
        }

        public static double[,] RpmFrequencyMap(Signal signal, Rpm rpm, int spectrumLines, double minRpm, double maxRpm, double rpmStep, double referenceValue, Format formatType, Window windowType, Weight weightType, Scale scaleType, RpmTrigger rpmTriggerType, out double[] rpmAxis, out double[] frequencyAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr rpmAxisPtr = IntPtr.Zero;
            IntPtr frequencyAxisPtr = IntPtr.Zero;
            int rpmBins = 0;
            int frequencyBins = 0;
            int errCode = NvhInterop.RpmFrequencyMap(signal, rpm, spectrumLines, minRpm, maxRpm, rpmStep, referenceValue, (int)formatType, (int)windowType, (int)weightType, (int)scaleType, (int)rpmTriggerType, ref dataPtr, ref rpmAxisPtr, ref frequencyAxisPtr, ref rpmBins, ref frequencyBins);
            Assert(errCode);
            double[,] data = new double[rpmBins, frequencyBins];
            double[] flatData = new double[rpmBins * frequencyBins];
            Marshal.Copy(dataPtr, flatData, 0, rpmBins * frequencyBins);
            Marshal.FreeCoTaskMem(dataPtr);
            for (int i = 0; i < rpmBins; i++)
            {
                for (int j = 0; j < frequencyBins; j++)
                {
                    data[i, j] = flatData[i * frequencyBins + j];
                }
            }
            rpmAxis = new double[rpmBins];
            Marshal.Copy(rpmAxisPtr, rpmAxis, 0, rpmBins);
            Marshal.FreeCoTaskMem(rpmAxisPtr);
            frequencyAxis = new double[frequencyBins];
            Marshal.Copy(frequencyAxisPtr, frequencyAxis, 0, frequencyBins);
            Marshal.FreeCoTaskMem(frequencyAxisPtr);
            return data;
        }

        public static double[,] RpmOrderMap(Signal signal, Rpm rpm, double maxOrder, double orderResolution, double minRpm, double maxRpm, double rpmStep, double referenceValue, Format formatType, Window windowType, Weight weightType, Scale scaleType, out double[] rpmAxis, out double[] orderAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr rpmAxisPtr = IntPtr.Zero;
            IntPtr orderAxisPtr = IntPtr.Zero;
            int rpmBins = 0;
            int orderBins = 0;
            int errCode = NvhInterop.RpmOrderMap(signal, rpm, maxOrder, orderResolution, 1.0, minRpm, maxRpm, rpmStep, referenceValue, (int)formatType, (int)windowType, (int)weightType, (int)scaleType, ref dataPtr, ref rpmAxisPtr, ref orderAxisPtr, ref rpmBins, ref orderBins);
            Assert(errCode);
            double[,] data = new double[rpmBins, orderBins];
            double[] flatData = new double[rpmBins * orderBins];
            Marshal.Copy(dataPtr, flatData, 0, rpmBins * orderBins);
            Marshal.FreeCoTaskMem(dataPtr);
            for (int i = 0; i < rpmBins; i++)
            {
                for (int j = 0; j < orderBins; j++)
                {
                    data[i, j] = flatData[i * orderBins + j];
                }
            }
            rpmAxis = new double[rpmBins];
            Marshal.Copy(rpmAxisPtr, rpmAxis, 0, rpmBins);
            Marshal.FreeCoTaskMem(rpmAxisPtr);
            orderAxis = new double[orderBins];
            Marshal.Copy(orderAxisPtr, orderAxis, 0, orderBins);
            Marshal.FreeCoTaskMem(orderAxisPtr);
            return data;
        }

        public static double[] HilbertEnvelope(Signal signal)
        {
            IntPtr dataPtr = IntPtr.Zero;
            int bins = 0;
            int errCode = NvhInterop.HilbertEnvelope(signal, ref dataPtr, ref bins);
            Assert(errCode);
            double[] data = new double[bins];
            Marshal.Copy(dataPtr, data, 0, bins);
            Marshal.FreeCoTaskMem(dataPtr);
            return data;
        }

        private static string GetLastErrorMessage(int errorCode)
        {
            return NvhInterop.GetLastErrorMessage(errorCode);
        }

        private static void Assert(int ret)
        {
            if (ret >= 0) return;
            
            throw new InvalidOperationException(GetLastErrorMessage(ret));
        }
    }
}
