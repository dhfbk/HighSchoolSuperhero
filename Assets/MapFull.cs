using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFull : MonoBehaviour
{
    public GameObject dot;
    public GameObject player;
    float offsetx = 105f;
    float offsety = 205f;
    float worldMapWidth = 105f + 650f;
    float worldMapHeight = 205f + 210f;
    private void Update()
    {
        //-105, -200
        //650, 205
        Vector3 playerWorldPos = new Vector3(player.transform.position.x+offsetx, player.transform.position.z+offsety);
        Vector3 playerMapPos = new Vector3((playerWorldPos.x * 1680f) / worldMapWidth, (playerWorldPos.y * 885f) / worldMapHeight);
        dot.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerMapPos.x, playerMapPos.y, transform.localPosition.z);
    }
}
