using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine;
using static Silence.Core;
using static Silence.Logger;

namespace Silence
{
    public class Patches
    {
        //[HarmonyPatch(typeof(SGMechTechPointsDisplay), nameof(SGMechTechPointsDisplay.Refresh))]
        //public static class SGMechTechPointsDisplay_Refresh_Patch
        //{
        //    public static void Postfix(SGMechTechPointsDisplay __instance)
        //    {
        //        __instance.NumMechTechText.fontSize = 14;
        //        __instance.NumMechTechText.enableWordWrapping = false;
        //    }
        //}
        //
        //// TODO check with generic patch
        //public static void PostEventPrefix(AudioEventList_vo eventEnumValue)
        //{
        //    LogDebug($"eventEnumValue {eventEnumValue}");
        //}

        [HarmonyPatch(typeof(WwiseManager), nameof(WwiseManager.PostEventByName))]
        public static class WwiseManager_PostEventByName_Patch
        {
            public static bool Prefix(string eventName)
            {
                LogDebug($"PostEventByName eventName {eventName}");

                if (!modSettings.EnableTorsoTwistingSounds && eventName.Contains("torso_rotate"))
                {
                    LogDebug("Not playing torso rotation sound");
                    return false;
                }

                if (!modSettings.EnableArgoAmbient && eventName.Contains("argo_ambient"))
                {
                    LogDebug("Not playing Argo ambient sound");
                    return false;
                }

                return true;
            }
        }

        //public static class WwiseManager_PostEvent_Patch
        //{
        //    public static bool PostEventPrefix()
        //    {
        //        LogDebug($"PostEvent generic");
        //
        //        return true;
        //    }
        //}
        //
        //[HarmonyPatch(typeof(MessageCenter), "PublishMessage")]
        //public class MessageCenterPatch
        //{
        //    static void Postfix(MessageCenter __instance, MessageCenterMessage message)
        //    {
        //        //LogDebug(message.ToString());
        //        //if (message is LevelLoadCompleteMessage)
        //        //{
        //        //    LevelLoadCompleteMessage levelLoadCompleteMessage = (LevelLoadCompleteMessage) message;
        //        //
        //        //}
        //    }
        //}

        // working
        [HarmonyPatch(typeof(AudioEventManager), nameof(AudioEventManager.PlayAudioEvent), MethodType.Normal)]
        [HarmonyPatch(new Type[] {typeof(string), typeof(string), typeof(AkGameObj), typeof(AkCallbackManager.EventCallback)})]
        public static class AudioEventManager_PlayAudioEvent_Patch
        {
            public static bool Prefix(string defId, string eventId, AkGameObj audioObject, AkCallbackManager.EventCallback audioEventCallback, ref bool __result)
            {
                // play everything except contract_lanceconfirm because you can't launch a mission without it
                if (modSettings.EnableBarks || defId != "audioeventdef_simgame_vo_barks") return true;
                if (eventId == "contract_lanceconfirm" || eventId == "skirmish_lanceconfirm")
                {
                    __result = AudioEventManager.PlayAudioEvent(AudioEventManager.GetAudioEvent(defId, eventId), audioObject, audioEventCallback);
                }

                return false;
            }
        }

        // working
        [HarmonyPatch(typeof(SGBarracksWidget), nameof(SGBarracksWidget.PilotClicked), MethodType.Normal)]
        public static class SGBarracksWidget_PilotClicked_Patch
        {
            public static bool Prefix(SGBarracksWidget __instance, SGBarracksRosterSlot slot)
            {
                var widget = __instance;
                bool newPilotClicked = widget.rosterList.SelectedSlot != slot;
                widget.rosterList.OnSlotSelected(slot);
                if (newPilotClicked)
                {
                    if (modSettings.EnablePilotBarks)
                    {
                        widget.mechWarriorDetails.PlayPilotChosenVO(slot.Pilot);
                    }
                }

                return false;
            }
        }

        // working
        [HarmonyPatch(typeof(SG_HiringHall_Screen), nameof(SGBarracksWidget.PilotClicked), MethodType.Normal)]
        public static class SG_HiringHall_Screen_PilotClicked_Patch
        {
            public static bool Prefix(SG_HiringHall_Screen __instance, SGBarracksRosterSlot slot)
            {
                var widget = __instance;
                bool newPilotClicked = widget.RosterList.SelectedSlot != slot;
                widget.RosterList.OnSlotSelected(slot);
                if (newPilotClicked)
                {
                    if (modSettings.EnablePilotBarks)
                    {
                        widget.DetailPanel.PlayPilotSelectionVO(slot.Pilot);
                    }
                }

                return false;
            }
        }
    }
}