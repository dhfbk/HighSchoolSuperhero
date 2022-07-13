using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUtility : MonoBehaviour
{
    public static void DisplayReward(Player agent, RewardInfo ri, float pitch = 1)
    {
        agent.cameraInterface.FX.pitch = pitch;
        agent.cameraInterface.FX.clip = ri.clip;
        agent.cameraInterface.FX.Play();
        agent.cameraInterface.FX.pitch = 1;

        GameObject r = Instantiate(Resources.Load<GameObject>("Reward"));
        r.GetComponent<Image>().sprite = ri.sprite;
        r.transform.parent = agent.cameraInterface.hudCanvas.transform;
        r.transform.localPosition = new Vector3(0,280,0);
        r.GetComponent<RectTransform>().sizeDelta = new Vector2(ri.sprite.rect.width, ri.sprite.rect.height);
        r.transform.localRotation = Quaternion.identity;
        r.transform.localScale = new Vector3(ri.scale, ri.scale, ri.scale);

        GameObject rays = Instantiate(Resources.Load<GameObject>("Rays"));
        foreach (Image img in rays.GetComponentsInChildren<Image>())
            img.color -= new Color(0,0, 0, 1f);
        rays.transform.parent = agent.cameraInterface.hudCanvas.transform;
        rays.transform.localPosition = new Vector3(0, 280, 0);
        rays.transform.localRotation = Quaternion.identity;
        rays.transform.localScale = (Vector3.one / 2)*ri.scale*3;

        r.GetComponent<Reward>().rays = rays;
    }
}
