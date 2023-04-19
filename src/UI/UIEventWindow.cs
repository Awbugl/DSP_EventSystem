using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static DSP_EventSystem.Util;

namespace DSP_EventSystem
{
    /// <summary>
    ///   special thanks to https://github.com/hetima/DSP_PlanetFinder/tree/main/PlanetFinder
    ///   special thanks to https://github.com/starfi5h/DSP_Mod_Support/tree/main/FactoryLocator/src/UI
    /// </summary>
    public class UIEventWindow : ManualBehaviour
    {
        private RectTransform _windowTrans;
        private Text _nameText;

        private RectTransform _tab1;

        private int _currentEffectCount = 4;

        private UIButton[] _effectBtns = new UIButton[4];
        private Text _effectText;
        private Effect[] _currentEffects;

        internal static UIEventWindow CreateWindow()
        {
            var win = CreateWindow<UIEventWindow>("UIEventWindow",
                                                        Localization.language == Language.zhCN ? "异常现象已发现" : "Event Investigation");
            return win;
        }

        public void OpenWindow() => Util.OpenWindow(this);

        protected override void _OnCreate()
        {
            _windowTrans = this.GetRectTransform();
            _windowTrans.SetRectTransformSize(new Vector2(480f, 200f));
            CreateUI();
        }

        private void CreateUI()
        {
            var tab1 = new GameObject();
            _tab1 = tab1.AddComponent<RectTransform>();
            tab1.name = "tab-1";

            _nameText = CreateText("_nameText", 16);
            _nameText.transform.NormalizeRectWithTopLeft(0, 20, _tab1);

            _effectText = CreateText("_effectText", 16);
            _effectText.alignment = TextAnchor.UpperLeft;
            _effectText.alignByGeometry = true;
            _effectText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _effectText.transform.NormalizeRectWithTopLeft(0, 60, _tab1);

            for (var i = 0; i < _currentEffectCount; ++i) CreateEffectUI(i);
        }

        private void CreateEffectUI(int id)
        {
            var effectBtn = CreateButton("Effect" + id, 40, 20);
            _effectBtns[id] = effectBtn;
            effectBtn.onClick += _ => OnBtnClick(id);
        }

        protected override void _OnUpdate()
        {
            if (VFInput.escape)
            {
                VFInput.UseEscape();
                _Close();
            }
        }

        internal void SetEvent(PlanetData planet, Event @event)
        {
            Dictionary<string, StringProto> trans = @event.Translations.ToDictionary(i => i.Name);
            var lang = Localization.language;

            string Translate(string s)
            {
                var proto = trans[s];

                var result = s;

                switch (lang)
                {
                    case Language.zhCN:
                        result = proto.ZHCN;
                        break;


                    case Language.enUS:
                        result = proto.ENUS;
                        break;


                    case Language.frFR:
                        result = proto.FRFR ?? proto.ENUS;
                        break;
                }

                return result.Replace("${PlanetName}", planet.displayName);
            }

            string TranslateEffect(string s, Dictionary<EffectType, int[]> effectvalue = null)
            {
                var result = Translate(s);

                if (effectvalue?.ContainsKey(EffectType.AddTechHash) == true)
                {
                    result = result.Replace("${TechName}", LDB.techs.Select(effectvalue[EffectType.AddTechHash][0]).Name.Translate())
                                   .Replace("${TechProgress}", effectvalue[EffectType.AddTechHash][1].ToString());
                }

                if (effectvalue?.ContainsKey(EffectType.AddItem) == true)
                {
                    result = result.Replace("${ItemName}", LDB.items.Select(effectvalue[EffectType.AddItem][0]).Name.Translate())
                                   .Replace("${ItemCount}", effectvalue[EffectType.AddItem][1].ToString());
                }
                
                if (effectvalue?.ContainsKey(EffectType.AddVein) == true)
                {
                    result = result.Replace("${VeinName}", LDB.veins.Select(effectvalue[EffectType.AddVein][0]).Name.Translate())
                                   .Replace("${VeinCount}", effectvalue[EffectType.AddVein][1].ToString());
                }
                
                return result;
            }

            _currentEffects = @event.Effects;
            _currentEffectCount = @event.Effects.Length;
            _nameText.text = planet.displayName + " - " + Translate(@event.Name);
            _effectText.text = Translate(@event.Description);

            var y = (int)((_effectText.preferredHeight + 2.0) / 2.0) * 2;
            _effectText.rectTransform.SetRectTransformSize(new Vector2(400f, y));
            _windowTrans.SetRectTransformSize(new Vector2(480f, y + 160 + @event.Effects.Length * 30));

            // preferredHeight changed after SetRectTransformSize so need to reset it
            y = (int)((_effectText.preferredHeight + 2.0) / 2.0) * 2;
            _windowTrans.SetRectTransformSize(new Vector2(480f, y + 160 + @event.Effects.Length * 30));
            
            _tab1.NormalizeRectWithMargin(40, 40, 40, 40, false, _windowTrans);
            _nameText.transform.NormalizeRectWithTopLeft(0, 20, _tab1);
            _effectText.transform.NormalizeRectWithTopLeft(0, 60, _tab1);

            var effectBtnsLength = _effectBtns.Length;

            if (effectBtnsLength < _currentEffectCount)
            {
                _effectBtns = new UIButton[_currentEffectCount];

                for (var i = effectBtnsLength; i < _currentEffectCount; ++i) CreateEffectUI(i);
            }

            for (var i = 0; i < @event.Effects.Length; ++i)
            {
                var effectBtn = _effectBtns[i];
                effectBtn.gameObject.SetActive(true);
                var eventEffect = @event.Effects[i];
                var tipsTipTitle = Translate(eventEffect.Name);
                effectBtn.SetUIButtonText(tipsTipTitle);
                effectBtn.tips.tipText = TranslateEffect(eventEffect.Description, eventEffect.Value);
                effectBtn.tips.tipTitle = tipsTipTitle;
                effectBtn.UpdateTip();
                effectBtn.transform.NormalizeRectWithTopLeft(0, y + 80 + i * 30, _tab1);
                var btnSize = new Vector2(400f, 25f);
                ((RectTransform)effectBtn.transform).SetRectTransformSize(btnSize);
            }

            for (var i = @event.Effects.Length; i < effectBtnsLength; ++i) _effectBtns[i].gameObject.SetActive(false);

            OpenWindow();
        }

        private void OnBtnClick(int id)
        {
            var effect = _currentEffects[id];

            if (effect.OnEffect != null) effect.OnEffect();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<EffectType, EventSystem.Awards> pair in EventSystem.AwardsMap)
            {
                if ((effect.Type & pair.Key) > 0) pair.Value(effect.Value[pair.Key]);
            }

            _Close();
        }
    }
}
