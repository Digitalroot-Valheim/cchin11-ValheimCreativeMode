﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ValheimCreativeMode
{
	// Only check for "devcommands" command at console to enable cheats
	//	This prevents the default behaviour of checking if we're on a dedicated server
    [HarmonyPatch(typeof(Console), "IsCheatsEnabled")]
    class Console_IsCheatsEnabled_Patch
    {
        static bool Prefix(ref bool __result, bool ___m_cheat)
        {
            if (___m_cheat)
            {
                __result = true;
            }
            return false;
        }

    }

	// Listen for custom commands
    [HarmonyPatch(typeof(Console), "Update")]
    class Console_Update_Patch
    {
        static void Postfix(Console __instance, string ___m_lastEntry)
        {
			if (Input.GetKeyDown(KeyCode.Return))
			{
				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm damage")
				{
					ValheimCheats.commands.ToggleDamageCheat();
					__instance.Print("max damage: " + ValheimCheats.commands.damageCheatEnabled);
				}

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm stamina")
				{
					ValheimCheats.commands.ToggleStaminaCheat();
					__instance.Print("infinite stamina: " + ValheimCheats.commands.staminaCheatEnabled);
				}

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm nobreak")
				{
					ValheimCheats.commands.ToggleWearNTearCheat();
					__instance.Print("no damage on buildings: " + ValheimCheats.commands.wearNTearCheatEnabled);
				}

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm nodrop")
				{
					ValheimCheats.commands.ToggleDropItemsCheat();
					__instance.Print("prevent items from dropping on death: " + ValheimCheats.commands.dropItemsCheatEnabled);
				}

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm fireplace")
                {
					ValheimCheats.commands.ToggleFireplaceCheat();
					__instance.Print("prevent fireplaces from running out: " + ValheimCheats.commands.fireplaceCheatEnabled);
                }

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower() == "vcm building")
				{
					ValheimCheats.commands.ToggleBuildingCheat();
					__instance.Print("allow free rotation of building objects: " + ValheimCheats.commands.buildingCheatEnabled);
				}

				if (__instance.IsCheatsEnabled() && ___m_lastEntry.ToLower().Contains("vcm raiseskills"))
                {
					int num = int.Parse(___m_lastEntry.Split(' ')[2]);
					Player.m_localPlayer.GetSkills().CheatResetSkill("run");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("run", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("clubs");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("clubs", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("bows");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("bows", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("swim");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("swim", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("knives");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("knives", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("sneak");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("sneak", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("blocking");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("blocking", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("unarmed");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("unarmed", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("axes");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("axes", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("pickaxes");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("pickaxes", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("jump");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("jump", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("swords");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("swords", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("spears");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("spears", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("polearms");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("polearms", (float)num);

					Player.m_localPlayer.GetSkills().CheatResetSkill("woodcutting");
					Player.m_localPlayer.GetSkills().CheatRaiseSkill("woodcutting", (float)num);
					__instance.Print("All skills raised to " + num.ToString());
				}
            }
        }
    }

	// Check for commands - run them if cheats are enabled and we're on a dedicated server
	[HarmonyPatch(typeof(Console), "InputText")]
	class Console_InputText_Patch
	{
		static void Postfix(Console __instance)
		{
			string text = __instance.m_input.text;
			string[] array = text.Split(new char[]
			{' '});

			// do not enable unless we are on dedicated server
			if (__instance.IsCheatsEnabled() && !ZNet_Patch.m_isServer)
			{
				if (array[0] == "players" && array.Length >= 2)
				{
					int num3;
					if (int.TryParse(array[1], out num3))
					{
						Game.instance.SetForcePlayerDifficulty(num3);
						__instance.Print("Setting players to " + num3);
					}
					return;
				}

				if (array[0] == "setkey")
				{
					if (array.Length >= 2)
					{
						ZoneSystem.instance.SetGlobalKey(array[1]);
						__instance.Print("Setting global key " + array[1]);
					}
					else
					{
						__instance.Print("Syntax: setkey [key]");
					}
				}

				if (array[0] == "resetkeys")
				{
					ZoneSystem.instance.ResetGlobalKeys();
					__instance.Print("Global keys cleared");
				}

				if (array[0] == "listkeys")
				{
					List<string> globalKeys = ZoneSystem.instance.GetGlobalKeys();
					__instance.Print("Keys " + globalKeys.Count);
					foreach (string text2 in globalKeys)
					{
						__instance.Print(text2);
					}
				}

				if (array[0] == "debugmode")
				{
					Player.m_debugMode = !Player.m_debugMode;
					__instance.Print("Debugmode " + Player.m_debugMode.ToString());
				}

				if (array[0] == "raiseskill")
				{
					if (array.Length > 2)
					{
						string name = array[1];
						int num4 = int.Parse(array[2]);
						Player.m_localPlayer.GetSkills().CheatRaiseSkill(name, (float)num4);
						return;
					}
					__instance.Print("Syntax: raiseskill [skill] [amount]");
					return;
				}
				if (array[0] == "resetskill")
				{
					if (array.Length > 1)
					{
						string name2 = array[1];
						Player.m_localPlayer.GetSkills().CheatResetSkill(name2);
						return;
					}
					__instance.Print("Syntax: resetskill [skill]");
					return;
				}

				if (array[0] == "sleep")
				{
					EnvMan.instance.SkipToMorning();
					return;
				}

				if (array[0] == "skiptime")
				{
					double num5 = ZNet.instance.GetTimeSeconds();
					float num6 = 240f;
					if (array.Length > 1)
					{
						num6 = float.Parse(array[1]);
					}
					num5 += (double)num6;
					ZNet.instance.SetNetTime(num5);
					__instance.Print(string.Concat(new object[]
					{
						"Skipping ",
						num6.ToString("0"),
						"s , Day:",
						EnvMan.instance.GetDay(num5)
					}));
					return;
				}

				if (text == "resetcharacter")
				{
					__instance.Print("Reseting character");
					Player.m_localPlayer.ResetCharacter();
					return;
				}

				if (array[0] == "randomevent")
				{
					RandEventSystem.instance.StartRandomEvent();
					return;
				}

				if (text.StartsWith("event "))
				{
					if (array.Length <= 1)
					{
						return;
					}
					string text3 = text.Substring(6);
					if (!RandEventSystem.instance.HaveEvent(text3))
					{
						__instance.Print("Random event not found:" + text3);
						return;
					}
					RandEventSystem.instance.SetRandomEventByName(text3, Player.m_localPlayer.transform.position);
					return;
				}

				if (array[0] == "stopevent")
				{
					RandEventSystem.instance.ResetRandomEvent();
					return;
				}

				if (text.StartsWith("removedrops"))
				{
					__instance.Print("Removing item drops");
					ItemDrop[] array2 = UnityEngine.Object.FindObjectsOfType<ItemDrop>();
					for (int i = 0; i < array2.Length; i++)
					{
						ZNetView component = array2[i].GetComponent<ZNetView>();
						if (component && component.IsValid() && component.IsOwner())
						{
							component.Destroy();
						}
					}
					return;
				}

				if (text.StartsWith("freefly"))
				{
					__instance.Print("Toggling free fly camera");
					GameCamera.instance.ToggleFreeFly();
					return;
				}

				if (array[0] == "ffsmooth")
				{
					if (array.Length <= 1)
					{
						__instance.Print(GameCamera.instance.GetFreeFlySmoothness().ToString());
						return;
					}
					float num7;
					if (!float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num7))
					{
						__instance.Print("syntax error");
						return;
					}
					__instance.Print("Setting free fly camera smoothing:" + num7);
					GameCamera.instance.SetFreeFlySmoothness(num7);
					return;
				}

				if (text.StartsWith("location "))
				{
					if (array.Length <= 1)
					{
						return;
					}
					string name3 = text.Substring(9);
					Vector3 pos = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 10f;
					ZoneSystem.instance.TestSpawnLocation(name3, pos);
					return;
				}

				if (array[0] == "spawn")
				{
					if (array.Length <= 1)
					{
						return;
					}
					string text4 = array[1];
					int num8 = (array.Length >= 3) ? int.Parse(array[2]) : 1;
					int num9 = (array.Length >= 4) ? int.Parse(array[3]) : 1;
					GameObject prefab = ZNetScene.instance.GetPrefab(text4);
					if (!prefab)
					{
						Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Missing object " + text4, 0, null);
						return;
					}
					DateTime now = DateTime.Now;
					if (num8 == 1)
					{
						Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + text4, 0, null);
						Character component2 = UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
						if (component2 & num9 > 1)
						{
							component2.SetLevel(num9);
						}
					}
					else
					{
						for (int j = 0; j < num8; j++)
						{
							Vector3 b = UnityEngine.Random.insideUnitSphere * 0.5f;
							Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + text4, 0, null);
							Character component3 = UnityEngine.Object.Instantiate<GameObject>(prefab, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up + b, Quaternion.identity).GetComponent<Character>();
							if (component3 & num9 > 1)
							{
								component3.SetLevel(num9);
							}
						}
					}
					ZLog.Log("Spawn time :" + (DateTime.Now - now).TotalMilliseconds + " ms");
					Gogan.LogEvent("Cheat", "Spawn", text4, (long)num8);
					return;
				}

				if (array[0] == "pos")
				{
					Player localPlayer = Player.m_localPlayer;
					if (localPlayer)
					{
						__instance.Print("Player position (X,Y,Z):" + localPlayer.transform.position.ToString("F0"));
					}
					return;
				}

				if (text.StartsWith("goto "))
				{
					string text5 = text.Substring(5);
					char[] separator = new char[]
					{
						',',
						' '
					};
					string[] array3 = text5.Split(separator);
					if (array3.Length < 2)
					{
						__instance.Print("Syntax /goto x,y");
						return;
					}
					try
					{
						float x = float.Parse(array3[0]);
						float z = float.Parse(array3[1]);
						Player localPlayer2 = Player.m_localPlayer;
						if (localPlayer2)
						{
							Vector3 pos2 = new Vector3(x, localPlayer2.transform.position.y, z);
							localPlayer2.TeleportTo(pos2, localPlayer2.transform.rotation, true);
						}
					}
					catch (Exception ex)
					{
						ZLog.Log("parse error:" + ex.ToString() + "  " + text5);
					}
					Gogan.LogEvent("Cheat", "Goto", "", 0L);
					return;
				}

				if (text.StartsWith("exploremap"))
				{
					Minimap.instance.ExploreAll();
					return;
				}

				if (text.StartsWith("resetmap"))
				{
					Minimap.instance.Reset();
					return;
				}

				if (text.StartsWith("puke") && Player.m_localPlayer)
				{
					Player.m_localPlayer.ClearFood();
					return;
				}

				if (text.StartsWith("tame"))
				{
					Tameable.TameAllInArea(Player.m_localPlayer.transform.position, 20f);
					return;
				}

				if (text.StartsWith("killall"))
				{
					foreach (Character character in Character.GetAllCharacters())
					{
						if (!character.IsPlayer())
						{
							HitData hitData = new HitData();
							hitData.m_damage.m_damage = 1E+10f;
							character.Damage(hitData);
						}
					}
					return;
				}

				if (text.StartsWith("heal"))
				{
					Player.m_localPlayer.Heal(Player.m_localPlayer.GetMaxHealth(), true);
					return;
				}

				if (text.StartsWith("god"))
				{
					Player.m_localPlayer.SetGodMode(!Player.m_localPlayer.InGodMode());
					__instance.Print("God mode:" + Player.m_localPlayer.InGodMode().ToString());
					Gogan.LogEvent("Cheat", "God", Player.m_localPlayer.InGodMode().ToString(), 0L);
					return;
				}

				if (text.StartsWith("ghost"))
				{
					Player.m_localPlayer.SetGhostMode(!Player.m_localPlayer.InGhostMode());
					__instance.Print("Ghost mode:" + Player.m_localPlayer.InGhostMode().ToString());
					Gogan.LogEvent("Cheat", "Ghost", Player.m_localPlayer.InGhostMode().ToString(), 0L);
					return;
				}

				if (text.StartsWith("beard"))
				{
					string beard = (text.Length >= 6) ? text.Substring(6) : "";
					if (Player.m_localPlayer)
					{
						Player.m_localPlayer.SetBeard(beard);
					}
					return;
				}

				if (text.StartsWith("hair"))
				{
					string hair = (text.Length >= 5) ? text.Substring(5) : "";
					if (Player.m_localPlayer)
					{
						Player.m_localPlayer.SetHair(hair);
					}
					return;
				}

				if (text.StartsWith("model "))
				{
					string s = text.Substring(6);
					int playerModel;
					if (Player.m_localPlayer && int.TryParse(s, out playerModel))
					{
						Player.m_localPlayer.SetPlayerModel(playerModel);
					}
					return;
				}

				if (text.StartsWith("tod "))
				{
					float num10;
					if (!float.TryParse(text.Substring(4), NumberStyles.Float, CultureInfo.InvariantCulture, out num10))
					{
						return;
					}
					__instance.Print("Setting time of day:" + num10);
					if (num10 < 0f)
					{
						EnvMan.instance.m_debugTimeOfDay = false;
					}
					else
					{
						EnvMan.instance.m_debugTimeOfDay = true;
						EnvMan.instance.m_debugTime = Mathf.Clamp01(num10);
					}
					return;
				}

				if (array[0] == "env" && array.Length > 1)
				{
					string text6 = text.Substring(4);
					__instance.Print("Setting debug enviornment:" + text6);
					EnvMan.instance.m_debugEnv = text6;
					return;
				}

				if (text.StartsWith("resetenv"))
				{
					__instance.Print("Reseting debug enviornment");
					EnvMan.instance.m_debugEnv = "";
					return;
				}

				if (array[0] == "wind" && array.Length == 3)
				{
					float angle = float.Parse(array[1]);
					float intensity = float.Parse(array[2]);
					EnvMan.instance.SetDebugWind(angle, intensity);
					return;
				}

				if (array[0] == "resetwind")
				{
					EnvMan.instance.ResetDebugWind();
					return;
				}
			}
		}
	}
}