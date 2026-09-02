using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vosk;
using Whisper.net;

namespace VoskDesktop
{
    public partial class F_Utama : Form
    {
        private WaveInEvent _waveIn;
        private MemoryStream _audioStream;
        private WhisperFactory _whisperFactory;
        private WhisperProcessor _whisperProcessor;
        private bool _isProcessing = false;

        public F_Utama()
        {
            InitializeComponent();
            InitWhisper();
        }

        private void InitWhisper()
        {
            try
            {
                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "ggml-small.en.bin");

                // Fallback jika path masih di level project folder
                if (!File.Exists(modelPath))
                {
                    modelPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Lib\ggml-small.en.bin"));
                }

                if (!File.Exists(modelPath))
                {
                    MessageBox.Show("Model Whisper tidak ditemukan di: " + modelPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Inisialisasi Factory & Processor Whisper
                _whisperFactory = WhisperFactory.FromPath(modelPath);
                _whisperProcessor = _whisperFactory.CreateBuilder()
                    .WithLanguage("en") // US English
                    .Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal inisialisasi Whisper:\n" + ex.Message, "Error Whisper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void F_Utama_Load(object sender, EventArgs e)
        {
            string welcomeHeader =
        "VOSK / WHISPER.NET TRANSCRIBER\r\n" +
        "----------------------------------------\r\n" +
        "• Author: Fajar Maulana\r\n" +
        "• Stack : C# | .NET 4.7.2 | Whisper.net\r\n" +
        "• Target: US Client Meeting Assistant\r\n" +
        "----------------------------------------\r\n" +
        "Ready. Click [Start] to listen..";
            this.xpTextBox1.Text = welcomeHeader;
        }

        private void btnSetings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Maaf fungsi ini belum di buat.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_whisperProcessor == null)
            {
                MessageBox.Show("Engine Whisper belum siap.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_waveIn != null) return; // Mencegah double start

            _audioStream = new MemoryStream();

            // 1. Inisialisasi NAudio (16 kHz, 16-bit, Mono)
            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(16000, 16, 1),
                BufferMilliseconds = 100
            };

            // 2. Pasang event listener audio (kumpulan chunk 3 detik)
            _waveIn.DataAvailable += (s, a) =>
            {
                if (_audioStream != null)
                {
                    _audioStream.Write(a.Buffer, 0, a.BytesRecorded);

                    // Buffer 3 detik = 96.000 byte
                    if (_audioStream.Length >= 96000 && !_isProcessing)
                    {
                        byte[] audioChunk = _audioStream.ToArray();
                        _audioStream.SetLength(0); // Reset buffer

                        Task.Run(() => ProcessWhisperChunk(audioChunk));
                    }
                }
            };

            // 3. Mulai merekam
            _waveIn.StartRecording();
            System.Diagnostics.Debug.WriteLine("Whisper listening started...");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_waveIn != null)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
            }

            if (_audioStream != null)
            {
                _audioStream.Dispose();
                _audioStream = null;
            }

            System.Diagnostics.Debug.WriteLine("Whisper listening stopped.");
        }



        private async Task ProcessWhisperChunk(byte[] pcmData)
        {
            _isProcessing = true;
            try
            {
                byte[] wavBytes;
                using (var tempStream = new MemoryStream())
                {
                    using (var writer = new WaveFileWriter(tempStream, new WaveFormat(16000, 16, 1)))
                    {
                        writer.Write(pcmData, 0, pcmData.Length);
                        writer.Flush();
                    }
                    wavBytes = tempStream.ToArray();
                }

                using (var audioStreamForWhisper = new MemoryStream(wavBytes))
                {
                    string resultText = "";

                    await foreach (var segment in _whisperProcessor.ProcessAsync(audioStreamForWhisper))
                    {
                        resultText += segment.Text + " ";
                    }

                    resultText = resultText.Trim();

                    // Filter artefak Whisper saat hening: [BLANK_AUDIO], [inaudible], atau deretan kurung siku [ [ [
                    if (string.IsNullOrWhiteSpace(resultText) ||
                        resultText.Contains("[BLANK_AUDIO]") ||
                        resultText.Contains("[inaudible]") ||
                        resultText.Replace("[", "").Replace("]", "").Trim().Length == 0)
                    {
                        return;
                    }

                    // Tampilkan hasil bersih ke UI
                    VoskDesktop.Helper.UIHelper.addNew(this.xpPanelGroup1, DateTime.Now, resultText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Whisper Error: " + ex.Message);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private bool IsAudioSilence(byte[] pcmData, short silenceThreshold = 100)
        {
            for (int i = 0; i < pcmData.Length - 1; i += 2)
            {
                short sample = (short)(pcmData[i] | (pcmData[i + 1] << 8));
                if (Math.Abs(sample) > silenceThreshold)
                {
                    return false; // Ada suara terdeteksi
                }
            }
            return true; // Hanya hening / desis ruangan
        }

    }
}
