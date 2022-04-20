using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreDisplay : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "Points Earned: " + PointSystem.GetPoints();
    }
}
