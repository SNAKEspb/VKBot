using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace VKBot.VkontakteBot.Services
{
    public class Util
    {
        
        public static byte[] ConvertAudio(byte[] sourceBytes)
        {
            using (System.IO.Stream sourceStream = new System.IO.MemoryStream(sourceBytes))
            {
                using (System.IO.Stream destinationStream = new System.IO.MemoryStream())
                {
                    ConvertMp3ToWav(sourceStream, destinationStream);
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        destinationStream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        
        public static void ConvertMp3ToWav(System.IO.Stream sourceStream, System.IO.Stream destinationStream)
        {
            using (var mp3 = new NAudio.Wave.Mp3FileReader(sourceStream))
            {
                using (NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    NAudio.Wave.WaveFileWriter.WriteWavFileToStream(destinationStream, pcm);
                    // NAudio.Wave.WaveFileWriter.CreateWaveFile("test.wav", pcm);
                }
            }
        }

        private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            using (NAudio.Wave.Mp3FileReader mp3 = new NAudio.Wave.Mp3FileReader(_inPath_))
            {
                using (NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    NAudio.Wave.WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                }
            }
        }
    }
}
