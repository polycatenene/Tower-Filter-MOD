// TowerFilterMod.cs
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[BepInPlugin("com.polysolene.towerfilter", "Tower Filter Mod", "1.0.0")]
public class TowerFilterMod : BaseUnityPlugin
{
    private void Start()
    {
        Logger.LogInfo("TowerFilterMod loaded");
        Harmony.CreateAndPatchAll(typeof(TowerFilterMod));
    }

    #region Data structures

    // 单个塔的多分类 + 单等级信息
    private class TowerCategoryInfo
    {
        public string TowerName;
        public List<string> Categories = new List<string>();
        public string Level; // "灰","绿","蓝","紫"
    }

    // 默认的分类数据
    private static readonly List<TowerCategoryInfo> DefaultTowerInfos = new List<TowerCategoryInfo>()
    {
        new TowerCategoryInfo { TowerName = "AngryFairyTower", Categories = new List<string>{"辅助", "抽象" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "AppleTower", Categories = new List<string>{ "治疗", "临时", "抽象"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "ArmedGoldMine", Categories = new List<string>{ "攻击", "矿塔" }, Level = "灰" },
        new TowerCategoryInfo { TowerName = "ArrowTower", Categories = new List<string>{ "攻击" }, Level = "灰" },
        new TowerCategoryInfo { TowerName = "ArrowTowerEX", Categories = new List<string>{ "攻击" }, Level = "灰" },  // 组合箭塔
        new TowerCategoryInfo { TowerName = "ArrowTowerTower", Categories = new List<string>{ "攻击" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "AttackRandomTower", Categories = new List<string>{ "攻击", "抽象" }, Level = "灰" },

        new TowerCategoryInfo { TowerName = "BabyArrowTower", Categories = new List<string>{ "攻击", "抽象" }, Level = "灰" },
        new TowerCategoryInfo { TowerName = "BakaArrowTower", Categories = new List<string>{ "攻击", "抽象" }, Level = "灰" },  // 双重箭塔
        new TowerCategoryInfo { TowerName = "BarbedWall", Categories = new List<string>{ "攻击", "围墙" }, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "Bomb", Categories = new List<string>{ "攻击", "临时" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "BoobyTrap", Categories = new List<string>{ "攻击", "临时" }, Level = "绿" },  // 诡雷
        new TowerCategoryInfo { TowerName = "BookTower", Categories = new List<string>{ "矿塔", "攻击", "围墙", "治疗", "抽象", "晶塔" }, Level = "紫" },  // 图鉴塔
        new TowerCategoryInfo { TowerName = "BottleTower", Categories = new List<string>{ "抽象" }, Level = "灰" },  // 奶瓶塔
        new TowerCategoryInfo { TowerName = "BugTower", Categories = new List<string>{ "临时", "抽象" }, Level = "灰" },
        new TowerCategoryInfo { TowerName = "BullerProofWall", Categories = new List<string>{ "围墙" }, Level = "绿" },  // 防弹围墙

        new TowerCategoryInfo { TowerName = "CandyTowerA", Categories = new List<string>{ "治疗", "临时" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "CandyTowerB", Categories = new List<string>{ "辅助", "治疗",  "临时" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "Cannon", Categories = new List<string>{ "攻击" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CannonEX", Categories = new List<string>{ "攻击" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CastTower", Categories = new List<string>{ "攻击" }, Level = "灰" },
        new TowerCategoryInfo { TowerName = "CheeseTower", Categories = new List<string>{"攻击", "辅助", "临时", "抽象" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CoffeeTower", Categories = new List<string>{ "辅助", "临时", "抽象" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "Congealer", Categories = new List<string>{ "辅助" }, Level = "蓝" },  // 冷却塔
        new TowerCategoryInfo { TowerName = "Crossbow", Categories = new List<string>{ "攻击" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CrystalApple", Categories = new List<string>{ "治疗", "临时", "抽象" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CrystalMine", Categories = new List<string>{ "矿塔" }, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "CrystalWatermelonTower", Categories = new List<string>{ "矿塔", "临时", "抽象" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "CycloneTower", Categories = new List<string>{ "攻击" }, Level = "蓝" },  // 旋风塔
 
        new TowerCategoryInfo { TowerName = "DamageAmplifier", Categories = new List<string>{ "辅助" }, Level = "紫" },  // 伤害放大器
        new TowerCategoryInfo { TowerName = "DebugTower", Categories = new List<string>{ "攻击", "围墙", "抽象" }, Level = "灰" },  // 测试用塔
        new TowerCategoryInfo { TowerName = "DeepGoldMine", Categories = new List<string>{ "矿塔" }, Level = "绿" },

        new TowerCategoryInfo { TowerName = "EarthquakeTower", Categories = new List<string>{ "攻击", "围墙" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "EarthTower", Categories = new List<string>{ "辅助" }, Level = "紫" },  // 环境适应器
        new TowerCategoryInfo { TowerName = "ElectricWall", Categories = new List<string>{ "围墙" }, Level = "蓝" },

        new TowerCategoryInfo { TowerName = "FairyTower", Categories = new List<string>{ "攻击", "抽象" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "FireBomb", Categories = new List<string>{ "攻击", "临时" }, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "FirecrackerTower", Categories = new List<string>{ "攻击", "辅助" }, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "FireCyrstal", Categories = new List<string>{"攻击",  "晶塔" }, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "FreezeCrystal", Categories = new List<string>{ "攻击", "晶塔" }, Level = "绿" },

        new TowerCategoryInfo { TowerName = "Generator", Categories = new List<string>{ "辅助" }, Level = "蓝" },  // 发电机
        new TowerCategoryInfo { TowerName = "GhostTower", Categories = new List<string>{ "抽象" }, Level = "绿" },
        new TowerCategoryInfo { TowerName = "GiantBomb", Categories = new List<string>{ "攻击", "临时" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "GoldApple", Categories = new List<string>{ "治疗", "临时", "抽象" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "GoldenWatermelonTower", Categories = new List<string>{ "矿塔", "临时", "抽象" }, Level = "紫" },
        new TowerCategoryInfo { TowerName = "GoldMine", Categories = new List<string>{ "矿塔" }, Level = "灰" },

        new TowerCategoryInfo { TowerName = "HappyFairyTower", Categories = new List<string>{"攻击", "辅助", "抽象"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "HealCrystal", Categories = new List<string>{"治疗", "晶塔"}, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "HealCrystalEX", Categories = new List<string>{"治疗"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "HealthRandomTower", Categories = new List<string>{"围墙", "治疗", "抽象"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "HeroTower", Categories = new List<string>{"攻击"}, Level = "蓝" },  // 聚能塔
        new TowerCategoryInfo { TowerName = "HeroTowerEX", Categories = new List<string>{"攻击"}, Level = "紫" },  // 狙击塔


        new TowerCategoryInfo { TowerName = "IceBomb", Categories = new List<string>{"临时"}, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "IceGoldMine", Categories = new List<string>{"矿塔"}, Level = "灰" },

        new TowerCategoryInfo { TowerName = "LemonTower", Categories = new List<string>{"攻击"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "LemonTowerEX", Categories = new List<string>{"围墙", "攻击"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "LightningRod", Categories = new List<string>{"辅助"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "LockedTower", Categories = new List<string>{"围墙", "抽象"}, Level = "灰" },

        new TowerCategoryInfo { TowerName = "MatchTower", Categories = new List<string>{"辅助"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "MegaCastTower", Categories = new List<string>{"攻击"}, Level = "紫" },  // 巨型抛射塔
        new TowerCategoryInfo { TowerName = "MegaTesla", Categories = new List<string>{"攻击"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "MegaTeslaEX", Categories = new List<string>{"攻击"}, Level = "紫" },  // 终极电磁塔
        new TowerCategoryInfo { TowerName = "MiniBomb", Categories = new List<string>{"攻击", "临时"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "MoonTower", Categories = new List<string>{"攻击"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "MultipleArrowTower", Categories = new List<string>{"攻击"}, Level = "绿" },

        new TowerCategoryInfo { TowerName = "PauseFairyTower", Categories = new List<string>{"抽象"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "PowerBomb", Categories = new List<string>{"攻击", "临时"}, Level = "蓝" },  // 高爆炸弹

        new TowerCategoryInfo { TowerName = "Rampart", Categories = new List<string>{"围墙"}, Level = "紫" },  // 城墙
        new TowerCategoryInfo { TowerName = "RefractiveCrystal", Categories = new List<string>{"辅助", "晶塔"}, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "ResourceConvertor", Categories = new List<string>{"矿塔"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "ResourceRandomTower", Categories = new List<string>{"矿塔", "抽象"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "ReviveCrystal", Categories = new List<string>{"治疗", "晶塔"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "ReviveCrystalEX", Categories = new List<string>{"治疗", "晶塔"}, Level = "紫" },

        new TowerCategoryInfo { TowerName = "SacredBomb", Categories = new List<string>{"辅助", "治疗", "临时"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "SalaryTower", Categories = new List<string>{"辅助", "临时", "抽象"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "SecretTower", Categories = new List<string>{"抽象"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "SignTower", Categories = new List<string>{"抽象"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "SleepingArrowTower", Categories = new List<string>{"攻击", "抽象"}, Level = "灰" },  // 摸鱼箭塔
        new TowerCategoryInfo { TowerName = "SpotlightCrystal", Categories = new List<string>{"攻击", "晶塔"}, Level = "灰" },  // 激光晶塔
        new TowerCategoryInfo { TowerName = "StarTower", Categories = new List<string>{"攻击"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "StrengthArrowTower", Categories = new List<string>{"攻击"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "SuperSecretTower", Categories = new List<string>{"抽象"}, Level = "灰" },

        new TowerCategoryInfo { TowerName = "TempDeepGoldMine", Categories = new List<string>{"矿塔", "临时"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "TemperatureSensor", Categories = new List<string>{"矿塔", "临时"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "TemporaryArrowTower", Categories = new List<string>{"攻击", "临时"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "TemporaryFireCyrstal", Categories = new List<string>{"攻击", "临时", "晶塔"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "TemporaryHealCrystal", Categories = new List<string>{"治疗", "临时", "晶塔"}, Level = "绿" },
        new TowerCategoryInfo { TowerName = "ThermalTower", Categories = new List<string>{"辅助"}, Level = "蓝" },  // 热能塔

        new TowerCategoryInfo { TowerName = "Wall", Categories = new List<string>{"围墙"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "WallEX", Categories = new List<string>{"围墙"}, Level = "紫" },
        new TowerCategoryInfo { TowerName = "WarmTower", Categories = new List<string>{"辅助", "治疗", "临时"}, Level = "紫" },  // 保温塔
        new TowerCategoryInfo { TowerName = "WatermelonTower", Categories = new List<string>{"矿塔", "临时"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "WindAccelerator", Categories = new List<string>{"辅助"}, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "WitchTower", Categories = new List<string>{"攻击", "辅助"}, Level = "灰" },
        new TowerCategoryInfo { TowerName = "WizardTower", Categories = new List<string>{"攻击"}, Level = "蓝" },
        new TowerCategoryInfo { TowerName = "WorkingArrowTower", Categories = new List<string>{"攻击", "抽象"}, Level = "灰" },

        new TowerCategoryInfo { TowerName = "ZapTrap", Categories = new List<string>{"攻击", "临时"}, Level = "灰" }  // 电击陷阱
    };

    // 运行时的塔信息表
    private static List<TowerCategoryInfo> TowerInfos = new List<TowerCategoryInfo>(DefaultTowerInfos);

    // 筛选项定义
    private static readonly string[] CategoryList = { "矿塔", "攻击", "围墙", "治疗", "辅助", "临时", "抽象", "晶塔" };
    private static readonly string[] LevelList = { "灰", "绿", "蓝", "紫" };
    // 用于显示等级颜色（分别对应灰/绿/蓝/紫）
    private static readonly Color[] LevelColors = {
        new Color(0.65f, 0.63f, 0.6f), // 灰
        new Color(0.31f, 0.9f, 0.3f),   // 绿
        new Color(0.3f, 0.58f, 1f),   // 蓝
        new Color(0.78f, 0.39f, 0.95f)    // 紫
    };

    // 当前筛选状态
    private static HashSet<string> ActiveCategories = new HashSet<string>();
    private static string ActiveLevel = null; // null 表示未选任何等级（即所有等级都通过）

    #endregion


    #region Harmony Patch: 注入筛选 UI

    // 在 OpenSelectPanel 执行后插入筛选栏
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PreparePanelController), "OpenSelectPanel")]
    public static void PreparePanel_OpenSelectPanel_Postfix(PreparePanelController __instance)
    {
        try
        {
            if (__instance == null) return;

            var selectParent = __instance.selectTowerParent;
            if (selectParent == null) return;

            var panelParent = selectParent.parent as Transform;
            if (panelParent == null) return;

            // 已有则跳过
            if (panelParent.Find("TowerFilterBar") != null) return;

            // -------- 创建 FilterBar（放在 selectParent 之前） --------
            var filterBarGO = new GameObject("TowerFilterBar", typeof(RectTransform));
            var filterRect = filterBarGO.GetComponent<RectTransform>();
            filterRect.SetParent(panelParent, false);

            // 放在 selectParent 的 siblingIndex 之前，这样 layout 会把后面的 selectParent 下移
            int insertIndex = selectParent.GetSiblingIndex();
            filterBarGO.transform.SetSiblingIndex(insertIndex);

            filterRect.anchorMin = new Vector2(0, 1);
            filterRect.anchorMax = new Vector2(1, 1);
            filterRect.pivot = new Vector2(0.5f, 1);
            filterRect.sizeDelta = new Vector2(0, 55); // 单行高度
            filterRect.anchoredPosition = new Vector2(0, -8);

            // 使用 HorizontalLayoutGroup 管理分类与等级在一行的布局
            var horizontal = filterBarGO.AddComponent<HorizontalLayoutGroup>();
            horizontal.spacing = 8;
            horizontal.padding = new RectOffset(8, 8, 6, 6);
            horizontal.childForceExpandWidth = false;
            horizontal.childControlWidth = false;
            horizontal.childAlignment = TextAnchor.MiddleLeft;

            // optional: add ContentSizeFitter so parent size adapts if needed
            var csf = filterBarGO.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // -------- 创建分类按钮（多选） --------
            foreach (var cat in CategoryList)
            {
                var btn = CreateCategoryButton(cat);
                btn.transform.SetParent(filterBarGO.transform, false);

                string c = cat;
                var buttonComp = btn.GetComponent<Button>();
                buttonComp.onClick.AddListener(() => {
                    ToggleCategory(__instance, c, btn);
                });
            }

            // -------- 在分类与等级之间插入竖直分隔符（有 LayoutElement） --------
            var sepGO = new GameObject("CatLevelSeparator", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            sepGO.transform.SetParent(filterBarGO.transform, false);
            var sepRect = sepGO.GetComponent<RectTransform>();
            sepRect.sizeDelta = new Vector2(2, 34);
            var sepImg = sepGO.GetComponent<Image>();
            sepImg.color = new Color(1f, 1f, 1f, 0.35f);
            var sepLE = sepGO.GetComponent<LayoutElement>();
            sepLE.preferredWidth = 2;
            sepLE.preferredHeight = 34;
            sepLE.flexibleWidth = 0;

            // -------- 创建等级按钮（单选） --------
            for (int i = 0; i < LevelList.Length; i++)
            {
                var color = LevelColors[i];
                var levelName = LevelList[i];
                var btn = CreateLevelButton(color);
                btn.transform.SetParent(filterBarGO.transform, false);

                var buttonComp = btn.GetComponent<Button>();
                // capture local
                string lvl = levelName;
                buttonComp.onClick.AddListener(() => {
                    ToggleLevel(__instance, lvl, btn);
                });
            }

            // -------- 在 filterBar 之后插入水平分割线 --------
            var dividerGO = new GameObject("FilterDivider", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            dividerGO.transform.SetParent(panelParent, false);
            // place divider right after filterBarGO
            dividerGO.transform.SetSiblingIndex(filterBarGO.transform.GetSiblingIndex() + 1);

            var divRect = dividerGO.GetComponent<RectTransform>();
            divRect.anchorMin = new Vector2(0, 1);
            divRect.anchorMax = new Vector2(1, 1);
            divRect.pivot = new Vector2(0.5f, 1);
            // 使分割线稍微向下移动（更明显）
            divRect.sizeDelta = new Vector2(0, 2);
            divRect.anchoredPosition = new Vector2(0, -60);

            var divImg = dividerGO.GetComponent<Image>();
            divImg.color = new Color(1f, 1f, 1f, 0.18f);

            filterRect.SetSiblingIndex(panelParent.GetSiblingIndex());
            dividerGO.transform.SetSiblingIndex(panelParent.GetSiblingIndex() + 1);
        }
        catch (Exception ex)
        {
            Debug.LogError("[TowerFilterMod] 注入筛选 UI 失败: " + ex);
        }

        // -------- 在 selectTowerParent 前面添加透明占位 --------
        try
        {
            var contentParent = __instance.selectTowerParent;
            if (contentParent != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var spacer = new GameObject($"Spacer_{i}", typeof(RectTransform), typeof(LayoutElement), typeof(CanvasRenderer), typeof(Image));
                    var rt = spacer.GetComponent<RectTransform>();
                    rt.SetParent(contentParent, false);
                    rt.SetSiblingIndex(0); // 始终插到最前面

                    // 设置尺寸（比如和塔按钮一样高，或略小）
                    rt.sizeDelta = new Vector2(200, 70);

                    // LayoutElement 可调整灵活性
                    var le = spacer.GetComponent<LayoutElement>();
                    le.preferredHeight = 70;
                    le.flexibleHeight = 0;

                    // 透明度设为 0（完全不可见）
                    var img = spacer.GetComponent<Image>();
                    img.color = new Color(0, 0, 0, 0);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[TowerFilterMod] 创建透明占位失败: " + e);
        }
    }

    #endregion

    #region UI 创建与交互逻辑

    // 创建一个带文字的分类按钮（默认未选中）
    private static GameObject CreateCategoryButton(string label)
    {
        var go = new GameObject("CatBtn_" + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(EventTrigger));
        go.AddComponent<SoundButton>();
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(75, 48);

        var img = go.GetComponent<Image>();
        img.color = new Color(0.28f, 0.28f, 0.28f, 0.9f);

        // 文本
        var textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var txtRt = textGO.GetComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(0, 0);
        txtRt.anchorMax = new Vector2(1, 1);
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;
        var txt = textGO.GetComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.color = Color.white;
        txt.fontSize = 22;

        // Button visuals set up (Image is already there)
        var btn = go.GetComponent<Button>();
        btn.transition = Selectable.Transition.None;

        // -------- 添加鼠标悬停事件 --------
        var trigger = go.GetComponent<EventTrigger>();

        // 移入
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) =>
        {
            img.color = new Color(0.5f, 0.7f, 1f, 1f); // 浅蓝
        });
        trigger.triggers.Add(entryEnter);

        // 移出
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) =>
        {
            if (ActiveCategories.Contains(label))
                img.color = new Color(0.8f, 0.5f, 0.3f, 1f); // 选中橙色
            else
                img.color = new Color(0.28f, 0.28f, 0.28f, 0.9f); // 默认灰
        });
        trigger.triggers.Add(entryExit);

        return go;
    }

    // 创建一个圆形/方形的等级按钮（没有文字），alpha 表示选中状态（未选 0.4，选中 1）
    private static GameObject CreateLevelButton(Color baseColor)
    {
        var go = new GameObject("LevelBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(EventTrigger));
        go.AddComponent<SoundButton>();
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(40, 48);

        var img = go.GetComponent<Image>();
        // 使用半透明表示未选中
        img.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);

        // 如果你有圆形 sprite 可以设置 img.sprite，这里用简单色块
        var btn = go.GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        // 悬停效果（稍微亮一点）
        var trigger = go.GetComponent<EventTrigger>();
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) =>
        {
            var c = img.color;
            img.color = new Color(c.r, c.g, c.b, c.a + 0.2f);
        });
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) =>
        {
            // 判断此颜色对应的等级是否为当前选中等级
            string myLevel = null;
            for (int i = 0; i < LevelColors.Length; i++)
            {
                var c = LevelColors[i];
                if (Mathf.Abs(c.r - baseColor.r) < 0.02f &&
                    Mathf.Abs(c.g - baseColor.g) < 0.02f &&
                    Mathf.Abs(c.b - baseColor.b) < 0.02f)
                {
                    myLevel = LevelList[i];
                    break;
                }
            }

            // 如果该等级是当前选中等级 -> 保持亮，否则恢复暗
            if (ActiveLevel == myLevel)
                img.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
            else
                img.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);
        });
        trigger.triggers.Add(entryExit);

        return go;
    }

    // 分类按钮切换（多选）
    private static void ToggleCategory(PreparePanelController controller, string category, GameObject buttonGO)
    {
        try
        {
            var img = buttonGO.GetComponent<Image>();
            if (ActiveCategories.Contains(category))
            {
                ActiveCategories.Remove(category);
                // 取消高亮（恢复默认）
                img.color = new Color(0.28f, 0.28f, 0.28f, 0.9f);
            }
            else
            {
                ActiveCategories.Add(category);
                // 高亮显示（橙色高亮）
                img.color = new Color(0.8f, 0.5f, 0.3f, 1f);
            }

            ApplyFilterToPanel(controller);
        }
        catch (Exception ex)
        {
            Debug.LogError("[TowerFilterMod] ToggleCategory 异常: " + ex);
        }
    }

    // 等级按钮切换（互斥）
    private static void ToggleLevel(PreparePanelController controller, string level, GameObject pressedButton)
    {
        try
        {
            // 如果点的就是当前等级 -> 取消选择；否则设置为此等级
            if (ActiveLevel == level)
            {
                ActiveLevel = null;
            }
            else
            {
                ActiveLevel = level;
            }

            // 更新同一行所有等级按钮的 alpha（按 parent 遍历）
            var parent = pressedButton.transform.parent;
            if (parent != null)
            {
                foreach (Transform t in parent)
                {
                    var img = t.GetComponent<Image>();
                    if (img == null) continue;
                    // 找出对应颜色值（如果 ActiveLevel 不为空就把被选的按钮 alpha 1，其他 0.4；若 ActiveLevel 为空则全部 0.4）
                    if (ActiveLevel == null)
                    {
                        // 所有等级按钮未选中
                        Color c = img.color;
                        img.color = new Color(c.r, c.g, c.b, 0.4f);
                    }
                    else
                    {
                        // 先比较颜色是否与 level 匹配：我们无法直接从 img 识别 level 名称，所以用索引逻辑：
                        // 利用 LevelList 与 LevelColors 的相同顺序：把每个按钮的 base color 以 rgb 比较（近似）
                        bool isSelected = false;
                        for (int i = 0; i < LevelList.Length; i++)
                        {
                            var baseCol = LevelColors[i];
                            // 比较 rgb（允许小幅误差）
                            if (ApproximatelyColor(img.color, new Color(baseCol.r, baseCol.g, baseCol.b, img.color.a)))
                            {
                                if (LevelList[i] == ActiveLevel)
                                    isSelected = true;
                                break;
                            }
                        }
                        // 如果为选中则 alpha 1 否则 0.4
                        Color cc = img.color;
                        img.color = new Color(cc.r, cc.g, cc.b, isSelected ? 1f : 0.4f);
                    }
                }
            }

            // 特别：如果新选中的等级是 null（取消），则确保所有按钮的 alpha 恢复为未选中 0.4
            if (ActiveLevel == null && parent != null)
            {
                foreach (Transform t in parent)
                {
                    var img = t.GetComponent<Image>();
                    if (img == null) continue;
                    Color c = img.color;
                    img.color = new Color(c.r, c.g, c.b, 0.4f);
                }
            }

            ApplyFilterToPanel(controller);
        }
        catch (Exception ex)
        {
            Debug.LogError("[TowerFilterMod] ToggleLevel 异常: " + ex);
        }
    }

    // 简单颜色近似比较（用于识别等级按钮基色）
    private static bool ApproximatelyColor(Color a, Color b, float tol = 0.02f)
    {
        return Mathf.Abs(a.r - b.r) < tol && Mathf.Abs(a.g - b.g) < tol && Mathf.Abs(a.b - b.b) < tol;
    }

    #endregion

    #region 过滤应用逻辑

    // 读取 PreparePanelController.towerSelectButtons 私有字段并设置按钮的 active 状态
    private static void ApplyFilterToPanel(PreparePanelController controller)
    {
        try
        {
            if (controller == null) return;

            // 反射获取私有字段 towerSelectButtons
            var field = typeof(PreparePanelController).GetField("towerSelectButtons", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
            {
                Debug.LogError("[TowerFilterMod] 找不到字段 towerSelectButtons，可能游戏版本不匹配。");
                return;
            }

            var listObj = field.GetValue(controller) as System.Collections.IEnumerable;
            if (listObj == null) return;

            // 遍历列表（假设元素类型是 TowerSelectButton）
            foreach (var elem in listObj)
            {
                if (elem == null) continue;
                // TowerSelectButton 有 CurrentTower 属性
                var tsType = elem.GetType();
                var currentTowerProp = tsType.GetProperty("CurrentTower", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (currentTowerProp == null) continue;
                var towerObj = currentTowerProp.GetValue(elem);
                if (towerObj == null)
                {
                    // 没塔则显示
                    SetGameObjectActive(elem, true);
                    continue;
                }

                // 读取 tower.TowerData.towerName
                var towerType = towerObj.GetType();
                var towerDataField = towerType.GetProperty("TowerData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                object towerData = null;
                if (towerDataField != null)
                    towerData = towerDataField.GetValue(towerObj);
                else
                {
                    // 有些类里可能是字段
                    var tdField2 = towerType.GetField("TowerData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (tdField2 != null) towerData = tdField2.GetValue(towerObj);
                }

                string towerName = null;
                if (towerData != null)
                {
                    var tnProp = towerData.GetType().GetProperty("towerName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (tnProp != null)
                        towerName = tnProp.GetValue(towerData) as string;
                    else
                    {
                        var tnField = towerData.GetType().GetField("towerName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (tnField != null)
                            towerName = tnField.GetValue(towerData) as string;
                    }
                }

                if (string.IsNullOrEmpty(towerName))
                {
                    // 作为后备，尝试用 towerObj.name
                    var nameProp = towerObj.GetType().GetProperty("name");
                    if (nameProp != null) towerName = nameProp.GetValue(towerObj) as string;
                }

                // 获取该塔的分类信息（优先配置）
                var info = TowerInfos.FirstOrDefault(x => string.Equals(x.TowerName, towerName, StringComparison.OrdinalIgnoreCase));
                // 如果没有配置，尝试从名字推断（简单规则）
                if (info == null)
                {
                    info = new TowerCategoryInfo
                    {
                        TowerName = towerName ?? "Unknown",
                        Categories = new List<string> { "抽象" },
                        Level = "灰"
                    };
                }

                // 分类匹配：如果没有任何激活的分类，则视为匹配；否则至少有一个交集即可
                bool matchCategory = ActiveCategories.Count == 0 || ActiveCategories.All(req => info.Categories.Contains(req));

                // 等级匹配：如果没有选任何等级则通过；否则 info.Level 必须等于 ActiveLevel
                bool matchLevel = string.IsNullOrEmpty(ActiveLevel) || string.Equals(info.Level, ActiveLevel, StringComparison.OrdinalIgnoreCase);

                bool visible = matchCategory && matchLevel;
                SetGameObjectActive(elem, visible);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[TowerFilterMod] ApplyFilterToPanel 异常: " + ex);
        }
    }

    // 将反射得到的 element 的 GameObject 设置为 active
    // elem 可能是一个 MonoBehaviour 的实例或 Component
    private static void SetGameObjectActive(object elem, bool active)
    {
        if (elem == null) return;
        var elemType = elem.GetType();
        // 尝试获取 gameObject 属性或字段
        var goProp = elemType.GetProperty("gameObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (goProp != null)
        {
            var go = goProp.GetValue(elem) as GameObject;
            if (go != null)
            {
                if (go.activeSelf != active) go.SetActive(active);
                return;
            }
        }
        // 尝试获取 gameObject via transform
        var transformProp = elemType.GetProperty("transform", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (transformProp != null)
        {
            var tr = transformProp.GetValue(elem) as Transform;
            if (tr != null)
            {
                if (tr.gameObject.activeSelf != active) tr.gameObject.SetActive(active);
                return;
            }
        }
        // 退化：尝试 elem as Component
        if (elem is Component comp)
        {
            if (comp.gameObject.activeSelf != active) comp.gameObject.SetActive(active);
        }
    }
    #endregion
}
