using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA.Loader;
using IPA.Utilities;
using SiraUtil.Affinity;
using SongCore.UI;
using UnityEngine;

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
			colorsUI.InvokeMethod<object, ColorsUI>("NotifyPropertyChanged", "NoteColors"); // Update toggle UI

			var chroma = PluginManager.GetPluginFromId("Chroma");
			if (chroma != null)
			{
				// Toggle Chroma setting "Disable Note Coloring"
				var settingsType = chroma.Assembly.GetTypes().First((Type t) => t.Name == "ChromaSettableSettings");
				var noteColoringDisabledSetting = Traverse.Create(settingsType).Property("NoteColoringDisabledSetting").GetValue();
				Traverse.Create(noteColoringDisabledSetting).Property("Value").SetValue(isOn);

				// Update toggle UI
				var menus = Traverse.Create(gameplaySetup).Field("menus").GetValue<IEnumerable<object>>();
				var menu = menus.FirstOrDefault(m => Traverse.Create(m).Property("Name").GetValue<string>() == "Chroma");
				if (menu != null)
				{
					var toggles = Traverse.Create(menu).Field("tabObject").GetValue<GameObject>().GetComponentsInChildren<ToggleSetting>();
					toggles[2].Value = isOn; // Hardcoding this sucks but I don't know a better way to find the right toggle
				}
			}
		}
	}
}
