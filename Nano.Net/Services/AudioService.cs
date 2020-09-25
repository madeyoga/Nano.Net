using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Nano.Net.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();
        public bool PauseState = false;

        public async Task JoinChannel(IVoiceChannel channel, ulong guildID)
        {
            var audioClient = await channel.ConnectAsync();
            _audioClients.TryAdd(guildID, audioClient);
        }

        public async Task LeaveChannel(SocketCommandContext Context)
        {
            if (_audioClients.TryGetValue(Context.Guild.Id, out IAudioClient audioClient))
            {
                try
                {
                    await audioClient.StopAsync();
                    _audioClients.TryRemove(Context.Guild.Id, out audioClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + ", cannot disconnect");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("I'm not connected");
            }
        }

        public Process CreateStream(string youtubeVideoUrl, SocketGuild guild)
        {
            // Stream song
            Process ytdlProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl.exe",
                Arguments = $"--format bestaudio -o Data/Music/{guild.Id}.mp3 {youtubeVideoUrl}",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            ytdlProcess.WaitForExit();

            Process ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-i Data/Music/{guild.Id}.mp3 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });

            return ffmpegProcess;
        }

        public async Task SendAudioAsync(SocketGuild guild, string url)
        {
            if (_audioClients.TryGetValue(guild.Id, out IAudioClient client))
            {
                using (Process ffmpegProcess = CreateStream(url, guild))
                using (Stream ffmpegOutput = ffmpegProcess.StandardOutput.BaseStream)
                using (AudioOutStream clientStream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try
                    {
                        await ffmpegOutput.CopyToAsync(clientStream);
                        //var buffer = new byte[3816];
                        //var br = await ffmpegOutput.ReadAsync(buffer, 0, buffer.Length);

                        //while ((br = await ffmpegOutput.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        //{
                        //    while (PauseState == true)
                        //    {
                        //        await Task.Delay(2000);
                        //    }

                        //    await clientStream.WriteAsync(buffer);
                        //}
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Stopped audio stream");
                        //File.Delete($"Data/Music/{guild.Id}.mp3");
                    }
                    finally
                    {
                        await clientStream.FlushAsync();
                        //Console.WriteLine($"Deleted Data/Music/{guild.Id}.mp3");
                        //File.Delete($"Data/Music/{guild.Id}.mp3");
                    }
                }
            }
        }
    }
}
