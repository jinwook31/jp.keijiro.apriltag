using UnityEngine;
using System.Linq;
using UI = UnityEngine.UI;
using Klak.TestTools;
using System.Collections.Generic;

sealed class DetectionTest : MonoBehaviour
{
    [SerializeField] ImageSource _source = null;
    [SerializeField] int _decimation = 4;
    [SerializeField] float _tagSize = 0.05f;
    [SerializeField] Material _tagMaterial = null;
    [SerializeField] GameObject[] _prefab = null;
    [SerializeField] UI.RawImage _webcamPreview = null;
    [SerializeField] UI.Text _debugText = null;
    [SerializeField] int[] idList = {2099, 2107, 2108, 2109, 2113};

    AprilTag.TagDetector _detector;
    TagDrawer _drawer;

    private List<int> tagLog;
    private List<bool> tagStatus;

    void Start()
    {
        var dims = _source.OutputResolution;
        _detector = new AprilTag.TagDetector(dims.x, dims.y, _decimation);
        _drawer = new TagDrawer(_tagMaterial, _prefab, idList);
        
        tagLog = new List<int>();
        tagStatus = new List<bool>();
    }

    void OnDestroy()
    {
        _detector.Dispose();
        _drawer.Dispose();
    }

    void LateUpdate()
    {
        _webcamPreview.texture = _source.Texture;

        // Source image acquisition
        var image = _source.Texture.AsSpan();
        if (image.IsEmpty) return;

        // AprilTag detection
        var fov = Camera.main.fieldOfView * Mathf.Deg2Rad;
        _detector.ProcessImage(image, fov, _tagSize);

        for(int i=0; i < tagStatus.Count; i++){
            tagStatus[i] = false;
        }

        // Detected tag visualization
        foreach (var tag in _detector.DetectedTags){
            // Add new found tag
            if(!tagLog.Contains(tag.ID)){
                Debug.Log(tag.ID);
                tagLog.Add(tag.ID);
                tagStatus.Add(true);
                _drawer.Draw(tag.ID, tag.Position, tag.Rotation, _tagSize);

            }else{
                tagStatus[tagLog.IndexOf(tag.ID)] = true;
                _drawer.Draw(tag.ID, tag.Position, tag.Rotation, _tagSize);
            }
        }

        // remove augmented object if tag is gone
        for(int i=0; i<tagStatus.Count; i++){
            if(!tagStatus[i])
                _drawer.unDraw(tagLog[i]);
        }  


        // Profile data output (with 30 frame interval)
        if (Time.frameCount % 30 == 0)
            _debugText.text = _detector.ProfileData.Aggregate
              ("Profile (usec)", (c, n) => $"{c}\n{n.name} : {n.time}");
    }
}
