using System.IO;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _toFill;

    private System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();

    public void SaveAudioClipToOgg()
    {
        string dirToSave = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirToSave);
        string pathToSave = Path.Combine(dirToSave, "1");

        Debug.Log($"Audio file length: {_audioSource.clip.length} sec.");

        SaveToWav(pathToSave);
        SaveToOgg(pathToSave);

        _stopwatch.Stop();
        Debug.Log(pathToSave);
    }

    private void SaveToWav(string pathToSave)
    {
        _stopwatch.Restart();
        Presentation.Utility.Loader.WaveAudio.Save(pathToSave + ".wav", _audioSource.clip);
        Debug.Log($"Wave file save took {_stopwatch.ElapsedMilliseconds} ms.");
    }
    private void SaveToOgg(string pathToSave)
    {
        _stopwatch.Restart();
        VorbisPlugin.Save(pathToSave, _audioSource.clip);
        Debug.Log($"Vorbis ogg file save took {_stopwatch.ElapsedMilliseconds} ms.");
    }

    public void LoadAudioClipFromOgg()
    {
        string dirFromLoad = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirFromLoad);
        string pathFromLoad = Path.Combine(dirFromLoad, "1");

        LoadWav(pathFromLoad);
        LoadOgg(pathFromLoad);

        _toFill.Play();
    }
    private void LoadWav(string pathFromLoad)
    {
        _stopwatch.Restart();
        byte[] wavBytes = File.ReadAllBytes(pathFromLoad + ".wav");
        Presentation.Utility.Loader.WaveAudio.ToAudioClip(wavBytes);
        Debug.Log($"Load wave file took {_stopwatch.ElapsedMilliseconds} ms.");
    }

    private void LoadOgg(string pathFromLoad)
    {
        _stopwatch.Restart();
        _toFill.clip = VorbisPlugin.Load(pathFromLoad + ".ogg");
        Debug.Log($"Load vorbis ogg file took {_stopwatch.ElapsedMilliseconds} ms.");
    }
}
