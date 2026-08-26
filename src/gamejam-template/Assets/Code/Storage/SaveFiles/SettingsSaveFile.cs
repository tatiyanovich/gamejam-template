using System;
using Framework.Storage;

namespace Code.Storage.SaveFiles
{
	[Serializable]
	public class SettingsSaveFile : ISaveFile
	{
		public bool MusicEnabled = true;
		public bool EffectsEnabled = true;
		public bool HighQuality = true;
		public int Framerate = Constants.GraphicsQuality.MidFramerate;
	}
}
