using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "TCG/Card Configuration")]
public class CardConfiguration : ScriptableObject
{
    public string cardName;
    public Sprite frontSprite;
    public Sprite backSprite;
    public int cost;
    public string description;
}
