using UnityEngine;
using UnityEngine.UI;

public class RectFollowMouse : MonoBehaviour
{
    public Canvas canvas;
    public Camera camera;
    Vector2 pos;
    public GameObject hold;
    public float offsetX;
    public float offsetY;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        offsetX = offsetX == 0 ? 0.2f : offsetX;
        offsetY = offsetY == 0 ? 0.15f : offsetY;
        //transform.position = new Vector3(canvas.transform.TransformPoint(pos).x + 0.15f + holdDuplicate.transform.localScale.x / 5, canvas.transform.TransformPoint(pos).y - offsetY, canvas.transform.TransformPoint(pos).z);
    }


    // Update is called once per frame
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, camera, out pos);
        print(pos);
        if (Control.Instance.inv.holdingItem)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(canvas.transform.TransformPoint(pos).x + offsetX + 0.15f, canvas.transform.TransformPoint(pos).y - offsetY, canvas.transform.TransformPoint(pos).z), Time.deltaTime * 100);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(canvas.transform.TransformPoint(pos).x + offsetX, canvas.transform.TransformPoint(pos).y - offsetY, canvas.transform.TransformPoint(pos).z), Time.deltaTime * 100);
        }
        //transform.position = Vector3.Lerp(transform.position, new Vector3(canvas.transform.TransformPoint(pos).x + offsetX, canvas.transform.TransformPoint(pos).y - offsetY, canvas.transform.TransformPoint(pos).z), Time.deltaTime * 100);
    }
}
