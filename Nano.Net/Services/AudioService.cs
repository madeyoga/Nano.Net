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
                RedirectStandardOutput = true
            });

            Process ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-i Data/Music/{guild.Id}.mp3 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

            return ffmpegProcess;
        }

        public async Task SendAudioAsync(SocketGuild guild, string url)
        {
            if (_audioClients.TryGetValue(guild.Id, out IAudioClient client))
            {
                using (Stream output = CreateStream(url, guild).StandardOutput.BaseStream)
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try
                    {
                        await output.CopyToAsync(stream);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Stopped audio stream");
                    }
                    finally
                    {
                        await stream.FlushAsync();
                    }
                }
            }
        }

    }

}
