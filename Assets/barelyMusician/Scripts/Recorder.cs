// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using System;
using System.IO;
using UnityEngine;

namespace BarelyAPI
{
    [AddComponentMenu("BarelyAPI/Recorder")]
    public class Recorder : MonoBehaviour
    {
        public string fileName;

        FileStream fileStream;
        string folderPath;

        bool recording;
        public bool IsRecording
        {
            get { return recording; }
        }

        const int HEADER_SIZE = 44;
        const int RESCALE_FACTOR = 32767;

        void Start()
        {
            folderPath = Application.dataPath + "/Records/";

            recording = false;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (recording)
            {
                ConvertAndWrite(ref data);
            }
        }

        public void StartRecord()
        {
            if (!recording)
            {
                StartWriting();

                recording = true;

                Debug.Log("Recording started..");
            }
        }

        public void StopRecord()
        {
            if (recording)
            {
                recording = false;

                WriteHeader();

                Debug.Log("Recording stopped. File saved to: " + Path.Combine(folderPath, fileName));

                //#if UNITY_EDITOR
                //UnityEditor.FileUtil.MoveFileOrDirectory(Path.Combine(folderPath, fileName), UnityEditor.EditorUtility.SaveFilePanel("Save file to:", Application.dataPath, "NewRecord", "wav"));
                //#endif
            }
        }

        void StartWriting()
        {
            // Create the folder beforehand if not exists
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);
            
            if (!fileName.ToLower().EndsWith(".wav"))
            {
                fileName += ".wav";
            }
            
            fileStream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create);
            byte emptyByte = new byte();

            for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
            {
                fileStream.WriteByte(emptyByte);
            }
        }
        
        void ConvertAndWrite(ref float[] samples)
        {
            Int16[] intData = new Int16[samples.Length];
            //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

            Byte[] bytesData = new Byte[samples.Length * 2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(Mathf.Clamp(samples[i], -1.0f, 1.0f) * RESCALE_FACTOR);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        void WriteHeader()
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);

            Byte[] channels = BitConverter.GetBytes(two);
            fileStream.Write(channels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(AudioProperties.SAMPLE_RATE);
            fileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(AudioProperties.SAMPLE_RATE * two * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            fileStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(two * 2);
            fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(datastring, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - HEADER_SIZE);
            fileStream.Write(subChunk2, 0, 4);

            fileStream.Close();
        }
    }
}