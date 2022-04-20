using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField] GameObject whiteText;
    [SerializeField] GameObject greenText;
    [SerializeField] GameObject redText;
    [SerializeField] GameObject canvas;
    [SerializeField] float changeTextSpeed;
    static public int points;
    static public int binValue = 1000;
    static public int damageValue = -250;
    static public int dodgeValue = 25;

    static public void ResetPoints()
    {
        points = 0;
    }

    static public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int toAdd, string details)
    {
        points += toAdd;
        if (toAdd >= 0)
        {
            StartCoroutine(ChangeText(greenText, toAdd, details));
        }
        else
        {
            StartCoroutine(ChangeText(redText, toAdd, details));
        }
        whiteText.GetComponent<TextMeshProUGUI>().text = "Points: " + points;
    }

    IEnumerator ChangeText(GameObject target, int toAdd, string details)
    {
        GameObject newTarget = Instantiate(target);
        newTarget.SetActive(true);
        newTarget.transform.SetParent(canvas.transform);
        newTarget.GetComponent<RectTransform>().position = target.GetComponent<RectTransform>().position;
        newTarget.GetComponent<TextMeshProUGUI>().text = "+" + toAdd + " (" + details + ")";
        Color c = newTarget.GetComponent<TextMeshProUGUI>().color;
        for (float alpha = 1f; alpha >= 0; alpha -= Time.deltaTime)
        {
            newTarget.GetComponent<RectTransform>().position = newTarget.GetComponent<RectTransform>().position + Vector3.up * changeTextSpeed * Time.deltaTime;
            c.a = alpha;
            newTarget.GetComponent<TextMeshProUGUI>().color = c;
            yield return null;
        }
        Destroy(newTarget);
    }
}
