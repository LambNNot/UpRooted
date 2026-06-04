using UnityEngine; 

[CreateAssetMenu(menuName = "Levels/Connection")] 

public class LevelConnection : ScriptableObject 
{
    public static LevelConnection ActiveConnection { get; set; } //ensures that this field will not be cleared when switching scenes
}