namespace OggVorbis
{
    public class NativeErrorException : System.Exception
    {
        public NativeErrorCode NativeErrorCode { get; }

        private NativeErrorException(NativeErrorCode nativeErrorCode)
        {
            NativeErrorCode = nativeErrorCode;
        }

        internal static void ThrowExceptionIfNecessary(int returnValue)
        {
            if (returnValue == 0)
            {
                return;
            }
            throw new NativeErrorException((NativeErrorCode)returnValue);
        }
    }
}
