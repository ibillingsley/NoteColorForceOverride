using System;
using HarmonyLib;
using SiraUtil.Affinity;

namespace NoteColorForceOverride.AffinityPatches
{
	public class NoteColorPatches : IAffinity
	{
		[AffinityPostfix]
		[AffinityPatch(typeof(ColorsOverrideSettingsPanelController), "HandleOverrideColorsToggleValueChanged")]
		public void NoteColorTogglePatch(ColorsOverrideSettingsPanelController __instance, bool isOn)
		{
			try
			{
				var songCoreConfig = Traverse.CreateWithType("SongCore.Plugin").Property("Configuration").GetValue();
				Traverse.Create(songCoreConfig).Property("CustomSongNoteColors").SetValue(!isOn);
			}
			catch (Exception)
			{ }

			try
			{
				var chromaSetting = Traverse.CreateWithType("Chroma.Settings.ChromaSettableSettings").Property("NoteColoringDisabledSetting").GetValue();
				Traverse.Create(chromaSetting).Property("Value").SetValue(isOn);
			}
			catch (Exception)
			{ }
		}
	}
}
