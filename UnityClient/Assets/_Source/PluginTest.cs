using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _toFill;

    private System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();

    public void SaveAudioClipToOgg()
    {
        float[] pcm = new float[_audioSource.clip.samples];
        _audioSource.clip.GetData(pcm, 0);

        string dirToSave = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirToSave);
        string pathToSave = Path.Combine(dirToSave, "1");

        Debug.Log($"Audio file length: {_audioSource.clip.length} sec.");
        _stopwatch.Restart();
        Presentation.Utility.Loader.WaveAudio.Save(pathToSave + ".wav", _audioSource.clip);
        Debug.Log($"Wave file save took {_stopwatch.ElapsedMilliseconds} ms.");
        _stopwatch.Restart();
        VorbisPlugin.EncodePcmDataToFile(pathToSave + ".ogg", pcm, pcm.Length, 1, 44100, 0.4f);
        Debug.Log($"Vorbis ogg file save took {_stopwatch.ElapsedMilliseconds} ms.");
        _stopwatch.Stop();

        Debug.Log(pathToSave);
    }

    public void LoadAudioClipFromOgg()
    {
        string dirFromLoad = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirFromLoad);
        string pathFromLoad = Path.Combine(dirFromLoad, "1");

        _stopwatch.Restart();
        byte[] wavBytes = File.ReadAllBytes(pathFromLoad + ".wav");
        Presentation.Utility.Loader.WaveAudio.ToAudioClip(wavBytes);
        Debug.Log($"Load wave file took {_stopwatch.ElapsedMilliseconds} ms.");
        _stopwatch.Restart();
        VorbisPlugin.DecodePcmDataFromFile(pathFromLoad + ".ogg", out System.IntPtr pcmPtr, out int pcmLength, out short channels, out int frequency);
        float[] pcm = new float[pcmLength];
        Marshal.Copy(pcmPtr, pcm, 0, pcmLength);
        VorbisPlugin.FreeSamplesArrayNativeMemory(ref pcmPtr);
        Debug.Log($"Load vorbis ogg file took {_stopwatch.ElapsedMilliseconds} ms.");

        AudioClip audioClip = AudioClip.Create("Test", pcmLength, channels, frequency, false);
        audioClip.SetData(pcm, 0);
        _toFill.clip = audioClip;

        _toFill.Play();

        Debug.Log($"{pcmPtr}, {pcm.Length}, {pcmLength}, {channels}, {frequency}");
    }
}
