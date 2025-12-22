using NvhLibCSharp.Enums;
using NvhLibCSharp.Interop;
using NvhLibCSharp.Options;
using System.Runtime.InteropServices;

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
