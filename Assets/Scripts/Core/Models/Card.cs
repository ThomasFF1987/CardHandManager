using UnityEngine;

public class Card : ICard
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Sprite CardFrontImage { get; set; }
    public Sprite CardBackImage { get; set; }
}
