using IPA;
using NoteColorForceOverride.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace NoteColorForceOverride
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }

		[Init]
		public Plugin(IPALogger logger, Zenjector zenjector)
		{
			Instance = this;
			Log = logger;
			zenjector.UseLogger(logger);
			zenjector.Install<NoteColorInstaller>(Location.Menu);
		}
	}
}
