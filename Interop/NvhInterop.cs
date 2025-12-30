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

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GetEnvelopeSpectra")]
        public static partial int HilbertEnvelopeSpectra(Signal signal, int formatType, ref IntPtr data, ref int outLength, ref IntPtr freqAxis, ref int freqBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "GetAvgEnvelopeSpectra")]
        public static partial int HilbertEnvelopeAvgSpectra(Signal signal, int segmentLength, double overlap, int formatType, int averageType, int weightType, int windowType, ref IntPtr data, ref int outLength, ref IntPtr freqAxis, ref int freqBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "MorletWaveletTransform")]
        public static partial int MorletWaveletTransform(Signal signal, IntPtr frequencyAxis, int frequencyBins, double nCycles, int scaleType, double referenceValue, ref IntPtr data, ref int timeBins, ref int freqBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "LmsMorletWaveletTransform")]
        public static partial int LmsMorletWaveletTransform(Signal signal, double minFreq, double maxFreq, int octave, int scaleType, double referenceValue, ref IntPtr data, ref int timeBins, ref IntPtr freqAxis, ref int freqBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "ModulationSpectrumAnalyze")]
        public static partial int ModulationSpectrumAnalyze(Signal signal, IntPtr frequencyAxis, int frequencyBins, int scaleType, double referenceValue, ref IntPtr data, ref int timeBins, ref IntPtr modulationDepth, ref IntPtr modulationFreq);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "StationaryLoudnessAnalyze")]
        public static partial int StationaryLoudnessAnalyze(Signal signal, int soundField, double skipInSec, ref double outLoudness, ref IntPtr outSpecLoudness, ref IntPtr outBarkAxis, ref int barkBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "TimeVaryingLoudnessAnalyze")]
        public static partial int TimeVaryingLoudnessAnalyze(Signal signal, int soundField, double skipInSec, ref IntPtr outLoudness, ref IntPtr outSpecLoudness, ref IntPtr outBarkAxis, ref IntPtr outTimeAxis, ref int barkBins, ref int timeBins);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "StationarySharpnessAnalyze")]
        public static partial int StationarySharpnessAnalyze(Signal signal, int sharpnessWeighting, int soundField, double skipInSec, ref double outSharpness);

        [LibraryImport("BrcSignalKit.dll", EntryPoint = "TimeVaryingSharpnessAnalyze")]
        public static partial int TimeVaryingSharpnessAnalyze(Signal signal, int sharpnessWeighting, int soundField, double skipInSec, ref IntPtr outSharpness, ref IntPtr outTimeAxis, ref int timeBins);
    }
}
