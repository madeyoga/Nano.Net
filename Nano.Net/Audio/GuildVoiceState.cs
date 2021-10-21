using Discord.Addons.Music.Player;

namespace Nano.Net.Audio
{
    public class GuildVoiceState
    {
        public AudioPlayer Player { get; }
        public TrackScheduler Scheduler { get; }

        public GuildVoiceState()
        {
            Player = new AudioPlayer();
            Scheduler = new TrackScheduler(Player);
        }
    }
}
