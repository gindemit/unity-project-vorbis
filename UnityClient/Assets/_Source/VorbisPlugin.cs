using System.Runtime.InteropServices;

public static class VorbisPlugin
{
    [DllImport("VorbisPlugin")]
    private static extern int EncodePcmDataToFile(string filePath, float[] samples, int samplesLength, short channels, int frequency, float base_quality);

    [DllImport("VorbisPlugin")]
    private static extern int DecodePcmDataFromFile(string filePath, out System.IntPtr samples, out int samplesLength, out short channels, out int frequency);

    [DllImport("VorbisPlugin")]
    private static extern int FreeSamplesArrayNativeMemory(ref System.IntPtr samples);

    public static void Save(string filePath, UnityEngine.AudioClip audioClip, int channels = -1)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new System.ArgumentException("The file path is null or white space");
        }
        if (audioClip == null)
        {
            throw new System.ArgumentNullException(nameof(audioClip));
        }
        short finalChannelsCount = channels == -1 ? (short)audioClip.channels : (short)channels;
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
        EncodePcmDataToFile(filePath, pcm, pcm.Length, finalChannelsCount, audioClip.frequency, 0.4f);
    }

    public static UnityEngine.AudioClip Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new System.ArgumentException("The file path is null or white space");
        }
        if (!System.IO.File.Exists(filePath))
        {
            throw new System.IO.FileNotFoundException();
        }
        DecodePcmDataFromFile(filePath, out System.IntPtr pcmPtr, out int pcmLength, out short channels, out int frequency);
        float[] pcm = new float[pcmLength];
        Marshal.Copy(pcmPtr, pcm, 0, pcmLength);
        FreeSamplesArrayNativeMemory(ref pcmPtr);

        var audioClip = UnityEngine.AudioClip.Create("Test", pcmLength, channels, frequency, false);
        audioClip.SetData(pcm, 0);

        return audioClip;
    }
}
