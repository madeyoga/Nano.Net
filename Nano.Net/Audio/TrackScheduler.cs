﻿using Discord.Addons.Music.Player;
using Discord.Addons.Music.Source;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Nano.Net.Audio
{
    public class TrackScheduler
    {
        public ConcurrentQueue<AudioTrack> SongQueue { get; }

        private AudioPlayer player;

        public TrackScheduler(AudioPlayer player)
        {
            SongQueue = new ConcurrentQueue<AudioTrack>();
            this.player = player;
            this.player.OnTrackStartAsync += OnTrackStartAsync;
            this.player.OnTrackEndAsync += OnTrackEndAsync;
        }

        public Task Enqueue(AudioTrack track)
        {
            if (player.PlayingTrack != null)
            {
                SongQueue.Enqueue(track);
            }
            else
            {
                // fire and forget
                player.StartTrackAsync(track).ConfigureAwait(false);
            }
            return Task.CompletedTask;
        }

        public async Task NextTrack()
        {
            AudioTrack nextTrack;
            if (SongQueue.TryDequeue(out nextTrack))
                await player.StartTrackAsync(nextTrack);
            else
                player.Stop();
        }

        private Task OnTrackStartAsync(IAudioClient audioClient, IAudioSource track)
        {
            Console.WriteLine("Track start! " + track.Info.Title);
            return Task.CompletedTask;
        }

        private async Task OnTrackEndAsync(IAudioClient audioClient, IAudioSource track)
        {
            Console.WriteLine("Track end! " + track.Info.Title);

            await NextTrack();
        }
    }
}
