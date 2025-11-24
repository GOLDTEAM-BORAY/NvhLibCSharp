using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace NvhLibCSharp.Interop
{
    public static partial class NvhInterop
    {
        [LibraryImport("BrcSignalKit.dll", EntryPoint = "LoadLicense", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
        public static partial int LoadLicense(string licensePath);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GetLastErrorMessage", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(AnsiStringMarshaller))]
        public static partial string GetLastErrorMessage(int errorCode);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "OverallLevelSpectral")]
        public static partial int OverallLevelSpectral(Signal signal, int spectrumLines, double increment, double referenceValue, int windowType, int weightType, int scaleType, ref IntPtr data, ref int bins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "OrderSection")]
        public static partial int OrderSection(Signal signal, Rpm rpm, int spectrumLines, double targetOrder, double orderBandwidth, double minRpm, double maxRpm, double rpmStep, double referenceValue, int formatType, int windowType, int weightType, int scaleType, int rpmTriggerType, ref IntPtr data, ref IntPtr rpmAxis, ref int bins);
        
        [LibraryImport("BrcSignalKit.dll", EntryPoint = "AveragedSpectrumByIncrement")]
        public static partial int AveragedSpectrum(Signal signal, int spectrumLines, double increment, int formatType, int averageType, int windowType, int weightType, ref IntPtr data, ref int bins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GenerateTimeFrequencyColormapByIncrement")]
        public static partial int TimeFrequencyMap(Signal signal, int spectrumLines, double increment, double startTime, double endTime, double referenceValue, int formatType, int windowType, int weightType, int scaleType, ref IntPtr data, ref int timeBins, ref int frequencyBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GenerateRpmFrequencyColormap")]
        public static partial int RpmFrequencyMap(Signal signal, Rpm rpm, int spectrumLines, double minRpm, double maxRpm, double rpmStep, double referenceValue, int formatType, int windowType, int weightType, int scaleType, int rpmTriggerType, ref IntPtr data, ref IntPtr rpmAxis, ref IntPtr frequencyAxis, ref int rpmBins, ref int frequencyBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GenerateRpmOrderColormap")]
        public static partial int RpmOrderMap(Signal signal, Rpm rpm, double maxOrder, double orderResolution, double oversamplingFactor, double minRpm, double maxRpm, double rpmStep, double referenceValue, int formatType, int windowType, int weightType, int scaleType, ref IntPtr data, ref IntPtr rpmAxis, ref IntPtr orderAxis, ref int rpmBins, ref int orderBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GetEnvelope")]
        public static partial int HilbertEnvelope(Signal signal, ref IntPtr data, ref int bins);
    }
}
