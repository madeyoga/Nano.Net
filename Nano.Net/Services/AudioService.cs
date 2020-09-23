using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Nano.Net.Services
{
    public class AudioService
    {
        public Process CreateStream(string youtubeVideoUrl)
        {
            // Stream youtube-dl output to ffmpeg input.
            string youtubeDlCommand = $"youtube-dl.exe -o - {youtubeVideoUrl} --format bestaudio --quiet --no-warnings";
            string ffmpegCommand = "ffmpeg -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -i pipe:0 -f asf pipe:1 -vn -nostats -loglevel 0";

            var process = Process.Start("cmd.exe",
                                        $"{youtubeDlCommand} | {ffmpegCommand}");
            return process;
        }

        public async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }

    }

}
