using NoteColorForceOverride.AffinityPatches;
using Zenject;

namespace NoteColorForceOverride.Installers
{
	internal class NoteColorInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<NoteColorPatches>().AsSingle();
		}
	}
}
