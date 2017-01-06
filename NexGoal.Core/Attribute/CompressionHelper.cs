namespace NexGoal.Core.Attribute
{
    using System.IO;

    public static class CompressionHelper
    {
        public static byte[] DeflateByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            MemoryStream output = null;
            try
            {
                output = new MemoryStream();
                using (var compressor = new Ionic.Zlib.DeflateStream(output, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed))
                {
                    output = null;
                    compressor.Write(str, 0, str.Length);
                }

                return output.ToArray();
            }
            finally
            {
                if (output != null)
                {
                    output.Dispose();
                }
            }
        }
    }
}