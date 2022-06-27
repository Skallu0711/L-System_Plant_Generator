using UnityEngine;
using UnityEngine.Rendering;

public class MeshCombiner : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        var meshFilter = transform.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh {indexFormat = IndexFormat.UInt32};
        meshFilter.mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }

    /// <summary>
    /// Clears unused objects
    /// </summary>
    public void ClearUnusedObjects()
    {
        if (transform.childCount <= 0) return;
    
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

} 