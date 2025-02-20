using BepInEx;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using HarmonyLib;
using RSCards.Cards;
using RSCards.Utilities;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using Jotunn.Utils;
using UnityEngine;
using System.Collections;
using UnboundLib;
using System.Linq;

namespace RSCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.dk.rounds.plugins.zerogpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RSCards : BaseUnityPlugin
    {
        private const string ModId = "com.rsmind.rounds.RSCards";
        private const string ModName = "RSCards";
        public const string Version = "1.3.6";
        public const string ModInitials = "RSC";
        public static RSCards instance { get; private set; }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            foreach (BaseUnityPlugin plugin in BepInEx.Bootstrap.Chainloader.Plugins)
            {
                if (plugin.Info.Metadata.GUID == "com.rsmind.rounds.RSClasses")
                {
                    RSClasses = true;
                    break;
                }
            }

            instance = this;

            RSCards.assets = AssetUtils.LoadAssetBundleFromResources("rscardart", typeof(RSCards).Assembly);
            assets.LoadAsset<GameObject>("CardHolder").GetComponent<CardHolder>().RegisterCards();

            if (RSCards.assets == null)
            {
                UnityEngine.Debug.Log("Failed to load RSCards asset bundle");
            }

            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);
        }
        IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            // Runs at start of pick phase
            foreach (var player in PlayerManager.instance.players)
            {

                if (player.data.GetComponent<Holding>().holdable.GetComponent<Gun>().reflects >= 2)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.BounceAbsorptionCategory);
                }
                else
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(RSCardCategories.BounceAbsorptionCategory);
                }

                if (player.GetComponent<CharacterStatModifiers>().lifeSteal >= 0.5f)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Remove(RSCardCategories.RepentanceCategory);
                }
                else
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(RSCardCategories.RepentanceCategory);
                }
            }
            yield break;
        }

        internal static bool RSClasses = false;
        public static bool Debug = false;
        internal static AssetBundle assets;
    }

    static class RSCardCategories
    {
        public static CardCategory BounceAbsorptionCategory = CustomCardCategories.instance.CardCategory("BounceAbsorptionCategory");
        public static CardCategory RepentanceCategory = CustomCardCategories.instance.CardCategory("RepentanceCategory");
    }
}