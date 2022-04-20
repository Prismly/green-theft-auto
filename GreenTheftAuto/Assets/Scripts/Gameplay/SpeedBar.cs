using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject speedArrow;
    [SerializeField] private GameObject arrowPath;
    float playerSpeed = 0f;
    float playerMinSpeed = 0f;
    float playerSpeedRange = 0f;
    int segmentsDestroyed = 0;
    [SerializeField] private Sprite[] gaugeSprites;
    [SerializeField] [Tooltip("Must be 1 less than the number of speed bar path points")] private int gaugeHP = 5;

    private void Start()
    {
        playerMinSpeed = player.GetComponent<PlayerTruck>().GetMinSpeed();
        playerSpeedRange = player.GetComponent<PlayerTruck>().GetMaxSpeed() - player.GetComponent<PlayerTruck>().GetMinSpeed();
    }

    // Update is called once per frame
    private void Update()
    {
        playerSpeed = player.GetComponent<PlayerTruck>().GetSpeed();
        float segmentLength = playerSpeedRange / (gaugeHP);
        int pathPointIndex = (int)((playerSpeed - playerMinSpeed) / segmentLength);
        float pathPointOffset = ((playerSpeed - playerMinSpeed) / segmentLength) - pathPointIndex;

        if (pathPointIndex >= gaugeHP)
        {
            // Top speed; because there is no "above point" beyond the topmost point, we'll approximate it by moving really far along the second highest point's path
            pathPointIndex = gaugeHP - 1;
            pathPointOffset = 0.999f;
        }

        //Debug.Log("playerSpeed: " + playerSpeed);
        //Debug.Log("segmentLength: " + segmentLength);
        //Debug.Log("pathPointIndex: " + pathPointIndex);
        //Debug.Log("pathPointOffset: " + pathPointOffset);

        RectTransform basePoint = arrowPath.transform.GetChild(pathPointIndex).GetComponent<RectTransform>();
        RectTransform abovePoint = arrowPath.transform.GetChild(pathPointIndex + 1).GetComponent<RectTransform>();

        Vector2 diff = new Vector2(abovePoint.position.x - basePoint.position.x, abovePoint.position.y - basePoint.position.y);
        Vector2 diffAdjusted = (diff * pathPointOffset) + new Vector2(basePoint.position.x, basePoint.position.y);

        speedArrow.GetComponent<RectTransform>().position = new Vector3(diffAdjusted.x, diffAdjusted.y, 0);
    }

    public void UpdateSegmentsDestroyedByAmount(int change)
    {
        segmentsDestroyed += change;

        // Clamp segmentsDestroyed
        segmentsDestroyed = segmentsDestroyed >= gaugeHP ? gaugeHP - 1 : segmentsDestroyed;
        segmentsDestroyed = segmentsDestroyed < 0 ? 0 : segmentsDestroyed;

        GetComponent<Image>().sprite = gaugeSprites[segmentsDestroyed];
    }

    public int GetGaugeHP()
    {
        return gaugeHP;
    }

    public int GetSegmentsDestroyed()
    {
        return segmentsDestroyed;
    }
}
