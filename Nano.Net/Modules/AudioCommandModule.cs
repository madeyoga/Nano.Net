using Discord.Addons.Music.Source;
using Discord.Commands;
using Discord.WebSocket;
using Nano.Net.Audio;
using Nano.Net.Services;
using System.Threading.Tasks;

namespace Nano.Net.Modules
{
    public class AudioCommandModule : ModuleBase<SocketCommandContext>
    {
        private AudioService audioService;

        public AudioCommandModule(AudioService audioService)
        {
            this.audioService = audioService;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string query)
        {
            // Ensure voice
            SocketVoiceChannel voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;

            if (voiceChannel is null)
            {
                await ReplyAsync("You r not in a voice channel");
                return;
            }

            SocketVoiceChannel selfVoiceChannel = (Context.Guild.CurrentUser as SocketGuildUser)?.VoiceChannel;
            if (selfVoiceChannel is null)
            {
                await audioService.JoinChannel(voiceChannel, Context.Guild);
            }

            AudioTrack loadedTrack = await audioService.LoadAndPlay(query, Context.Guild);
            if (loadedTrack != null)
            {
                await ReplyAsync($":musical_note: Added to queue {loadedTrack.Info.Title}");
            }
            else
            {
                await ReplyAsync($"Not found anything with {query}");
            }
        }

        [Command("join", RunMode = RunMode.Async)]
        [Alias("summon")]
        public async Task JoinAsync()
        {
            SocketVoiceChannel voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;

            if (voiceChannel is null)
            {
                await ReplyAsync("You r not in a voice channel");
                return;
            }

            await audioService.JoinChannel(voiceChannel, Context.Guild);
        }

        [Command("stop", RunMode = RunMode.Async)]
        [Alias("leave")]
        public async Task StopAsync()
        {
            GuildVoiceState voiceState = audioService.GetVoiceState(Context.Guild);
            voiceState.Player.Stop();
            await audioService.LeaveChannel(Context);
        }

        [Command("pause", RunMode = RunMode.Async)]
        public async Task PauseAsync()
        {
            GuildVoiceState voiceState = audioService.GetVoiceState(Context.Guild);
            if (voiceState.Player.Paused)
            {
                voiceState.Player.Paused = false;
                await ReplyAsync("Resume!");
            }
            else
            {
                voiceState.Player.Paused = true;
                await ReplyAsync("Pause!");
            }
        }

        [Command("volume", RunMode = RunMode.Async)]
        public async Task VolumeAsync([Remainder] string volumeNumber)
        {
            GuildVoiceState voiceState = audioService.GetVoiceState(Context.Guild);
            voiceState.Player.Volume = double.Parse(volumeNumber) / 100;
            await ReplyAsync("Volume changed to " + volumeNumber + "!");
        }

        [Command("np", RunMode = RunMode.Async)]
        public async Task AnnounceNowplayAsync()
        {
            GuildVoiceState voiceState = audioService.GetVoiceState(Context.Guild);
            IAudioSource np = voiceState.Player.PlayingTrack;

            if (np == null)
            {
                await ReplyAsync("Not playing anything.");
            }
            else
            {
                await ReplyAsync(":musical_note: Now playing: " + voiceState.Player.PlayingTrack.Info.Title);
            }
        }
    }
}
