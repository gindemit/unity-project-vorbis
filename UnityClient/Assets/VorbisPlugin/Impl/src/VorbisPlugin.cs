using System.Runtime.InteropServices;

namespace OggVorbis
{
    public static class VorbisPlugin
    {
        public static void Save(
            string filePath,
            UnityEngine.AudioClip audioClip,
            float quality = 0.4f,
            int samplesToRead = 1024)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new System.ArgumentException("The file path is null or white space");
            }
            if (audioClip == null)
            {
                throw new System.ArgumentNullException(nameof(audioClip));
            }
            if (samplesToRead <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(samplesToRead));
            }
            short finalChannelsCount = (short)audioClip.channels;
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
            int returnCode = NativeBridge.WriteAllPcmDataToFile(filePath, pcm, pcm.Length, finalChannelsCount, audioClip.frequency, quality, samplesToRead);
            NativeErrorException.ThrowExceptionIfNecessary(returnCode);
        }

        public static UnityEngine.AudioClip Load(string filePath, int maxSamplesToRead = 1024)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new System.ArgumentException("The file path is null or white space");
            }
            if (maxSamplesToRead <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(maxSamplesToRead));
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException();
            }
            int returnCode = NativeBridge.ReadAllPcmDataFromFile(
                filePath,
                out System.IntPtr pcmPtr,
                out int pcmLength,
                out short channels,
                out int frequency,
                maxSamplesToRead);
            NativeErrorException.ThrowExceptionIfNecessary(returnCode);
            float[] pcm = new float[pcmLength];
            Marshal.Copy(pcmPtr, pcm, 0, pcmLength);
            NativeBridge.FreeSamplesArrayNativeMemory(ref pcmPtr);
            NativeErrorException.ThrowExceptionIfNecessary(returnCode);

            var audioClip = UnityEngine.AudioClip.Create(System.IO.Path.GetFileName(filePath), pcmLength / channels, channels, frequency, false);
            audioClip.SetData(pcm, 0);

            return audioClip;
        }
    }
}