using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuObject : MonoBehaviour
{
    public GameObject amountBG;
    public SpriteRenderer spriteR;
    public GameObject amountObj;
    public int amount;
    public bool follow;
    public Vector3 destSlot;
    public string name;

    public Canvas canvas;
    Vector2 pos;
    public Vector3 startScale;
    public bool destroy;
    float timeToReachTarget;
    float t;
    public Vector3 startPos;
    public float speed;

    public bool instantiated;
    // Start is called before the first frame update
    void Start()
    {
        if (!instantiated)
        {
            amount = int.Parse(amountObj.GetComponent<TextMeshPro>().text);
        }
        else
        {
            amountObj.GetComponent<TextMeshPro>().text = amount.ToString();
            transform.localScale = startScale;
            instantiated = false;
        }
        startPos = transform.position;
        spriteR = GetComponent<SpriteRenderer>();
        name = spriteR.sprite.name;
        timeToReachTarget = 1.0f;
        canvas = Control.Instance.inventory.transform.parent.GetComponent<Canvas>();
        speed = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);

            transform.position = Vector3.Lerp(transform.position, canvas.transform.TransformPoint(pos), Time.deltaTime * speed);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.8f, 1.8f, 1.8f), Time.deltaTime * speed);
            if (destroy)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(1, 1, 1, 0), Time.deltaTime * speed);
                amountObj.GetComponent<TextMeshPro>().color = Color.Lerp(amountObj.GetComponent<TextMeshPro>().color, new Color(1, 1, 1, 0), Time.deltaTime * speed);
                if (Vector3.Distance(transform.position, canvas.transform.TransformPoint(pos)) < 0.01f)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * speed);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * speed);
            if (destroy)
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(1, 1, 1, 0), Time.deltaTime * speed);
                amountObj.GetComponent<TextMeshPro>().color = Color.Lerp(amountObj.GetComponent<TextMeshPro>().color, new Color(1, 1, 1, 0), Time.deltaTime * speed);
                if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.01f)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
    public void SetAmount(int i)
    {
        amount = i;
        UpdateAmount();
    }

    public void ChangeAmount(int i)
    {
        amount += i;
        UpdateAmount();
    }
    public void UpdateAmount()
    {
        if (amount > 0)
        {
            amountObj.GetComponent<TextMeshPro>().text = amount.ToString();
        }
        else
        {
            amountObj.GetComponent<TextMeshPro>().text = "";
        }
    }
}
