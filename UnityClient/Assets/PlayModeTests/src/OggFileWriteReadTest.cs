using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

public class OggFileWriteReadTest
{
    private AudioClip _stereoFile3Minutes48000Hz;
    private AudioClip _monoFile3Minutes48000Hz;
    private AudioClip _stereoFile2Minutes44100Hz;
    private AudioClip _monoFile2Minutes44100Hz;

    private string _filesFolder;

    [SetUp]
    public void SetUp()
    {
        _stereoFile3Minutes48000Hz = Resources.Load<AudioClip>("Audio/vivaldi-summer-the-four-seasons");
        _monoFile3Minutes48000Hz = Resources.Load<AudioClip>("Audio/vivaldi-summer-the-four-seasons_mono");
        _stereoFile2Minutes44100Hz = Resources.Load<AudioClip>("Audio/Kai_Engel_-_07_-_Interception");
        _monoFile2Minutes44100Hz = Resources.Load<AudioClip>("Audio/Kai_Engel_-_07_-_Interception_mono");

        _filesFolder = Path.Combine(Application.persistentDataPath, "VorbisTestFiles");
        Directory.CreateDirectory(_filesFolder);
    }

    [Test]
    public void TheAudioClipsAreLoaded()
    {
        Assert.IsNotNull(_stereoFile3Minutes48000Hz);
        Assert.IsNotNull(_monoFile3Minutes48000Hz);
        Assert.IsNotNull(_stereoFile2Minutes44100Hz);
        Assert.IsNotNull(_monoFile2Minutes44100Hz);
    }
    [Test]
    public void TheAudioClipsHaveTheRightChannelCount()
    {
        Assert.AreEqual(2, _stereoFile3Minutes48000Hz.channels);
        Assert.AreEqual(1, _monoFile3Minutes48000Hz.channels);
        Assert.AreEqual(2, _stereoFile2Minutes44100Hz.channels);
        Assert.AreEqual(1, _monoFile2Minutes44100Hz.channels);
    }
    [Test]
    public void TheAudioClipsHaveTheRightFrequencySampleRate()
    {
        Assert.AreEqual(48000, _stereoFile3Minutes48000Hz.frequency);
        Assert.AreEqual(48000, _monoFile3Minutes48000Hz.frequency);
        Assert.AreEqual(44100, _stereoFile2Minutes44100Hz.frequency);
        Assert.AreEqual(44100, _monoFile2Minutes44100Hz.frequency);
    }
    [Test]
    public void TheVorbisPluginSavesStereo48000HzFile()
    {
        string pathToFile = Path.Combine(_filesFolder, _stereoFile3Minutes48000Hz.name) + ".ogg";
        OggVorbis.VorbisPlugin.Save(pathToFile, _stereoFile3Minutes48000Hz);
        Assert.IsTrue(File.Exists(pathToFile));
        var fileInfo = new FileInfo(pathToFile);
        Assert.IsTrue(fileInfo.Length > 1000);
    }
    [Test]
    public void TheVorbisPluginSavesAndLoadsStereo48000HzFile()
    {
        string pathToFile = Path.Combine(_filesFolder, _stereoFile3Minutes48000Hz.name) + ".ogg";
        OggVorbis.VorbisPlugin.Save(pathToFile, _stereoFile3Minutes48000Hz);
        AudioClip audioClip = OggVorbis.VorbisPlugin.Load(pathToFile);
        OggVorbis.VorbisPlugin.Save(Path.Combine(_filesFolder, _stereoFile3Minutes48000Hz.name) + "_1.ogg", audioClip);

        Assert.AreEqual(_stereoFile3Minutes48000Hz.channels, audioClip.channels);
        Assert.AreEqual(_stereoFile3Minutes48000Hz.frequency, audioClip.frequency);

        // Tested on Win 10 Unity 2021.1.2f1 version:
        // 128 delta is added because Unity _stereoFile3Minutes48000Hz.samples returns
        // an array that is 128 samples smaller in size that the original file contains
        // The _stereoFile3Minutes48000Hz source file in Unity project is an mp3 file
        // The Audacity returns:
        //      9 362 304 samples for the original mp3 file from Unity Assets folder
        //      9 362 432 samples for the first stored ogg file
        //      9 362 432 samples for the ogg file that is stored from the loaded first ogg file
        Assert.AreEqual(_stereoFile3Minutes48000Hz.samples, audioClip.samples, 128);
        // And the length of the files differs little bit
        Assert.AreEqual(_stereoFile3Minutes48000Hz.length, audioClip.length, 0.1);

        float[] samplesFromResourcesFile = new float[_stereoFile3Minutes48000Hz.samples];
        Assert.IsTrue(_stereoFile3Minutes48000Hz.GetData(samplesFromResourcesFile, 0));
        float[] samplesFromLoadedOggFile = new float[audioClip.samples];
        Assert.IsTrue(audioClip.GetData(samplesFromLoadedOggFile, 0));

        int length = Mathf.Min(_stereoFile3Minutes48000Hz.samples, audioClip.samples);
        for (int i = 0; i < length; ++i)
        {
            Assert.AreEqual(samplesFromResourcesFile[i], samplesFromLoadedOggFile[i], 0.2, "Wrong sample value at " + i);
        }
    }

    [UnityTest]
    public IEnumerator OggWritingToFileTestWithEnumeratorPasses()
    {
        yield return null;
    }
}
