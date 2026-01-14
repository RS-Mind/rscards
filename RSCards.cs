using BepInEx;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using HarmonyLib;
using Jotunn.Utils;
using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.dk.rounds.plugins.zerogpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rsmind.rounds.weaponsmanager", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rsmind.rounds.fancycardbar", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSCards : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSCards";
        private const string ModName = "RSCards";
        public const string Version = "1.5.2";
        public const string ModInitials = "RSC";
        private CardHolder cardHolder;
        public static RSCards instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            foreach (KeyValuePair<string, PluginInfo> pluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos)
            {
                if (pluginInfo.Value.Metadata.GUID == "com.rsmind.rounds.RSClasses")
                {
                    RSClasses = true;
                    break;
                }
            }

            instance = this;
            RSCards.assets = AssetUtils.LoadAssetBundleFromResources("rscardart", typeof(RSCards).Assembly);

            if (RSCards.assets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSCards asset bundle");
            }
            cardHolder = assets.LoadAsset<GameObject>("CardHolder").GetComponent<CardHolder>();
            cardHolder.RegisterCards();

            CustomCardCategories.instance.MakeCardsExclusive(CardHolder.cards["Hitscan"], CardHolder.cards["Mortar"]);

            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);
        }
        IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            // Runs at start of pick phase
            foreach (var player in PlayerManager.instance.players)
            {
                if (player.data.GetComponent<Holding>().holdable.GetComponent<Gun>().reflects >= 2 
                    && ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(cardHolder.BounceAbsorptionCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(cardHolder.BounceAbsorptionCategory);
                }
                else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(cardHolder.BounceAbsorptionCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(cardHolder.BounceAbsorptionCategory);
                }

                if (player.GetComponent<CharacterStatModifiers>().lifeSteal >= 0.5f
                    && ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(cardHolder.RepentanceCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(cardHolder.RepentanceCategory);
                }
                else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(cardHolder.RepentanceCategory))
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(cardHolder.RepentanceCategory);
                }
            }
            yield break;
        }

        internal static bool RSClasses = false;
        public static bool Debug = false;
        internal static AssetBundle assets;
    }
}