using Discord;
using System.Collections.Concurrent;

namespace Nano.Net.Audio
{
    public class GuildVoiceStateManager
    {
        private readonly ConcurrentDictionary<ulong, GuildVoiceState> voiceStates;

        public GuildVoiceStateManager()
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
    }
}
