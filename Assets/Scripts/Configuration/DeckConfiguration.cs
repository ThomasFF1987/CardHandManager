using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckConfig", menuName = "TCG/Deck Configuration")]
public class DeckConfiguration : ScriptableObject
{
    public List<CardConfiguration> startingCards;
    public int maxCards = 60;
}
