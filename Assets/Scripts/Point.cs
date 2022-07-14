using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PointType { exp, heart }
public class Point : MonoBehaviour
{
    public PointType pointType;
    public Player player;
    float t = 0;
    Vector2 startPos;
    public Vector2 destPos;
    [SerializeField]
    public float speed;
    public float Speed { get => speed; set => speed = value; }
    // Start is called before the first frame update
    void Start()
    {
        startPos = (Vector2)transform.localPosition + new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        //destPos = new Vector2(-325, 325);
        StartCoroutine(Collect(player));
    }

    IEnumerator Collect(Player player)
    {
        while (t < 1.0F)
        {
            transform.localPosition = Vector2.Lerp(startPos, destPos, t);
            t+=Time.deltaTime*speed;
            yield return null;
        }
        if (pointType == PointType.exp)
            player.AddExp(1);
        else if (pointType == PointType.heart)
        {
            
            player.AddLikes(1);
            FindObjectOfType<Heart>().StartAnimation();
        }
        Destroy(this.gameObject);
    }
}
