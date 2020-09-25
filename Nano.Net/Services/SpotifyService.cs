using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Nano.Net.Services
{
	public class SpotifyService
	{
		#region Member

		private string _clientId;
		private string _clientSecret;
		private SpotifyWebAPI _player; // music player

		#endregion Member

		#region Constructor & Destructor

		public SpotifyService()
		{
			var text = System.IO.File.ReadAllText("spotify.txt").Split(";");
			this._clientId = text[0];
			this._clientSecret = text[1];
			this._player = null;

			var res = this.AuthAsync();

			if (res.Result == false)
			{
				Console.WriteLine("Error Spotify auth");
			}
		}

		~SpotifyService()
		{
			this._clientId = this._clientSecret = string.Empty;
			this._player.Dispose();
		}

		#endregion Constructor & Destructor

		#region Public Method

		/// <summary>
		/// Do auth for access API.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AuthAsync()
		{
			try
			{
				CredentialsAuth auth = new CredentialsAuth(this._clientId, this._clientSecret);
				Token token = await auth.GetToken();
				SpotifyWebAPI api = new SpotifyWebAPI()
				{
					TokenType = token.TokenType,
					AccessToken = token.AccessToken
				};
				this._player = api;
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine("Error : " + e.Message);
				return false;
			}
		}

		/// <summary>
		/// search song title.
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public async Task<List<FullTrack>> SearchTrack(string title)
		{
			SearchItem item = this._player.SearchItems(title, SearchType.Track);
			return item.Tracks.Items;
		}

		/// <summary>
		/// get track object based on track id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<FullTrack> GetTrackAsync(string id)
		{
			return await this._player.GetTrackAsync(id);
		}

		/// <summary>
		/// get track name, id, preview url
		/// </summary>
		/// <param name="track"></param>
		/// <returns></returns>
		public string GetTrackInfomation(FullTrack track)
		{
			var sb = new StringBuilder();
			sb.Append("name: " + track.Name).AppendLine();
			sb.Append("id: " + track.Id).AppendLine();
			sb.Append("preview: " + track.PreviewUrl).AppendLine();
			return sb.ToString();
		}

		#endregion Public Method

	}
}
