using System.IO;
using UnityEngine;

public class PluginTest : MonoBehaviour
{
    [System.Serializable]
    private struct Data
    {
        public float[] PCMData;
    }

    [SerializeField] private AudioSource _audioSource;

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
        VorbisPlugin.EncodePcmDataToFile(pcm, pcm.Length, 1, 44100, 0.4f, pathToSave + ".ogg");

        Debug.Log(pathToSave);
    }
}
