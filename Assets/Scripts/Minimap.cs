using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    Vector3 mapCenter = new Vector3(270, 0, 5);
    Vector2 playerPos;
    public bool inverted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Find point on minimap
        playerPos = new Vector2(transform.root.GetComponent<CameraInterface>().player.transform.position.x - mapCenter.x,
            transform.root.GetComponent<CameraInterface>().player.transform.position.z-mapCenter.z);

        //Move minimap
        if (inverted)
        GetComponent<RectTransform>().localPosition = new Vector2(playerPos.y, -playerPos.x);
        else
            GetComponent<RectTransform>().localPosition = new Vector2(-playerPos.x, -playerPos.y);

    }
}
