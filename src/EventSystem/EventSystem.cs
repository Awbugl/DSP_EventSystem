using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace DSP_EventSystem
{
    public static partial class EventSystem
    {
        private static VFAudio _vfAudio;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGame), "_OnCreate")]
        public static void UIGame_Init_Postfix(UIGame __instance) => _uiEventWindow = UIEventWindow.CreateWindow();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "Create")]
        public static void Player_Create_Postfix(Player __result)
        {
            var audioTransform = __result.audio.transform;
            _vfAudio = VFAudio.Create("landing", audioTransform, Vector3.zero);
            _vfAudio.transform.position = audioTransform.position;
            _vfAudio.occupied = true;
            _vfAudio.loopOverride = 0;
            _vfAudio.fadeInDuration = 0f;
            _vfAudio.pitchOverride = 1f;
            _vfAudio.spatialBlendOverride = 0f;

            var traverse = Traverse.Create(_vfAudio).Field("proto");
            traverse.SetValue(new AudioProto
                              {
                                  Pitch = 1,
                                  Volume = 0.4f,
                                  SpatialBlend = 1,
                                  ClipCount = 1,
                                  audioClip = AudioClip,
                                  audioClipGroup = new[] { AudioClip },
                                  PitchRandomness = 0f
                              });
        }

        [HarmonyPatch(typeof(GameScenarioLogic), "NotifyOnLandPlanet")]
        [HarmonyPostfix]
        public static void GameScenarioLogic_NotifyOnLandPlanet_Postfix(PlanetData planet)
        {
            Logger.LogInfo($"Landed on planet {planet.id}");

            if (_birthPlanetId < 0)
            {
                _birthPlanetId = planet.galaxy.birthPlanetId;
                _landedPlanets.Add(_birthPlanetId);
            }

            if (_landedPlanets.Contains(planet.id)) return;

            _landedPlanets.Add(planet.id);

            if (Devmode || Random.NextDouble() < 0.15f)
            {
                Logger.LogInfo($"Trigger Event on planet {planet.id}");

                TriggerRandomPlanetEvent(planet);
            }
        }

        [CanBeNull]
        public static Event GetRandomEvent(this PlanetEventSubType planetEventSubType)
        {
            try
            {
                return PlanetEvents[planetEventSubType].GetRandomEvent();
            }
            catch (System.Exception e)
            {
                Logger.LogError($"Failed to get event by subtype {planetEventSubType}: {e}");
                return null;
            }
        }

        public static void TriggerRandomPlanetEvent(PlanetData planet)
        {
            var planetEventSubType = planet.type == EPlanetType.Gas ? PlanetEventSubType.GasGiant : PlanetEventSubType.Terrestrial;

            var @event = planetEventSubType.GetRandomEvent();

            if (@event != null) TriggerEvent(planet, @event);
        }

        public static void TriggerEvent(PlanetData planet, Event @event)
        {
            _uiEventWindow.SetEvent(planet, @event);
            _vfAudio.Play();
        }
    }
}
