using UnityEngine;

public class Card : ICard
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Sprite CardImage { get; set; }
}
