using NUnit.Framework;
using System.IO;
using Unity.PerformanceTesting;
using UnityEngine;

namespace PlayModeTests
{
    public sealed class PerformanceTests
    {
        [Test, Performance]
        public void TestSavePerformance()
        {
            string filesFolder = Path.Combine(Application.persistentDataPath, Consts.TEST_FILE_DIR_NAME);
            AudioClip sourceAudioClip = Resources.Load<AudioClip>(Consts.SOURCE_AUDIO_CLIP_RESOURCES_PATH_STEREO_48000HZ);
            string pathToFile = Path.Combine(filesFolder, sourceAudioClip.name) + ".ogg";


            Measure.Method(() => { OggVorbis.VorbisPlugin.Save(pathToFile, sourceAudioClip); })
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(5)
                .GC()
                .Run();
        }

    }
}
