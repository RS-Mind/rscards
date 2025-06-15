using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using RSCards.Cards;
using RSCards.Utilities;
using System.Collections.Generic;
using UnboundLib.Cards;
using UnityEngine;

namespace RSCards
{
    public class CardHolder : MonoBehaviour
    {
        public List<GameObject> Cards;
        public List<GameObject> HiddenCards;

        internal void RegisterCards()
        {
            foreach (var Card in Cards)
            {
                Card.AddComponent<Template>();
                if (RSCards.RSClasses && Card.name == "Twin Scythe")
                {
                    continue;
                }
                if (!DateTools.WeekOf(new System.DateTime(System.DateTime.UtcNow.Year, 4, 1)) && Card.name == "Repentence")
                {
                    continue;
                }
                if (DateTools.WeekOf(new System.DateTime(System.DateTime.UtcNow.Year, 4, 1)) && Card.name == "Repentance")
                {
                    continue;
                }
                CustomCard.RegisterUnityCard(Card, RSCards.ModInitials, Card.GetComponent<CardInfo>().cardName, true, null);
                CustomCardCategories.instance.UpdateAndPullCategoriesFromCard(Card.GetComponent<CardInfo>());
            }
            foreach (var Card in HiddenCards)
            {
                Card.AddComponent<Template>();
                if (!DateTools.WeekOf(new System.DateTime(System.DateTime.UtcNow.Year, 4, 1)) && Card.name == "Repen10ce")
                {
                    continue;
                }
                CustomCard.RegisterUnityCard(Card, RSCards.ModInitials, Card.GetComponent<CardInfo>().cardName, false, null);
                CustomCardCategories.instance.UpdateAndPullCategoriesFromCard(Card.GetComponent<CardInfo>());
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(Card.GetComponent<CardInfo>());
            }
        }
    }
}