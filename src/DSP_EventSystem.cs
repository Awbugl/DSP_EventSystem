using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using crecheng.DSPModSave;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace DSP_EventSystem
{
    [BepInPlugin(MODGUID, MODNAME, VERSION)]
    public class CustomEventSystem : BaseUnityPlugin, IModCanSave
    {
        public const string MODGUID = "top.awbugl.DSP.CustomEventSystem";
        public const string MODNAME = "CustomEventSystem";
        public const string VERSION = "1.0.0";

        private void Awake()
        {
            EventSystem.Logger = Logger;
            Harmony.CreateAndPatchAll(typeof(EventSystem), MODGUID);
            Logger.LogInfo("CustomEventSystem JsonEvents:" + EventSystem.Events.Sum(i => i.Value.Count));
            Logger.LogInfo("CustomEventSystem EventTriggers:" + EventSystem.EventTriggers.Count);
        }

        public void Export(BinaryWriter w) => EventSystem.Export(w);

        public void Import(BinaryReader r) => EventSystem.Import(r);

        public void IntoOtherSave() => EventSystem.IntoOtherSave();
    }
}
