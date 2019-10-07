using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    [System.Serializable]
    private struct Data
    {
        public float[] PCMData;
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _toFill;

    public void SaveAudioClipToOgg()
    {
        float[] pcm = new float[_audioSource.clip.samples];
        _audioSource.clip.GetData(pcm, 0);

        string dirToSave = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirToSave);
        string pathToSave = Path.Combine(dirToSave, "1");

        Data data = new Data() { PCMData = pcm };

        File.WriteAllText(pathToSave + ".json", JsonUtility.ToJson(data));
        Debug.Log(pcm.Length);

        Presentation.Utility.Loader.WaveAudio.Save(pathToSave + ".wav", _audioSource.clip);
        VorbisPlugin.EncodePcmDataToFile(pathToSave + ".ogg", pcm, pcm.Length, 1, 44100, 0.4f);

        Debug.Log(pathToSave);
    }

    public void LoadAudioClipFromOgg()
    {
        string dirFromLoad = Path.Combine(Application.persistentDataPath, "Ogg");
        Directory.CreateDirectory(dirFromLoad);
        string pathFromLoad = Path.Combine(dirFromLoad, "1");

        VorbisPlugin.DecodePcmDataFromFile(pathFromLoad + ".ogg", out System.IntPtr pcmPtr, out int pcmLength, out short channels, out int frequency);

        float[] pcm = new float[pcmLength];
        Marshal.Copy(pcmPtr, pcm, 0, pcmLength);
        VorbisPlugin.FreeSamplesArrayNativeMemory(ref pcmPtr);

        AudioClip audioClip = AudioClip.Create("Test", pcmLength, channels, frequency, false);
        audioClip.SetData(pcm, 0);
        _toFill.clip = audioClip;

        _toFill.Play();

        Debug.Log($"{pcmPtr}, {pcm.Length}, {pcmLength}, {channels}, {frequency}");
    }
}
