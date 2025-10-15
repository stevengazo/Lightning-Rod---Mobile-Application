using Manchito.DataBaseContext;
using Manchito.Model;
using Plugin.AudioRecorder;
using Plugin.Maui.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Services
{
    public class AudioService
    {
        private readonly AudioRecorderService _recorderService = new()
        {
            StopRecordingAfterTimeout = false,
            StopRecordingOnSilence = false
        };
        private readonly string _basePath;

        public AudioService(string basePath)
        {
            _basePath = basePath;
        }

        public async Task<string> StartRecordingAsync()
        {
            await _recorderService.StartRecording();
            return "Recording started";
        }

        public async Task<string> StopRecordingAsync(string categoryPath)
        {
            await _recorderService.StopRecording();
            string file = _recorderService.GetAudioFilePath();

            if (string.IsNullOrEmpty(file) || _recorderService.TotalAudioTimeout.Seconds <= 1)
                return null;

            string filename = $"Audio-{DateTime.Now:HH_mm_ss}.wav";
            string pathAudio = Path.Combine(categoryPath, filename);

            Directory.CreateDirectory(Path.GetDirectoryName(pathAudio)!);
            File.Copy(file, pathAudio, true);

            return pathAudio;
        }

        public async Task SaveAudioToDatabaseAsync(string filePath, int categoryId)
        {
            await using var db = new DBLocalContext();
            int nextId = db.AudioNote.OrderByDescending(a => a.AudioNoteId)
                                     .Select(a => a.AudioNoteId)
                                     .FirstOrDefault() + 1;

            var audioNote = new AudioNote
            {
                AudioNoteId = nextId,
                PathFile = filePath,
                CategoryId = categoryId
            };

            db.AudioNote.Add(audioNote);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAudioAsync(int id)
        {
            await using var db = new DBLocalContext();
            var audio = db.AudioNote.FirstOrDefault(a => a.AudioNoteId == id);
            if (audio != null)
            {
                db.AudioNote.Remove(audio);
                await db.SaveChangesAsync();
                if (File.Exists(audio.PathFile))
                    File.Delete(audio.PathFile);
            }
        }

        public async Task<List<AudioNote>> GetAudiosAsync(int categoryId)
        {
            await using var db = new DBLocalContext();
            return db.AudioNote.Where(a => a.CategoryId == categoryId).ToList();
        }
    }
}
