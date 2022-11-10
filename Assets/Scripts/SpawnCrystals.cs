using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCrystals : MonoBehaviour
{
    public CrystalListSO clist;
    public int amount;

    public void Spawn(Player target, int amount)
    {
        StartCoroutine(SpawnOverFrames(target, amount));
    }
    public IEnumerator SpawnOverFrames(Player target, int amount)
    {
        int i = 0;
        while (i < amount)
        {
            GameObject crystal = Instantiate(clist.crystal);
            crystal.transform.position = transform.position;
            crystal.transform.localScale = Vector3.one;
            crystal.GetComponent<CollectCrystal>().Collect(target);
            crystal.GetComponent<CollectCrystal>().FromChest = true;
            i++;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
