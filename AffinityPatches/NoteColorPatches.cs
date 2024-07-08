using System;
using HarmonyLib;
using SiraUtil.Affinity;
using IPA.Loader;
using System.Linq;

namespace NoteColorForceOverride.AffinityPatches
{
	public class NoteColorPatches : IAffinity
	{
		[AffinityPostfix]
		[AffinityPatch(typeof(ColorsOverrideSettingsPanelController), "HandleOverrideColorsToggleValueChanged")]
		public void NoteColorTogglePatch(ColorsOverrideSettingsPanelController __instance, bool isOn)
		{
			var songCoreConfig = Traverse.Create(typeof(SongCore.Plugin)).Property("Configuration").GetValue();
			Traverse.Create(songCoreConfig).Property("CustomSongNoteColors").SetValue(!isOn);

			var chroma = PluginManager.GetPluginFromId("Chroma");
			if (chroma != null)
			{
				var settingsType = chroma.Assembly.GetTypes().First((Type t) => t.Name == "ChromaSettableSettings");
				var noteColoringDisabledSetting = Traverse.Create(settingsType).Property("NoteColoringDisabledSetting").GetValue();
				Traverse.Create(noteColoringDisabledSetting).Property("Value").SetValue(isOn);
			}
		}
	}
}
