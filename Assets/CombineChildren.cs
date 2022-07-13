using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CombineChildren : MonoBehaviour
{
    public GameObject myRig;
    public Transform baseBone;
    public GameObject[] toMerge;
    public Material material;
    public Material colorMaterial;
    public bool colorized;
    public GameObject colorizeEffect;

    SkinnedMeshRenderer finalSMR;

    public bool combineSkinnedAtStart;
    public bool combineMeshesAtStart;
    private void Start()
    {
        if (combineSkinnedAtStart)
            Combine();
    }
    public void Combine()
    {
        //Create Rig
        GameObject rig = GameObject.Instantiate(myRig);

        //Find the "packed" renderer, which has bone info
        SkinnedMeshRenderer rigRenderer = rig.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] bones = rigRenderer.bones;
        Transform rootBone = baseBone;//rigRenderer.rootBone;

        //Then build your array of SkinnedMeshDatas. These are the meshes / materials that you want to merge:

        Mesh[] meshes = new Mesh[toMerge.Length];
        for (int i = 0; i < meshes.Length; i++)
            meshes[i] = toMerge[i].GetComponent<SkinnedMeshRenderer>().sharedMesh;


        //Pack! (In this case, by overwriting the already existing renderer)
        CombineFast(this, rigRenderer, rootBone, bones, meshes, material, toMerge, myRig);
    }
    public static void CombineFast(CombineChildren cc, SkinnedMeshRenderer skinnedMeshRenderer, Transform baseBone, Transform[] bones, Mesh[] meshes, Material material, GameObject[] toMerge, GameObject myRig)
    {
        if (meshes.Length == 0)
            return;

        CombineInstance[] combineInstances = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i] == null)
                continue;

            combineInstances[i] = new CombineInstance();
            combineInstances[i].transform = new Matrix4x4();// Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), new Vector3(100, 100, 100));
            combineInstances[i].mesh = meshes[i];
        }

        //Copy bind poses from first mesh in array
        Matrix4x4[] bindPoses = meshes[0].bindposes;

        Mesh combined_new_mesh = new Mesh();
        combined_new_mesh.CombineMeshes(combineInstances, true, false);
        combined_new_mesh.bindposes = bindPoses;

        //Note: Mesh.boneWeights returns a copy of bone weights (this is undocumented)
        BoneWeight[] newboneweights = combined_new_mesh.boneWeights;
        //Realign boneweights
        int offset = 0;
        for (int i = 0; i < meshes.Length; i++)
        {
            for (int k = 0; k < meshes[i].vertexCount; k++)
            {
                newboneweights[offset + k].boneIndex0 -= bones.Length * i;
                newboneweights[offset + k].boneIndex1 -= bones.Length * i;
                newboneweights[offset + k].boneIndex2 -= bones.Length * i;
                newboneweights[offset + k].boneIndex3 -= bones.Length * i;
            }

            offset += meshes[i].vertexCount;
        }

        combined_new_mesh.boneWeights = newboneweights;

        //combined_new_mesh.RecalculateBounds();

        skinnedMeshRenderer.sharedMesh = combined_new_mesh;
        skinnedMeshRenderer.sharedMaterial = material;
        skinnedMeshRenderer.bones = bones;
        skinnedMeshRenderer.rootBone = baseBone;

        foreach (GameObject obj in toMerge)
            obj.SetActive(false);

        cc.finalSMR = skinnedMeshRenderer;
        //myRig.SetActive(false);
    }

    public void Colorize()
    {
        //foreach (Transform t in transform)
        //{
        print("colorizing");
        //if (finalSMR != null)
        //{
            finalSMR.sharedMaterial = colorMaterial;
            colorized = true;
            print("smr " + finalSMR);
            print("colorized" + transform.name);
        GameObject cE = Instantiate(colorizeEffect);
        cE.transform.position = transform.position;
        cE.GetComponent<ParticleSystem>().Play();
                //smr.materials = new Material[smr.materials.Length];
                //foreach (Material m in smr.materials)
                //    m = colorMaterial;
        //}
            
        //}
    }

    //public void CombineMesh()
    //{
    //    GameObject parent = transform.GetChild(0).transform.gameObject;
    //    foreach (Transform child in transform)
    //        if (parent != child)
    //            child.SetParent(parent.transform);

    //    GameObject combineParent = parent;

    //    bool is32bit = true;
    //    // Verify there is existing object root, otherwise bail.
    //    if (combineParent == null)
    //    {
    //        Debug.LogError("Mesh Combine Wizard: Parent of objects to combne not assigned. Operation cancelled.");
    //        return;
    //    }

    //    // Remember the original position of the object. 
    //    // For the operation to work, the position must be temporarily set to (0,0,0).
    //    Vector3 originalPosition = combineParent.transform.position;
    //    combineParent.transform.position = Vector3.zero;

    //    // Locals
    //    Dictionary<Material, List<MeshFilter>> materialToMeshFilterList = new Dictionary<Material, List<MeshFilter>>();
    //    List<GameObject> combinedObjects = new List<GameObject>();

    //    MeshFilter[] meshFilters = combineParent.GetComponentsInChildren<MeshFilter>();

    //    // Go through all mesh filters and establish the mapping between the materials and all mesh filters using it.
    //    foreach (var meshFilter in meshFilters)
    //    {
    //        var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
    //        if (meshRenderer == null)
    //        {
    //            Debug.LogWarning("The Mesh Filter on object " + meshFilter.name + " has no Mesh Renderer component attached. Skipping.");
    //            continue;
    //        }

    //        var materials = meshRenderer.sharedMaterials;
    //        if (materials == null)
    //        {
    //            Debug.LogWarning("The Mesh Renderer on object " + meshFilter.name + " has no material assigned. Skipping.");
    //            continue;
    //        }

    //        // If there are multiple materials on a single mesh, cancel.
    //        if (materials.Length > 1)
    //        {
    //            // Rollback: return the object to original position
    //            combineParent.transform.position = originalPosition;
    //            Debug.LogError("Objects with multiple materials on the same mesh are not supported. Create multiple meshes from this object's sub-meshes in an external 3D tool and assign separate materials to each. Operation cancelled.");
    //            return;
    //        }
    //        var material = materials[0];

    //        // Add material to mesh filter mapping to dictionary
    //        if (materialToMeshFilterList.ContainsKey(material)) materialToMeshFilterList[material].Add(meshFilter);
    //        else materialToMeshFilterList.Add(material, new List<MeshFilter>() { meshFilter });
    //    }

    //    // For each material, create a new merged object, in the scene and in the assets folder.
    //    foreach (var entry in materialToMeshFilterList)
    //    {
    //        List<MeshFilter> meshesWithSameMaterial = entry.Value;
    //        // Create a convenient material name
    //        string materialName = entry.Key.ToString().Split(' ')[0];

    //        CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
    //        for (int i = 0; i < meshesWithSameMaterial.Count; i++)
    //        {
    //            combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
    //            combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
    //        }

    //        // Create a new mesh using the combined properties
    //        var format = is32bit ? IndexFormat.UInt32 : IndexFormat.UInt16;
    //        Mesh combinedMesh = new Mesh { indexFormat = format };
    //        combinedMesh.CombineMeshes(combine);

    //        // Create asset
    //        materialName += "_" + combinedMesh.GetInstanceID();
    //        AssetDatabase.CreateAsset(combinedMesh, "Assets/CombinedMeshes_" + materialName + ".asset");

    //        // Create game object
    //        string goName = (materialToMeshFilterList.Count > 1) ? "CombinedMeshes_" + materialName : "CombinedMeshes_" + combineParent.name;
    //        GameObject combinedObject = new GameObject(goName);
    //        var filter = combinedObject.AddComponent<MeshFilter>();
    //        filter.sharedMesh = combinedMesh;
    //        var renderer = combinedObject.AddComponent<MeshRenderer>();
    //        renderer.sharedMaterial = entry.Key;
    //        combinedObjects.Add(combinedObject);
    //    }

    //    // If there were more than one material, and thus multiple GOs created, parent them and work with result
    //    GameObject resultGO = null;
    //    if (combinedObjects.Count > 1)
    //    {
    //        resultGO = new GameObject("CombinedMeshes_" + combineParent.name);
    //        foreach (var combinedObject in combinedObjects) combinedObject.transform.parent = resultGO.transform;
    //    }
    //    else
    //    {
    //        resultGO = combinedObjects[0];
    //    }

    //    // Create prefab
    //    Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + resultGO.name + ".prefab");
    //    PrefabUtility.ReplacePrefab(resultGO, prefab, ReplacePrefabOptions.ConnectToPrefab);

    //    // Disable the original and return both to original positions
    //    combineParent.SetActive(false);
    //    combineParent.transform.position = originalPosition;
    //    resultGO.transform.position = originalPosition;
    //}
}

