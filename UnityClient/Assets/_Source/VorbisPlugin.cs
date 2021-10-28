using System.IO;
using System.Runtime.InteropServices;

public static class VorbisPlugin
{
#if UNITY_IOS && !UNITY_EDITOR
    private const string PLUGIN_NAME = "__Internal";
#else
    private const string PLUGIN_NAME = "VorbisPlugin";
#endif

    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "write_all_pcm_data_to_file")]
    private static extern int WriteAllPcmDataToFile(
        string filePath,
        float[] samples,
        int samplesLength,
        short channels,
        int frequency,
        float base_quality,
        int samplesToRead);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "write_all_pcm_data_to_memory")]
    private static extern int WriteAllPcmDataToMemory(
        out System.IntPtr bytesPtr,
        out int bytesLength,
        float[] samples,
        int samplesLength,
        short channels,
        int frequency,
        float base_quality,
        int samplesToRead);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_memory_array_for_write_all_pcm_data")]
    private static extern int FreeMemoryArrayForWriteAllPcmData(
        System.IntPtr bytesNativeArray);

    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "read_all_pcm_data_from_file")]
    private static extern int ReadAllPcmDataFromFile(
        string filePath,
        out System.IntPtr samples,
        out int samplesLength,
        out short channels,
        out int frequency,
        int maxSamplesToRead);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "read_all_pcm_data_from_memory")]
    private static extern int ReadAllPcmDataFromMemory(
        byte[] memoryArray,
        int memoryArrayLength,
        out System.IntPtr samples,
        out int samplesLength,
        out short channels,
        out int frequency,
        int maxSamplesToRead);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_samples_array_native_memory")]
    private static extern int FreeSamplesArrayNativeMemory(
        ref System.IntPtr samples);

    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "open_read_file_stream")]
    public static extern System.IntPtr OpenReadFileStream(
        string filePath,
        out short channels,
        out int frequency);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "read_from_file_stream")]
    public static extern int ReadFromFileStream(
        System.IntPtr state,
        float[] samplesToFill,
        int maxSamplesToRead);
    [DllImport(PLUGIN_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "close_file_stream")]
    public static extern int CloseFileStream(
        System.IntPtr state);


    public static void Save(string filePath, UnityEngine.AudioClip audioClip, int samplesToRead = 1024)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new System.ArgumentException("The file path is null or white space");
        }
        if (audioClip == null)
        {
            throw new System.ArgumentNullException(nameof(audioClip));
        }
        if (samplesToRead <= 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(samplesToRead));
        }
        short finalChannelsCount = (short)audioClip.channels;
        if (finalChannelsCount != 1 && finalChannelsCount != 2)
        {
            throw new System.ArgumentException($"Only one or two channels are supported, provided channels count: {finalChannelsCount}");
        }
        if (!filePath.EndsWith(".ogg"))
        {
            filePath += ".ogg";
        }

        float[] pcm = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(pcm, 0);
        WriteAllPcmDataToFile(filePath, pcm, pcm.Length, finalChannelsCount, audioClip.frequency, 0.4f, samplesToRead);
    }

    public static UnityEngine.AudioClip Load(string filePath, int maxSamplesToRead = 1024)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new System.ArgumentException("The file path is null or white space");
        }
        if (maxSamplesToRead <= 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(maxSamplesToRead));
        }
        if (!System.IO.File.Exists(filePath))
        {
            throw new System.IO.FileNotFoundException();
        }
        ReadAllPcmDataFromFile(filePath, out System.IntPtr pcmPtr, out int pcmLength, out short channels, out int frequency, maxSamplesToRead);
        float[] pcm = new float[pcmLength];
        Marshal.Copy(pcmPtr, pcm, 0, pcmLength);
        FreeSamplesArrayNativeMemory(ref pcmPtr);

        UnityEngine.Debug.Log($"{pcmLength}, {channels}, {frequency}");
        var audioClip = UnityEngine.AudioClip.Create("Test", pcmLength, channels, frequency, false);
        audioClip.SetData(pcm, 0);

        return audioClip;
    }
}
