using UnityEngine;
using System;
using Object = UnityEngine.Object;

sealed class TagDrawer : System.IDisposable{
    Mesh _mesh;
    Material _sharedMaterial;
    GameObject[] _prefab, augmentedObj;

    private bool[] isFirst;
    private int[] idList;

    public TagDrawer(Material material, GameObject[] prefab, int[] ids){
        _mesh = BuildMesh();
        _sharedMaterial = material;
        _prefab = prefab;
        augmentedObj = new GameObject[_prefab.Length];
        idList = ids;
        
        isFirst = new bool[idList.Length];
        for(int i=0; i< isFirst.Length; i++)
            isFirst[i] = true;
    }

    public void Dispose(){
        Object.Destroy(_mesh);
        _mesh = null;
        _sharedMaterial = null;
        _prefab = null;
    }

    // Augment different object depending on ID
    public void Draw(int id, Vector3 position, Quaternion rotation, float scale){
        int idx = Array.IndexOf(idList, id);
        
        if(isFirst[idx]){
            //Debug.Log(id);
            isFirst[idx] = false;
            augmentedObj[idx] = GameObject.Instantiate(_prefab[idx], position, rotation) as GameObject;
            if(idx == 0){
                augmentedObj[idx].transform.localScale = new Vector3(scale * 50, scale * 50, scale * 50);
            }else{
                augmentedObj[idx].transform.localScale = new Vector3(scale * 2, scale * 2, scale * 2);
            }

        }else{
            // rot, pos, scale
            augmentedObj[idx].transform.position = position;
            augmentedObj[idx].transform.rotation = rotation;
            if(idx == 0){
                augmentedObj[idx].transform.localScale = new Vector3(scale * 50, scale * 50, scale * 50);
            }else{
                augmentedObj[idx].transform.localScale = new Vector3(scale * 2, scale * 2, scale * 2);
            }
        }

        // Yellow Cube
        //var xform = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
        //Graphics.DrawMesh(_mesh, xform, _sharedMaterial, 0);
    }

    public void unDraw(int id){
        int idx = Array.IndexOf(idList, id);
        augmentedObj[idx].transform.localScale = new Vector3(0,0,0);
    }


    static Mesh BuildMesh(){
        var vtx = new Vector3 [] { new Vector3(-0.5f, -0.5f, 0),
                                   new Vector3(+0.5f, -0.5f, 0),
                                   new Vector3(+0.5f, +0.5f, 0),
                                   new Vector3(-0.5f, +0.5f, 0),
                                   new Vector3(-0.5f, -0.5f, -1),
                                   new Vector3(+0.5f, -0.5f, -1),
                                   new Vector3(+0.5f, +0.5f, -1),
                                   new Vector3(-0.5f, +0.5f, -1),
                                   new Vector3(-0.2f, 0, 0),
                                   new Vector3(+0.2f, 0, 0),
                                   new Vector3(0, -0.2f, 0),
                                   new Vector3(0, +0.2f, 0),
                                   new Vector3(0, 0, 0),
                                   new Vector3(0, 0, -1.5f) };

        var idx = new int [] { 0, 1, 1, 2, 2, 3, 3, 0,
                               4, 5, 5, 6, 6, 7, 7, 4,
                               0, 4, 1, 5, 2, 6, 3, 7,
                               8, 9, 10, 11, 12, 13 };

        var mesh = new Mesh();
        mesh.vertices = vtx;
        mesh.SetIndices(idx, MeshTopology.Lines, 0);

        return mesh;
    }
}
