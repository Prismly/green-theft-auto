using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public enum PathColor
    {
        WHITE, MAGENTA, PINK,
        CORAL, RED, MAROON,
        BROWN, ORANGE, YELLOW,
        BANANA, LIME, GREEN,
        CYAN, BLUE, PURPLE,
        TAN, GRAY, BLACK
    }
    public static Color[] rainbow = { new Color(.839f, .882f, .941f), new Color(.933f, .333f, .729f), new Color(.925f, .753f, .827f),
                                new Color(.925f, .459f, .471f), new Color(.780f, .063f, .063f), new Color(.420f, .169f, .235f),
                                new Color(.447f, .286f, .114f), new Color(.945f, .490f, .055f), new Color(.965f, .969f, .341f),
                                new Color(1.00f, .996f, .745f), new Color(.314f, .941f, .220f), new Color(.063f, .502f, .176f),
                                new Color(.224f, .886f, .867f), new Color(.075f, .184f, .824f), new Color(.420f, .188f, .737f),
                                new Color(.573f, .529f, .463f), new Color(.514f, .592f, .655f), new Color(.247f, .282f, .306f) };

    [SerializeField] private PathColor lineColor;

    public static Color GetTrackColorFromEnum(PathColor enumVal)
    {
        return rainbow[(int)enumVal];
    }

    public Color GetTrackColor()
    {
        return Track.GetTrackColorFromEnum(lineColor);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = rainbow[(int)lineColor];
        Vector3 startOfSeg;
        Vector3 endOfSeg;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            startOfSeg = transform.GetChild(i).position;
            endOfSeg = transform.GetChild(i + 1).position;
            Gizmos.DrawLine(startOfSeg, endOfSeg);
        }
    }
}
