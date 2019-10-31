using System.IO;
using UnityEngine;

public class StreamTest : MonoBehaviour
{
    private System.IntPtr _fileStream;

    private void OnEnable()
    {
        string workingDirectory = Path.Combine(Application.persistentDataPath, "VorbisPluginTest");

        string pathToFile = Path.Combine(workingDirectory, "vivaldi-summer-the-four-seasons.ogg");
        _fileStream = VorbisPlugin.OpenReadFileStream(pathToFile, out short channels, out int frequency);
        Debug.Log($"Successesfully opened filestream, channels: {channels}, frequency: {frequency}");
    }
    private void OnDisable()
    {
        VorbisPlugin.CloseFileStream(_fileStream);
        Debug.Log($"Successesfully closed filestream");
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        int requestedSamples = data.Length / channels;
        VorbisPlugin.ReadFromFileStream(_fileStream, data, requestedSamples);
    }
}
