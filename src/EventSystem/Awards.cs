using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSP_EventSystem
{
    public static partial class EventSystem
    {
        internal delegate void Awards(int[] value);

        internal static readonly Dictionary<EffectType, Awards> AwardsMap = new Dictionary<EffectType, Awards>
                                                                            {
                                                                                { EffectType.AddItem, AddItemAwards },
                                                                                { EffectType.AddVein, AddVeinAwards },
                                                                                { EffectType.AddEntity, AddEntityAwards },
                                                                                { EffectType.AddTechHash, AddTechHashAwards },
                                                                                { EffectType.TriggerItemEvent, TriggerItemEventAwards },
                                                                                { EffectType.TriggerVeinEvent, TriggerVeinEventAwards },
                                                                                { EffectType.TriggerEntityEvent, TriggerEntityEventAwards },
                                                                                { EffectType.TriggerPlanetEvent, TriggerPlanetEventAwards },
                                                                                { EffectType.TriggerStarEvent, TriggerStarEventAwards },
                                                                                { EffectType.PlanetEffect, PlanetEffectAwards },
                                                                                { EffectType.StarEffect, StarEffectAwards },
                                                                            };

        // TODO: Implement all awards

        public static void AddItemAwards(int[] value)
        {
            for (var i = 0; i < value.Length; i += 2) GameMain.history.GainTechAwards(value[i], value[i + 1]);
        }

        public static void AddVeinAwards(int[] value)
        {
            for (var i = 0; i < value.Length; i += 2)
            {
                var pos = GameMain.mainPlayer.position;

                var factory = GameMain.mainPlayer.planetData.factory;
                var planet = factory.planet;

                var veinProto = LDB.veins.Select(value[i]);
                if (veinProto == null) return;
                var veinData = new VeinData { type = (EVeinType)veinProto.ID, modelIndex = (short)veinProto.ModelIndex, amount = value[i + 1] };

                if (veinData.amount < 1) veinData.amount = 1;
                if (veinData.amount > 1000000000) veinData.amount = 1000000000;
                veinData.productId = veinProto.MiningItem;

                veinData.pos = pos;
                if (veinData.type == EVeinType.Oil)
                {
                    veinData.pos = planet.aux.RawSnap(veinData.pos);
                    veinData.pos = veinData.pos.normalized * planet.data.QueryModifiedHeight(pos);
                }

                veinData.minerCount = 0;
                factory.AssignGroupIndexForNewVein(ref veinData);
                var index1 = factory.AddVeinData(veinData);
                VeinData[] veinPool = factory.veinPool;
                veinPool[index1].modelId
                    = planet.factoryModel.gpuiManager.AddModel(veinPool[index1].modelIndex, index1, veinPool[index1].pos,
                                                               Maths.SphericalRotation(pos, 0.0f));
                ColliderData[] colliders = veinProto.prefabDesc.colliders;
                
                for (var index2 = 0; colliders != null && index2 < colliders.Length; ++index2)
                {
                    veinPool[index1].colliderId
                        = planet.physics.AddColliderData(colliders[index2].BindToObject(index1, veinPool[index1].colliderId, EObjectType.Vein,
                                                                                        veinPool[index1].pos,
                                                                                        Quaternion.FromToRotation(Vector3.up,
                                                                                                                  veinPool[index1].pos.normalized)));
                }

                factory.RefreshVeinMiningDisplay(index1, 0, 0);

                factory.RecalculateVeinGroup(factory.veinPool[index1].groupIndex);
            }
        }

        public static void AddEntityAwards(int[] value) => throw new NotImplementedException();

        public static void AddTechHashAwards(int[] value)
        {
            for (var i = 0; i < value.Length; i += 2)
            {
                var techState = GameMain.history.techStates[value[i]];

                if (techState.unlocked) continue;

                techState.hashUploaded += Math.Min(techState.hashNeeded - techState.hashUploaded, value[i + 1] * techState.hashNeeded / 100);

                GameMain.history.techStates[value[i]] = techState;
            }
        }

        public static void TriggerItemEventAwards(int[] value) => throw new NotImplementedException();

        public static void TriggerVeinEventAwards(int[] value) => throw new NotImplementedException();

        public static void TriggerEntityEventAwards(int[] value) => throw new NotImplementedException();

        public static void TriggerPlanetEventAwards(int[] value) => throw new NotImplementedException();

        public static void TriggerStarEventAwards(int[] value) => throw new NotImplementedException();

        public static void PlanetEffectAwards(int[] value) => throw new NotImplementedException();

        public static void StarEffectAwards(int[] value) => throw new NotImplementedException();
    }
}
