using RarityLib.Utils;
using UnityEngine;

namespace RSCards.Utilities
{
    /// <summary>
    /// Intended of use in making unity cards only.
    /// </summary>
    internal class RarityAdder : MonoBehaviour
    {
        public enum Rarity
        {
            Trinket,
            Common,
            Scarce,
            Uncommon,
            Exotic,
            Rare,
            Epic,
            Legendary,
            Mythical,
            Divine,
            Unique
        }

        private string[] rarityNames =
        {
            "Trinket",
            "Common",
            "Scarce",
            "Uncommon",
            "Exotic",
            "Rare",
            "Epic",
            "Legendary",
            "Mythical",
            "Divine",
            "Unique"
        };

        public Rarity rarity = Rarity.Common;

        public void Start()
        {
            GetComponent<CardInfo>().rarity = RarityUtils.GetRarity(rarityNames[(int)rarity]);
        }
    }
}
