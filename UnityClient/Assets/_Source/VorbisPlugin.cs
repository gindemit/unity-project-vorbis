using UnityEngine;
using System.Runtime.InteropServices;

public sealed class VorbisPlugin
{

    [DllImport("VorbisPlugin")]
    public static extern int EncodePcmDataToFile(string filePath, float[] samples, int samplesLength, short channels, int frequency, float base_quality);

    [DllImport("VorbisPlugin")]
    public static extern int DecodePcmDataFromFile(string filePath, out System.IntPtr samples, out int samplesLength, out short channels, out int frequency);

    [DllImport("VorbisPlugin")]
    public static extern int FreeSamplesArrayNativeMemory(ref System.IntPtr samples);
}
