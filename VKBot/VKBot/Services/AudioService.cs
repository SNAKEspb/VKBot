using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace VKBot.Services
{
    public class AudioService
    {
        [StructLayout(LayoutKind.Sequential)]
        // Структура, описывающая заголовок WAV файла.
        internal class WavHeader
        {
            // WAV-формат начинается с RIFF-заголовка:

            // Содержит символы "RIFF" в ASCII кодировке
            // (0x52494646 в big-endian представлении)
            public UInt32 ChunkId;

            // 36 + subchunk2Size, или более точно:
            // 4 + (8 + subchunk1Size) + (8 + subchunk2Size)
            // Это оставшийся размер цепочки, начиная с этой позиции.
            // Иначе говоря, это размер файла - 8, то есть,
            // исключены поля chunkId и chunkSize.
            public UInt32 ChunkSize;

            // Содержит символы "WAVE"
            // (0x57415645 в big-endian представлении)
            public UInt32 Format;

            // Формат "WAVE" состоит из двух подцепочек: "fmt " и "data":
            // Подцепочка "fmt " описывает формат звуковых данных:

            // Содержит символы "fmt "
            // (0x666d7420 в big-endian представлении)
            public UInt32 Subchunk1Id;

            // 16 для формата PCM.
            // Это оставшийся размер подцепочки, начиная с этой позиции.
            public UInt32 Subchunk1Size;

            // Аудио формат, полный список можно получить здесь http://audiocoding.ru/wav_formats.txt
            // Для PCM = 1 (то есть, Линейное квантование).
            // Значения, отличающиеся от 1, обозначают некоторый формат сжатия.
            public UInt16 AudioFormat;

            // Количество каналов. Моно = 1, Стерео = 2 и т.д.
            public UInt16 NumChannels;

            // Частота дискретизации. 8000 Гц, 44100 Гц и т.д.
            public UInt32 SampleRate;

            // sampleRate * numChannels * bitsPerSample/8
            public UInt32 ByteRate;

            // numChannels * bitsPerSample/8
            // Количество байт для одного сэмпла, включая все каналы.
            public UInt16 BlockAlign;

            // Так называемая "глубиная" или точность звучания. 8 бит, 16 бит и т.д.
            public UInt16 BitsPerSample;

            // Подцепочка "data" содержит аудио-данные и их размер.

            // Содержит символы "data"
            // (0x64617461 в big-endian представлении)
            public UInt32 Subchunk2Id;

            // numSamples * numChannels * bitsPerSample/8
            // Количество байт в области данных.
            public UInt32 Subchunk2Size;

            // Далее следуют непосредственно Wav данные.
        }

        static WavHeader wavHeader(string file)
        {
            var header = new WavHeader();
            // Размер заголовка
            var headerSize = Marshal.SizeOf(header);

            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var buffer = new byte[headerSize];
            fileStream.Read(buffer, 0, headerSize);

            // Чтобы не считывать каждое значение заголовка по отдельности,
            // воспользуемся выделением unmanaged блока памяти
            var headerPtr = Marshal.AllocHGlobal(headerSize);
            // Копируем считанные байты из файла в выделенный блок памяти
            Marshal.Copy(buffer, 0, headerPtr, headerSize);
            // Преобразовываем указатель на блок памяти к нашей структуре
            Marshal.PtrToStructure(headerPtr, header);

            // Выводим полученные данные
            Console.WriteLine("Sample rate: {0}", header.SampleRate);
            Console.WriteLine("Channels: {0}", header.NumChannels);
            Console.WriteLine("Bits per sample: {0}", header.BitsPerSample);

            // Посчитаем длительность воспроизведения в секундах
            var durationSeconds = 1.0 * header.Subchunk2Size / (header.BitsPerSample / 8.0) / header.NumChannels / header.SampleRate;
            var durationMinutes = (int)Math.Floor(durationSeconds / 60);
            durationSeconds = durationSeconds - (durationMinutes * 60);
            Console.WriteLine("Duration: {0:00}:{1:00}", durationMinutes, durationSeconds);

            //Console.ReadKey();

            // Освобождаем выделенный блок памяти
            Marshal.FreeHGlobal(headerPtr);
            return header;
        }

        public static byte[] decodeMP3ToWavMono(byte[] sourceBytes)
        {
            using (System.IO.Stream sourceStream = new System.IO.MemoryStream(sourceBytes))
            {
                using (var destinationStream = new System.IO.MemoryStream())
                {
                    using (var tempStream = new System.IO.MemoryStream())
                    {
                        using (MP3Sharp.MP3Stream stream = new MP3Sharp.MP3Stream(sourceStream, 4096, MP3Sharp.SoundFormat.Pcm16BitMono))
                        {
                            stream.CopyTo(tempStream);
                            WriteWavHeader(destinationStream, false, (ushort)1, 16, stream.Frequency, tempStream.Length);
                            tempStream.Seek(0, SeekOrigin.Begin);
                            tempStream.CopyTo(destinationStream);
                            return destinationStream.ToArray();
                        }
                    }
                }
            }
        }

        private static void WriteWavHeader(Stream stream, bool isFloatingPoint, ushort channelCount, ushort bitDepth, int sampleRate, long totalSampleCount)
        {
            stream.Position = 0;
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);//chunkId - Содержит символы "RIFF" в ASCII кодировке
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);//chunkSize - Это оставшийся размер цепочки, начиная с этой позиции. Иначе говоря, это размер файла - 8
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);//format - Содержит символы "WAVE" 
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);//subchunk1Id - Содержит символы "fmt"
            stream.Write(BitConverter.GetBytes(16), 0, 4);//subchunk1Size - длинна заголовка fmt - 16 для формата PCM(или 18 для экстра данных)
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);//audioFormat - Для PCM = 1.Значения, отличающиеся от 1, обозначают некоторый формат сжатия.
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);//numChannels - Количество каналов. Моно = 1, Стерео = 2 и т.д.
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);//sampleRate - Частота дискретизации. 8000 Гц, 44100 Гц и т.д.
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);//byteRate - Количество байт, переданных за секунду воспроизведения.
            stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);//blockAlign - Количество байт для одного сэмпла, включая все каналы.
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);//bitsPerSample - Количество бит в сэмпле. Так называемая "глубина" или точность звучания. 8 бит, 16 бит и т.д.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);//subchunk2Id - Содержит символы "data"
            stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);//subchunk2Size - Количество байт в области данных.
            //data
            //data is here =)
        }

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
                }
            }
        }

        public static void ConvertMp3ToWav(string _inPath_, string _outPath_)
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
