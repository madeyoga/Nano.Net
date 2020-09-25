using System.Text;
using System.Threading.Tasks;

using Discord.Commands;

using Nano.Net.Services;


namespace Nano.Net.Modules
{
	public class SpotifyModule : ModuleBase<SocketCommandContext>
	{
		private SpotifyService _service = new SpotifyService();

		[Command("spotify")]
		[Alias("spo")]
		public async Task SearchSpotifyTrackAsync([Remainder] string query)
		{
			var result = await this._service.SearchTrack(query);
			var ctr = 1;
			var sb = new StringBuilder();

			foreach (var song in result)
			{
				sb.Append(ctr + ". ").
				   Append($"**{ song.Name }**").AppendLine().
				   Append(song.Id).
				   AppendLine();
				ctr++;
			}
			await Context.Channel.SendMessageAsync(sb.ToString());
		}

		[Command("spotify track info")]
		[Alias("spoti")]
		public async Task TrackInfo(string id)
		{
			var track = await this._service.GetTrackAsync(id);
			var info = this._service.GetTrackInfomation(track);

			await Context.Channel.SendMessageAsync(info);
		}

	}
}
