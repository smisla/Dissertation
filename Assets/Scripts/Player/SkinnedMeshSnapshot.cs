
using UnityEngine;

public class SkinnedMeshSnapshot : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public void BakePose()
    {
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);
        GameObject bakedObject = new GameObject("BakedPlankPose");
        MeshFilter mf = bakedObject.AddComponent<MeshFilter>();
        mf.mesh = bakedMesh;
        MeshRenderer mr = bakedObject.AddComponent<MeshRenderer>();
        mr.material = skinnedMeshRenderer.material;
    }
}
