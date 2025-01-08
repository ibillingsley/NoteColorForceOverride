using System;
using HarmonyLib;
using SiraUtil.Affinity;
using IPA.Loader;
using System.Linq;
using SongCore.UI;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Components.Settings;

namespace NoteColorForceOverride.AffinityPatches
{
	public class NoteColorPatches : IAffinity
	{
		readonly ColorsUI colorsUI;
		readonly GameplaySetup gameplaySetup;

		public NoteColorPatches(ColorsUI _colorsUI, GameplaySetup _gameplaySetup)
		{
			colorsUI = _colorsUI;
			gameplaySetup = _gameplaySetup;
		}

		[AffinityPostfix]
		[AffinityPatch(typeof(ColorsOverrideSettingsPanelController), "HandleOverrideColorsToggleValueChanged")]
		public void NoteColorTogglePatch(ColorsOverrideSettingsPanelController __instance, bool isOn)
		{
			// Toggle SongCore setting "Allow Custom Song Note Colors"
			colorsUI.NoteColors = !isOn;
			colorsUI.NotifyPropertyChanged("NoteColors"); // Update toggle UI

			var chroma = PluginManager.GetPluginFromId("Chroma");
			if (chroma != null)
			{
				// Toggle Chroma setting "Disable Note Coloring"
				var settingsType = chroma.Assembly.GetTypes().First((Type t) => t.Name == "ChromaSettableSettings");
				var noteColoringDisabledSetting = Traverse.Create(settingsType).Property("NoteColoringDisabledSetting").GetValue();
				Traverse.Create(noteColoringDisabledSetting).Property("Value").SetValue(isOn);

				// Update toggle UI
				GameplaySetupMenu menu = gameplaySetup.menus.FirstOrDefault(m => m.Name == "Chroma");
				if (menu != null)
				{
					var toggles = menu.tabObject.GetComponentsInChildren<ToggleSetting>();
					toggles[2].OnValueChanged(isOn); // Hardcoding this sucks but I don't know a better way to find the right toggle
				}
			}
		}
	}
}
