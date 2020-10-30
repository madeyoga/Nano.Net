﻿using Discord.Addons.Music.Core;
using Discord.Addons.Music.Exception;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nano.Net.Services
{
    public class TrackScheduler : AudioEventAdapter
    {
        public void OnTrackEnd(AudioTrack track)
        {
            Console.WriteLine("Track end");
        }

        public void OnTrackError(AudioTrack track, TrackErrorException exception)
        {
            throw new NotImplementedException();
        }

        public void OnTrackStart(AudioTrack track)
        {
            Console.WriteLine("Start playing track");
        }

        public Task OnTrackStartAsync(AudioTrack track)
        {
            Console.WriteLine("Track start!");
            
            return Task.CompletedTask;
        }

        public void OnTrackStuck(AudioTrack track, TrackStuckException exception)
        {
            throw new NotImplementedException();
        }
    }
}
