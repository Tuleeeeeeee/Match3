using UnityEngine;

[CreateAssetMenu(fileName = "Gem_", menuName = "ScriptableObjects/GemSO", order = 1)]
public class GemSO : ScriptableObject
{
    public string gemName;
    public Sprite gemSprite;
}
