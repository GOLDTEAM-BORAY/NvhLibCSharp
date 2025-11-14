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
    }
}
