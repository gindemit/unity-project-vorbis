using UnityEngine;
using System.Runtime.InteropServices;

public sealed class VorbisPlugin
{

    [DllImport("VorbisPlugin")]
    public static extern int EncodePcmDataToFile(float[] samples, int samplesLength, short channels, int frequency, float base_quality, string filePath);
}
