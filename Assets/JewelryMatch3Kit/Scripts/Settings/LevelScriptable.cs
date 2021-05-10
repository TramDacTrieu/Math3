using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "Diamond splash/Add level", order = 1)]
public class LevelScriptable : ScriptableObject
{
    // public Target Target = Target.DIAMONDS;
    public int Collect = 3;
    public int Moves = 15;
    // public LIMIT LimitType = LIMIT.MOVES;
    [Range(3, 5)]
    public int ColorLimit = 3;
    public int[] Stars = new int[3] { 100, 500, 1000 };

    public int Tutorial;
}
