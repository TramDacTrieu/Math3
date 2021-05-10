using System.Collections.Generic;

public static class ColorGenerator
{
    public static int GenColor(int maxColors = 6, int exceptColor = -1, bool onlyNONEType = false)
    {
        List<int> remainColors = new List<int>();
        var thisColorLimit = LevelSettings.THIS.LevelScriptable.ColorLimit - ((LevelManager.THIS?.gameStatus == GameState.RegenLevel) ? 1 : 0);
        for (int i = 0; i < thisColorLimit; i++)
        {
            if (i != exceptColor)
            {
                remainColors.Add(i);
            }
        }

        int randColor = UnityEngine.Random.Range(0, thisColorLimit);
        if (remainColors.Count > 0)
            randColor = remainColors[UnityEngine.Random.Range(0, remainColors.Count)];
        if (exceptColor == randColor)
            randColor = (randColor++) % maxColors;
        return randColor;
    }
}