using NvhLibCSharp.Enums;
using NvhLibCSharp.Interop;
using NvhLibCSharp.Options;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace NvhLibCSharp
{
    /// <summary>
    /// 提供对 NVH 本机库功能的托管包装器方法。
    /// </summary>
    /// <remarks>
    /// 本类包含一组静态方法，用于调用位于 <c>NvhLibCSharp.Interop.NvhInterop</c> 的本机方法，
    /// 并处理非托管内存的拷贝与释放，以及将本机返回的错误码转换为托管异常。
    /// </remarks>
    public static class Nvh
    {
        /// <summary>
        /// 从指定路径加载 NVH 许可证文件。
        /// </summary>
        /// <param name="licensePath">许可证文件的完整路径。</param>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
        public static void LoadLicense(string licensePath)
        {
            var errCode = NvhInterop.LoadLicense(licensePath);
            Assert(errCode);
        }

        /// <summary>
        /// 计算整体声级的频谱（单次谱线集合）。
        /// </summary>
        /// <param name="signal">输入信号描述。</param>
        /// <param name="spectrumLines">用于频谱计算的谱线数。</param>
        /// <param name="increment">时间轴增量（秒），用于输出时间轴的采样间隔。</param>
        /// <param name="referenceValue">（用于 dB 计算的参考值）。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型（例如 A 权重、C 权重）。</param>
        /// <param name="scaleType">刻度类型（线性或对数等）。</param>
        /// <param name="timeAxis">输出的时间轴数组，长度与返回频谱数据相同，单位为秒。</param>
        /// <returns>
        /// 返回计算得到的频谱数据数组。数组长度等于本机返回的 bins 值。
        /// 数组索引 i 对应于 <paramref name="timeAxis"/> 中的时间点。
        /// </returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 计算给定转速范围与步长下的阶次截面（Order Section）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="rpm">表示转速数据信息的对象（可包含时间对齐信息）。</param>
        /// <param name="spectrumLines">频谱谱线数。</param>
        /// <param name="targetOrder">目标阶次（中心值）。</param>
        /// <param name="orderBandwidth">阶次带宽。</param>
        /// <param name="minRpm">最小转速（用于轴）。</param>
        /// <param name="maxRpm">最大转速（用于轴）。</param>
        /// <param name="rpmStep">转速步长。</param>
        /// <param name="referenceValue">参考值（用于dB 计算）。</param>
        /// <param name="formatType">频谱格式类型。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <param name="scaleType">刻度类型。</param>
        /// <param name="rpmTriggerType">转速触发类型。</param>
        /// <param name="rpmAxis">输出的转速轴数组（以 RPM 为单位），长度等于返回数组的元素数。</param>
        /// <returns>返回阶次截面数据数组，对应每个 rpm 轴点的幅值。</returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 计算平均线性自功率谱（Averaged AutoPower Linear Spectrum）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="calcOpt">频谱计算选项，用于决定谱线数/分辨率/帧长等。</param>
        /// <param name="stepOpt">步进选项，用于决定时间步进或重叠。</param>
        /// <param name="formatType">线性自功率谱（AutoPower Linear）幅值格式类型。</param>
        /// <param name="averageType">平均方式（算术平均/能量平均/最大平均）。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <returns>返回平均谱数据数组，长度由本机计算决定。</returns>
        /// <remarks>
        /// 根据 <see cref="SpectraCalcOptions.CalcType"/> 和 <see cref="SpectraStepOptions.StepType"/>，
        /// 会将选项转换为具体的谱线数和时间增量用于本机调用。
        /// </remarks>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
        public static double[] AveragedSpectrum(Signal signal, SpectraCalcOptions calcOpt, SpectraStepOptions stepOpt, ScaleOptions scaleOpt, Format formatType, Average averageType, Window windowType, Weight weightType)
        {
            var spectrumLines = calcOpt.CalcType switch
            {
                SpectraCalcType.SpectrumLines => calcOpt.CalcValue,
                SpectraCalcType.Resolution => 1 / (signal.DeltaTime * calcOpt.CalcValue * 2),
                SpectraCalcType.FrameLength => calcOpt.CalcValue / 2,
                _ => throw new InvalidOperationException("Invalid SpectraCalcType"),
            };

            var increment = stepOpt.StepType switch
            {
                SpectraStepType.Increment => stepOpt.StepValue,
                SpectraStepType.Overlap => (1 - stepOpt.StepValue) * (spectrumLines * signal.DeltaTime * 2),
                _ => throw new InvalidOperationException("Invalid SpectraStepType"),
            };

            IntPtr dataPtr = IntPtr.Zero;
            int bins = 0;
            int errCode = NvhInterop.AveragedSpectrum(signal, (int)spectrumLines, increment, (int)formatType, (int)averageType, (int)windowType, (int)weightType, ref dataPtr, ref bins);
            Assert(errCode);

            double[] data = new double[bins];
            Marshal.Copy(dataPtr, data, 0, bins);
            Marshal.FreeCoTaskMem(dataPtr);

            if (scaleOpt.Scale == Scale.Db)
            {
                double referenceValue = scaleOpt.ReferenceValue;
                for (int i = 0; i < bins; i++)
                {
                    // 避免对数计算中的零值或负值
                    var noneZeroValue = Math.Max(data[i] / referenceValue, 1e-20);
                    data[i] = 20.0 * Math.Log10(noneZeroValue);
                }
            }

            return data;
        }

        /// <summary>
        /// 生成时间-频率图（Time-Frequency Map）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="spectrumLines">谱线数。</param>
        /// <param name="increment">时间轴增量（秒）。</param>
        /// <param name="referenceValue">参考值（用于dB）。</param>
        /// <param name="formatType">格式类型。</param>
        /// <param name="windowType">窗类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <param name="scaleType">刻度类型。</param>
        /// <param name="timeAxis">输出时间轴，长度等于第一维（timeBins），单位为秒。</param>
        /// <param name="frequencyAxis">输出频率轴，长度等于第二维（frequencyBins），单位为赫兹。</param>
        /// <returns>
        /// 返回二维数组，维度为 [timeBins, frequencyBins]。数组的数据布局为按时间主序（第一维）按频率次序（第二维）。
        /// </returns>
        /// <remarks>
        /// 频率分辨率使用公式：1 / (signal.DeltaTime * spectrumLines * 2) 进行计算。
        /// </remarks>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 生成转速-频率图（RPM-Frequency Map）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="rpm">转速信号数据。</param>
        /// <param name="spectrumLines">谱线数。</param>
        /// <param name="minRpm">最小转速。</param>
        /// <param name="maxRpm">最大转速。</param>
        /// <param name="rpmStep">转速步长。</param>
        /// <param name="referenceValue">参考值（用于归一化或 dB）。</param>
        /// <param name="formatType">格式类型。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <param name="scaleType">刻度类型。</param>
        /// <param name="rpmTriggerType">转速触发类型。</param>
        /// <param name="rpmAxis">输出的转速轴数组，长度为 rpmBins。</param>
        /// <param name="frequencyAxis">输出的频率轴数组，长度为 frequencyBins，单位为赫兹。</param>
        /// <returns>返回二维数据，维度为 [rpmBins, frequencyBins]。</returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 计算转速-阶次图（RPM-Order Map）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="rpm">转速信息。</param>
        /// <param name="maxOrder">最大阶次。</param>
        /// <param name="orderResolution">阶次分辨率。</param>
        /// <param name="minRpm">最小转速。</param>
        /// <param name="maxRpm">最大转速。</param>
        /// <param name="rpmStep">转速步长。</param>
        /// <param name="referenceValue">参考值。</param>
        /// <param name="formatType">格式类型。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <param name="scaleType">刻度类型。</param>
        /// <param name="rpmAxis">输出转速轴数组。</param>
        /// <param name="orderAxis">输出阶次轴数组。</param>
        /// <returns>返回二维数组，维度为 [rpmBins, orderBins]，其中第二维为阶次轴。</returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 计算信号的希尔伯特包络（Hilbert Envelope）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <returns>返回包络线数组，长度由本机计算决定。</returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
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

        /// <summary>
        /// 计算指定信号的希尔伯特包络谱，并将谱数据作为 double 数组返回。
        /// </summary>
        /// <remarks>
        /// 返回的频谱与频率轴数组一一对应，谱中每个元素与频率轴相同索引处的频率值相匹配。
        /// 本方法为输出数组分配内存，内存由调用方负责管理。
        /// </remarks>
        /// <param name="signal">要分析的输入信号。不能为空。</param>
        /// <param name="format">计算频谱时使用的格式，指定输出的表示形式。</param>
        /// <param name="freqAxis">
        /// 方法返回时包含与频谱箱对应的频率值数组。该数组的长度与返回频谱的 bins 数相同。
        /// </param>
        /// <returns>
        /// 一个 double 数组，表示输入信号的希尔伯特包络谱。数组长度与频谱箱数一致。
        /// </returns>
        public static double[] HilbertEnvelopeSpectra(Signal signal, Format format, out double[] freqAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr freqAxisPtr = IntPtr.Zero;
            int outLength = 0;
            int bins = 0;

            int errCode = NvhInterop.HilbertEnvelopeSpectra(signal, (int)format, ref dataPtr, ref outLength, ref freqAxisPtr, ref bins);
            Assert(errCode);

            double[] data = new double[outLength];
            Marshal.Copy(dataPtr, data, 0, outLength);
            Marshal.FreeCoTaskMem(dataPtr);
            freqAxis = new double[bins];
            Marshal.Copy(freqAxisPtr, freqAxis, 0, bins);
            Marshal.FreeCoTaskMem(freqAxisPtr);

            return data;
        }

        /// <summary>
        /// 计算指定信号的希尔伯特包络平均谱，并将谱数据作为 double 数组返回。
        /// </summary>
        /// <param name="signal">要分析的输入信号。不能为空。</param>
        /// <param name="calcOpt">频谱计算选项，用于决定谱线数/分辨率/帧长等。</param>
        /// <param name="stepOpt">步进选项，用于决定时间步进或重叠。</param>
        /// <param name="formatType">线性自功率谱（AutoPower Linear）幅值格式类型。</param>
        /// <param name="averageType">平均方式（算术平均/能量平均/最大平均）。</param>
        /// <param name="windowType">窗函数类型。</param>
        /// <param name="weightType">加权类型。</param>
        /// <param name="freqAxis">频率轴</param>
        /// <returns>一个 double 数组，表示输入信号的希尔伯特包络谱。</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static double[] HilbertEnvelopeAvgSpectra(Signal signal, SpectraCalcOptions calcOpt, SpectraStepOptions stepOpt, Format formatType, Average averageType, Window windowType, Weight weightType, out double[] freqAxis)
        {
            var segmentLength = calcOpt.CalcType switch
            {
                SpectraCalcType.SpectrumLines => calcOpt.CalcValue * 2,
                SpectraCalcType.Resolution => 1 / (signal.DeltaTime * calcOpt.CalcValue),
                SpectraCalcType.FrameLength => calcOpt.CalcValue,
                _ => throw new InvalidOperationException("Invalid SpectraCalcType"),
            };

            var overlap = stepOpt.StepType switch
            {
                SpectraStepType.Increment => 1 - stepOpt.StepValue / segmentLength / signal.DeltaTime,
                SpectraStepType.Overlap => stepOpt.StepValue,
                _ => throw new InvalidOperationException("Invalid SpectraStepType"),
            };

            IntPtr dataPtr = IntPtr.Zero;
            IntPtr freqAxisPtr = IntPtr.Zero;
            int outLength = 0;
            int bins = 0;

            int errCode = NvhInterop.HilbertEnvelopeAvgSpectra(signal, (int)segmentLength, overlap, (int)formatType, (int)averageType, (int)weightType, (int)windowType, ref dataPtr, ref outLength, ref freqAxisPtr, ref bins);
            Assert(errCode);

            double[] data = new double[outLength];
            Marshal.Copy(dataPtr, data, 0, outLength);
            Marshal.FreeCoTaskMem(dataPtr);

            freqAxis = new double[bins];
            Marshal.Copy(freqAxisPtr, freqAxis, 0, bins);
            Marshal.FreeCoTaskMem(freqAxisPtr);

            return data;
        }

        /// <summary>
        /// 使用 Morlet 小波对指定频率轴进行小波变换。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="scaleOpt">dB/Lin选项，指定输出数据的db/Lin类型和参考值。</param>
        /// <param name="frequencyAxis">要分析的频率轴数组（赫兹）。</param>
        /// <param name="nCycles">小波的循环数，控制时间-频率分辨率权衡。</param>
        /// <param name="timeAxis">输出时间轴，单位为秒。</param>
        /// <returns>
        /// 返回二维数组，维度为 [frequencyBins, timeBins]，其中 frequencyBins 等于 <paramref name="frequencyAxis"/> 的长度。
        /// 数组按频率主序（第一维）按时间次序（第二维）。
        /// </returns>
        /// <remarks>
        /// 方法会为传入的频率轴在非托管内存中分配临时缓冲区并在完成后释放。
        /// </remarks>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
        public static double[,] MorletWaveletTransform(Signal signal, ScaleOptions scaleOpt, double[] frequencyAxis, double nCycles, out double[] timeAxis)
        {
            IntPtr freqAxisPtr = Marshal.AllocCoTaskMem(frequencyAxis.Length * sizeof(double));
            Marshal.Copy(frequencyAxis, 0, freqAxisPtr, frequencyAxis.Length);

            IntPtr dataPtr = IntPtr.Zero;
            int timeBins = 0;
            int freqBins = 0;
            int errCode = NvhInterop.MorletWaveletTransform(signal, freqAxisPtr, frequencyAxis.Length, nCycles, (int)scaleOpt.Scale, scaleOpt.ReferenceValue, ref dataPtr, ref timeBins, ref freqBins);
            Assert(errCode);

            Marshal.FreeCoTaskMem(freqAxisPtr);
            double[,] data = new double[freqBins, timeBins];
            double[] flatData = new double[timeBins * freqBins];
            Marshal.Copy(dataPtr, flatData, 0, timeBins * freqBins);
            Marshal.FreeCoTaskMem(dataPtr);

            for (int i = 0; i < freqBins; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    data[i, j] = flatData[i * timeBins + j];
                }
            }

            timeAxis = new double[timeBins];
            for (int i = 0; i < timeBins; i++)
            {
                timeAxis[i] = i * signal.DeltaTime;
            }
            return data;
        }

        /// <summary>
        /// 以对数/倍频程方式（LMS）计算 Morlet 小波变换并返回频率轴。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="scaleOpt">dB/Lin选项，指定输出数据的db/Lin类型和参考值。</param>
        /// <param name="minFrequency">最小频率（Hz）。</param>
        /// <param name="maxFrequency">最大频率（Hz）。</param>
        /// <param name="octave">每倍频程的划分数（分辨率）。</param>
        /// <param name="timeAxis">输出时间轴，单位为秒。</param>
        /// <param name="frequencyAxis">输出频率轴数组，表示每个频带的中心频率（Hz）。</param>
        /// <returns>返回二维数组，维度为 [frequencyBins, timeBins]。</returns>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
        public static double[,] LmsMorletWaveletTransform(Signal signal, ScaleOptions scaleOpt, double minFrequency, double maxFrequency, int octave, out double[] timeAxis, out double[] frequencyAxis)
        {
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr frequencyBinsPtr = IntPtr.Zero;
            int timeBins = 0;
            int freqBins = 0;
            int errCode = NvhInterop.LmsMorletWaveletTransform(signal, minFrequency, maxFrequency, octave, (int)scaleOpt.Scale, scaleOpt.ReferenceValue, ref dataPtr, ref timeBins, ref frequencyBinsPtr, ref freqBins);
            Assert(errCode);

            double[,] data = new double[freqBins, timeBins];
            double[] flatData = new double[timeBins * freqBins];
            Marshal.Copy(dataPtr, flatData, 0, timeBins * freqBins);
            Marshal.FreeCoTaskMem(dataPtr);

            frequencyAxis = new double[freqBins];
            Marshal.Copy(frequencyBinsPtr, frequencyAxis, 0, freqBins);
            Marshal.FreeCoTaskMem(frequencyBinsPtr);

            for (int i = 0; i < freqBins; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    data[i, j] = flatData[i * timeBins + j];
                }
            }

            timeAxis = new double[timeBins];
            for (int i = 0; i < timeBins; i++)
            {
                timeAxis[i] = i * signal.DeltaTime;
            }

            return data;
        }

        /// <summary>
        /// 对给定频率轴执行调制谱分析（Modulation Spectrum Analysis）。
        /// </summary>
        /// <param name="signal">输入信号。</param>
        /// <param name="scaleOpt">dB/Lin选项，指定输出数据的db/Lin类型和参考值。</param>
        /// <param name="frequencyAxis">要分析的频率轴数组（赫兹）。</param>
        /// <param name="modulationDepth">输出的调制深度数组，对应时间轴的每个点。</param>
        /// <param name="modulationFreq">输出的调制频率数组，对应时间轴的每个点（赫兹）。</param>
        /// <returns>
        /// 返回二维数组，维度为 [frequencyAxis.Length, timeBins]，表示在每个频率上随时间变化的调制强度。
        /// </returns>
        /// <remarks>
        /// 本方法在非托管内存中为频率轴分配缓冲区并在完成后释放；输出的 <paramref name="modulationDepth"/> 与 <paramref name="modulationFreq"/> 长度等于时间箱数（timeBins）。
        /// </remarks>
        /// <exception cref="InvalidOperationException">当本机库返回错误码时抛出，消息来自 <see cref="GetLastErrorMessage(int)"/>。</exception>
        public static double[,] ModulationSpectrumAnalysis(Signal signal, ScaleOptions scaleOpt, double[] frequencyAxis, out double[] modulationDepth, out double[] modulationFreq)
        {
            IntPtr freqAxisPtr = Marshal.AllocCoTaskMem(frequencyAxis.Length * sizeof(double));
            Marshal.Copy(frequencyAxis, 0, freqAxisPtr, frequencyAxis.Length);
            IntPtr dataPtr = IntPtr.Zero;
            IntPtr modulationDepthPtr = IntPtr.Zero;
            IntPtr modulationFreqPtr = IntPtr.Zero;
            int timeBins = 0;
            int errCode = NvhInterop.ModulationSpectrumAnalyze(signal, freqAxisPtr, frequencyAxis.Length, (int)scaleOpt.Scale, scaleOpt.ReferenceValue, ref dataPtr, ref timeBins, ref modulationDepthPtr, ref modulationFreqPtr);
            Assert(errCode);
            Marshal.FreeCoTaskMem(freqAxisPtr);
            double[,] data = new double[frequencyAxis.Length, timeBins];
            double[] flatData = new double[timeBins * frequencyAxis.Length];
            Marshal.Copy(dataPtr, flatData, 0, timeBins * frequencyAxis.Length);
            Marshal.FreeCoTaskMem(dataPtr);
            for (int i = 0; i < frequencyAxis.Length; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    data[i, j] = flatData[i * timeBins + j];
                }
            }
            modulationDepth = new double[timeBins];
            Marshal.Copy(modulationDepthPtr, modulationDepth, 0, timeBins);
            Marshal.FreeCoTaskMem(modulationDepthPtr);
            modulationFreq = new double[timeBins];
            Marshal.Copy(modulationFreqPtr, modulationFreq, 0, timeBins);
            Marshal.FreeCoTaskMem(modulationFreqPtr);
            return data;
        }

        /// <summary>
        /// 分析音频信号的稳态响度，并返回整体响度以及在 Bark 频带上的特定响度分布。
        /// </summary>
        /// <remarks>
        /// 此方法基于指定的声场执行稳态（时间不变）响度分析。特定响度数组表示在关键频带上的响度分布，可用于进一步的听觉感知分析。
        /// </remarks>
        /// <param name="signal">待分析的输入音频信号，不能为空。</param>
        /// <param name="soundField">用于分析的声场配置，例如自由场或漫射场。</param>
        /// <param name="skipInSec">分析开始前从信号起始处跳过的时长（秒），必须大于或等于 0。</param>
        /// <param name="barkAxis">输出的 Bark 频带轴数组，表示每个频带的中心频率（Bark）。数组长度对应分析得到的 Bark 频带数量。</param>
        /// <returns>
        /// 返回一个元组，包含整体响度值以及每个 Bark 频带的特定响度数组。数组长度对应分析得到的 Bark 频带数量。
        /// </returns>
        public static (double Loudness, double[] SpecLoudness) StationaryLoudnessAnalyze(Signal signal, SoundField soundField, double skipInSec, out double[] barkAxis)
        {
            double loudness = double.NaN;
            IntPtr specLoudnessPtr = IntPtr.Zero;
            IntPtr barkAxisPtr = IntPtr.Zero;
            int barkBins = int.MinValue;

            int errCode = NvhInterop.StationaryLoudnessAnalyze(signal, (int)soundField, skipInSec, ref loudness, ref specLoudnessPtr, ref barkAxisPtr, ref barkBins);
            Assert(errCode);

            double[] specLoudness = new double[barkBins];
            barkAxis = new double[barkBins];

            Marshal.Copy(specLoudnessPtr, specLoudness, 0, barkBins);
            Marshal.Copy(barkAxisPtr, barkAxis, 0, barkBins);

            Marshal.FreeCoTaskMem(specLoudnessPtr);
            Marshal.FreeCoTaskMem(barkAxisPtr);
            return (loudness, specLoudness);
        }

        /// <summary>
        /// 分析音频信号的时变响度，返回整体响度以及在时间与 Bark 频带上的特定响度数组。
        /// </summary>
        /// <remarks>
        /// 返回的特定响度数组按时间主序存储：每个时间帧的所有 Bark 频带响度值连续排列。
        /// 本方法会分配并填充输出数组；调用方需结合 Bark 轴和时间轴解释结果。
        /// </remarks>
        /// <param name="signal">待分析的输入音频信号。</param>
        /// <param name="soundField">用于分析的声场配置，例如自由场或漫射场。</param>
        /// <param name="skipInSec">分析前从信号起始处跳过的时长（秒），必须大于或等于 0。</param>
        /// <param name="barkAxis">方法返回时包含 Bark 频率轴（Bark），长度对应 Bark 频带数量。</param>
        /// <param name="timeAxis">方法返回时包含分析的时间轴（秒），长度对应时间帧数量。</param>
        /// <returns>
        /// 返回一个元组：第一个数组为每个时间帧的整体响度，第二个数组为按行主序展平的特定响度（时间帧 × Bark 频带）。
        /// </returns>
        public static (double[] Loudness, double[,] SpecLoudness) TimeVaryingLoudnessAnalyze(Signal signal, SoundField soundField, double skipInSec, out double[] barkAxis, out double[] timeAxis)
        {
            IntPtr loudnessPtr = IntPtr.Zero;
            IntPtr specLoudnessPtr = IntPtr.Zero;
            IntPtr barkAxisPtr = IntPtr.Zero;
            IntPtr timeAxisPtr = IntPtr.Zero;
            int timeBins = int.MinValue;
            int barkBins = int.MinValue;

            var errCode = NvhInterop.TimeVaryingLoudnessAnalyze(signal, (int)soundField, skipInSec, ref loudnessPtr, ref specLoudnessPtr, ref barkAxisPtr, ref timeAxisPtr, ref barkBins, ref timeBins);
            Assert(errCode);

            double[] loudness = new double[timeBins];
            double[] flatSpacLoudness = new double[timeBins * barkBins];
            double[,] specLoudness = new double[barkBins, timeBins];

            barkAxis = new double[barkBins];
            timeAxis = new double[timeBins];

            Marshal.Copy(loudnessPtr, loudness, 0, timeBins);
            Marshal.Copy(specLoudnessPtr, flatSpacLoudness, 0, timeBins * barkBins);
            Marshal.Copy(barkAxisPtr, barkAxis, 0, barkBins);
            Marshal.Copy(timeAxisPtr, timeAxis, 0, timeBins);
            Marshal.FreeCoTaskMem(loudnessPtr);
            Marshal.FreeCoTaskMem(specLoudnessPtr);
            Marshal.FreeCoTaskMem(barkAxisPtr);
            Marshal.FreeCoTaskMem(timeAxisPtr);

            for (int i = 0; i < barkBins; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    specLoudness[i, j] = flatSpacLoudness[j * barkBins + i];
                }
            }

            return (loudness, specLoudness);
        }

        /// <summary>
        /// 使用指定的锐度加权和声场参数分析音频信号的稳态锐度。
        /// </summary>
        /// <remarks>
        /// 稳态锐度是一种心理声学指标，用于量化稳态声音的感知尖锐度。结果取决于所选的加权方式和声场。本方法不会修改输入信号。
        /// </remarks>
        /// <param name="signal">待分析的音频信号，必须包含有效的声音数据。</param>
        /// <param name="sharpnessWeighting">用于分析的锐度加权方法，决定所采用的感知模型。</param>
        /// <param name="soundField">执行分析的声场条件，用于描述音频环境的空间特性。</param>
        /// <param name="skipInSec">开始分析前在信号起始处跳过的时长（秒），必须大于或等于 0。</param>
        /// <returns>
        /// 返回计算得到的稳态锐度值（单位：acum）。如果无法执行分析则返回 NaN。
        /// </returns>
        public static double StationarySharpnessAnalyze(Signal signal, SharpnessWeighting sharpnessWeighting, SoundField soundField, double skipInSec)
        {
            double sharpness = double.NaN;

            NvhInterop.StationarySharpnessAnalyze(signal, (int)sharpnessWeighting, (int)soundField, skipInSec, ref sharpness);
            return sharpness;
        }

        /// <summary>
        /// 使用指定的锐度加权和声场设置分析信号的时变锐度。
        /// </summary>
        /// <remarks>
        /// 返回的锐度数组长度与时间轴数组长度一致。该方法通常用于评估音频信号中感知锐度随时间的变化。
        /// </remarks>
        /// <param name="signal">要分析的输入信号，不能为空。</param>
        /// <param name="sharpnessWeighting">分析时应用的锐度加权方法。</param>
        /// <param name="soundField">用于分析的声场配置。</param>
        /// <param name="skipInSec">分析开始前在信号起始处跳过的时长（秒），必须大于或等于 0。</param>
        /// <param name="timeAxis">方法返回时包含与每个锐度测量值对应的时间数组（秒）。</param>
        /// <returns>表示信号每个时间点锐度的 double 数组。</returns>
        public static double[] TimeVaryingSharpnessAnalyze(Signal signal, SharpnessWeighting sharpnessWeighting, SoundField soundField, double skipInSec, out double[] timeAxis)
        {
            IntPtr sharpnessPtr = IntPtr.Zero;
            IntPtr timeAxisPtr = IntPtr.Zero;
            int timeBins = int.MinValue;
            NvhInterop.TimeVaryingSharpnessAnalyze(signal, (int)sharpnessWeighting, (int)soundField, skipInSec, ref sharpnessPtr, ref timeAxisPtr, ref timeBins);
            double[] sharpness = new double[timeBins];
            timeAxis = new double[timeBins];
            Marshal.Copy(sharpnessPtr, sharpness, 0, timeBins);
            Marshal.Copy(timeAxisPtr, timeAxis, 0, timeBins);
            Marshal.FreeCoTaskMem(sharpnessPtr);
            Marshal.FreeCoTaskMem(timeAxisPtr);
            return sharpness;
        }

        /// <summary>
        /// 分析音频信号的粗糙度，并返回整体粗糙度值，以及随时间变化的粗糙度和粗糙度谱数据。
        /// </summary>
        /// <remarks>
        /// 此方法根据指定的声场执行感知粗糙度分析。输出数组提供了详细的时域和频域信息，可用于进一步的信号特征描述或可视化。
        /// 该方法会分配并填充输出数组；调用方在调用前无需初始化它们。
        /// </remarks>
        /// <param name="signal">要分析粗糙度特性的输入音频信号。</param>
        /// <param name="soundField">用于分析的声场配置。指定听觉条件，例如单耳或双耳。</param>
        /// <param name="skipInSec">开始分析前从信号起始处跳过的时长（秒）。必须大于或等于零。</param>
        /// <param name="roughnessTimeDep">方法返回时，包含为信号每个时间帧计算的粗糙度值数组。</param>
        /// <param name="roughnessSpec">方法返回时，包含表示跨频带和时间帧的粗糙度谱的二维数组。</param>
        /// <param name="roughnessSpecAvg">方法返回时，包含整个信号每个频带的平均粗糙度值数组。</param>
        /// <param name="bandAxis">方法返回时，包含对应于频谱分析的频带中心值数组。</param>
        /// <param name="timeAxis">方法返回时，包含分析中对应每个时间帧的时间值数组（秒）。</param>
        /// <returns>输入信号的整体粗糙度值，基于所有时间帧和频带计算得出。</returns>
        public static double RoughnessAnalyze(Signal signal, SoundField soundField, double skipInSec, out double[] roughnessTimeDep, out double[,] roughnessSpec, out double[] roughnessSpecAvg, out double[] bandAxis, out double[] timeAxis)
        {
            double roughness = double.NaN;
            IntPtr roughnessTimeDepPtr = IntPtr.Zero;
            IntPtr roughnessSpecPtr = IntPtr.Zero;
            IntPtr roughnessSpecAvgPtr = IntPtr.Zero;
            IntPtr bandAxisPtr = IntPtr.Zero;
            int bandBins = int.MinValue;
            IntPtr timeAxisPtr = IntPtr.Zero;
            int timeBins = int.MinValue;

            NvhInterop.RoughnessAnalyze(signal, (int)soundField, skipInSec, ref roughness, ref roughnessTimeDepPtr, ref roughnessSpecPtr, ref roughnessSpecAvgPtr, ref bandAxisPtr, ref bandBins, ref timeAxisPtr, ref timeBins);

            roughnessTimeDep = new double[timeBins];
            Marshal.Copy(roughnessTimeDepPtr, roughnessTimeDep, 0, timeBins);

            double[] flatRoughnessSpec = new double[bandBins * timeBins];
            Marshal.Copy(roughnessSpecPtr, flatRoughnessSpec, 0, bandBins * timeBins);
            roughnessSpec = new double[bandBins, timeBins];
            for (int i = 0; i < bandBins; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    roughnessSpec[i, j] = flatRoughnessSpec[j * bandBins + i];
                }
            }

            roughnessSpecAvg = new double[bandBins];
            Marshal.Copy(roughnessSpecAvgPtr, roughnessSpecAvg, 0, bandBins);

            bandAxis = new double[bandBins];
            Marshal.Copy(bandAxisPtr, bandAxis, 0, bandBins);

            timeAxis = new double[timeBins];
            Marshal.Copy(timeAxisPtr, timeAxis, 0, timeBins);

            Marshal.FreeCoTaskMem(roughnessTimeDepPtr);
            Marshal.FreeCoTaskMem(roughnessSpecPtr);
            Marshal.FreeCoTaskMem(roughnessSpecAvgPtr);
            Marshal.FreeCoTaskMem(bandAxisPtr);
            Marshal.FreeCoTaskMem(timeAxisPtr);

            return roughness;
        }
        
        /// <summary>
        /// 使用指定的方法分析指定信号的波动强度，并返回随时间变化的波动强度值。
        /// </summary>
        /// <remarks>输出数组由该方法分配并填充。返回数组的长度和维度取决于输入信号和所选的分析方法。此方法是线程安全的。</remarks>
        /// <param name="signal">要进行波动强度分析的输入信号。</param>
        /// <param name="method">应用于信号的波动分析方法。</param>
        /// <param name="fluctuationSpec">当此方法返回时，包含一个二维数组，表示跨频带和时间帧的波动强度频谱。</param>
        /// <param name="fluctuationSpecAvg">当此方法返回时，包含一个数组，表示每个频带的平均波动强度。</param>
        /// <param name="bandAxis">当此方法返回时，包含一个频带值数组，对应于波动频谱的频率轴。</param>
        /// <param name="timeAxis">当此方法返回时，包含一个时间值数组，对应于波动频谱的时间帧。</param>
        /// <returns>一个 double 数组，表示输入信号随时间变化的波动强度。</returns>
        public static double[] FluctuationStrengthAnalyze(Signal signal, FluctuationMethod method, out double[,] fluctuationSpec, out double[] fluctuationSpecAvg, out double[] bandAxis, out double[] timeAxis)
        {
            IntPtr fluctuationTimeDepPtr = IntPtr.Zero;
            IntPtr fluctuationSpecPtr = IntPtr.Zero;
            IntPtr fluctuationSpecAvgPtr = IntPtr.Zero;
            IntPtr bandAxisPtr = IntPtr.Zero;
            int bandBins = int.MinValue;
            IntPtr timeAxisPtr = IntPtr.Zero;
            int timeBins = int.MinValue;

            NvhInterop.FluctuationStrengthAnalyze(signal, (int)method, ref fluctuationTimeDepPtr, ref fluctuationSpecPtr, ref fluctuationSpecAvgPtr, ref bandAxisPtr, ref bandBins, ref timeAxisPtr, ref timeBins);

            double[] fluctuationTimeDep = new double[timeBins];
            Marshal.Copy(fluctuationTimeDepPtr, fluctuationTimeDep, 0, timeBins);

            double[] flatFluctuationSpec = new double[bandBins * timeBins];
            Marshal.Copy(fluctuationSpecPtr, flatFluctuationSpec, 0, bandBins * timeBins);
            fluctuationSpec = new double[bandBins, timeBins];
            for (int i = 0; i < bandBins; i++)
            {
                for (int j = 0; j < timeBins; j++)
                {
                    fluctuationSpec[i, j] = flatFluctuationSpec[j * bandBins + i];
                }
            }

            fluctuationSpecAvg = new double[bandBins];
            Marshal.Copy(fluctuationSpecAvgPtr, fluctuationSpecAvg, 0, bandBins);

            bandAxis = new double[bandBins];
            Marshal.Copy(bandAxisPtr, bandAxis, 0, bandBins);

            timeAxis = new double[timeBins];
            Marshal.Copy(timeAxisPtr, timeAxis, 0, timeBins);

            Marshal.FreeCoTaskMem(fluctuationTimeDepPtr);
            Marshal.FreeCoTaskMem(fluctuationSpecPtr);
            Marshal.FreeCoTaskMem(fluctuationSpecAvgPtr);
            Marshal.FreeCoTaskMem(bandAxisPtr);
            Marshal.FreeCoTaskMem(timeAxisPtr);

            return fluctuationTimeDep;
        }

        /// <summary>
        /// 根据本机错误码检索可读的错误消息。
        /// </summary>
        /// <param name="errorCode">本机接口返回的错误码（负值代表错误）。</param>
        /// <returns>返回与错误码对应的人类可读错误消息字符串。如果无法从本机获取消息，返回空或默认消息。</returns>
        private static string GetLastErrorMessage(int errorCode)
        {
            return NvhInterop.GetLastErrorMessage(errorCode);
        }

        /// <summary>
        /// 检查本机返回码并在发生错误时抛出包含本机错误信息的 <see cref="InvalidOperationException"/>。
        /// </summary>
        /// <param name="ret">本机接口返回的整数值。非负值表示成功，负值表示错误。</param>
        /// <exception cref="InvalidOperationException">当 <paramref name="ret"/> 为负值时抛出，异常消息通过 <see cref="GetLastErrorMessage(int)"/> 获取。</exception>
        private static void Assert(int ret)
        {
            if (ret >= 0) return;
            
            throw new InvalidOperationException(GetLastErrorMessage(ret));
        }
    }
}
