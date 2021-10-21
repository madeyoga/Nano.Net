using Discord;
using Discord.Addons.Music.Common;
using Discord.Addons.Music.Source;
using Discord.Commands;
using Nano.Net.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nano.Net.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, GuildVoiceState> voiceStates;

        public AudioService()
        {
            voiceStates = new ConcurrentDictionary<ulong, GuildVoiceState>();
        }

        public GuildVoiceState GetVoiceState(IGuild guild)
        {
            GuildVoiceState voiceState;

            if (!voiceStates.ContainsKey(guild.Id))
            {
                voiceStates.TryAdd(guild.Id, new GuildVoiceState());
            }
            voiceState = voiceStates[guild.Id];

            return voiceState;
        }

        public async Task JoinChannel(IVoiceChannel channel, IGuild guild)
        {
            var audioClient = await channel.ConnectAsync();

            GuildVoiceState voiceState = GetVoiceState(guild);
            voiceState.Player.SetAudioClient(audioClient);
        }

        public async Task LeaveChannel(SocketCommandContext Context)
        {
            if (voiceStates.TryGetValue(Context.Guild.Id, out GuildVoiceState voiceState))
            {
                try
                {
                    await voiceState.Player.AudioClient.StopAsync();
                    voiceStates.TryRemove(Context.Guild.Id, out voiceState);
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

        public async Task<AudioTrack> LoadAndPlay(string query, IGuild guild)
        {
            List<AudioTrack> tracks;

            // If query is Url
            bool wellFormedUri = Uri.IsWellFormedUriString(query, UriKind.Absolute);
            tracks = await TrackLoader.LoadAudioTrack(query, fromUrl: wellFormedUri).ConfigureAwait(false);

            if (tracks.Count == 0)
            {
                return null;
            }

            Console.WriteLine("Loaded " + tracks.Count + " entri(es)");

            GuildVoiceState voiceState = GetVoiceState(guild);

            foreach (AudioTrack track in tracks)
            {
                Console.WriteLine("Enqueue " + track.Info.Title);
                await voiceState.Scheduler.Enqueue(track);
                return track;
            }

            return null;
        }
    }
}
