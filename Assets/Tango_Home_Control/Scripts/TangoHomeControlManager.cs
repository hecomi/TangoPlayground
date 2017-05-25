using System.Collections;
using UnityEngine;
using Tango;

public class TangoHomeControlManager : MonoBehaviour, ITangoDepth, ITangoLifecycle /*, ITangoPose*/
{
    [SerializeField]
    TangoApplication application;

    [SerializeField]
    TangoPoseController poseController;

    [SerializeField]
    TangoPointCloud pointCloud;

    [SerializeField]
    GameObject focusPrefab;

    [SerializeField]
    float placeNormalOffset;

    bool isWaitingForDepth_ = false;
    FocusObject selectedFocus_ = null;

    void Start()
    {
        application.Register(this);
        application.RequestPermissions();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            OnTap(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
#else
        if (Input.touchCount == 1) {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                OnTap(t.position);
            }
        }
#endif
    }

    void OnTap(Vector2 touchPos)
    {
        if (selectedFocus_ && selectedFocus_.selected) return;

        var camera = Camera.main;

        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(touchPos), out hit)) {
            var focus = hit.collider.GetComponent<FocusObject>();
            if (focus) {
                selectedFocus_ = focus;
                focus.Select();
            }
        } else {
            StartCoroutine(_WaitForDepthAndFindPlane(touchPos));
        }
    }

    public void OnTangoPermissions(bool granted)
    {
        if (!granted) return;

        var list = AreaDescription.GetList();
        if (list.Length == 0) {
            Debug.LogError("No area description available.");
            return;
        }

        var latestAreaDescription = list[0];
        var latestMetaData = latestAreaDescription.GetMetadata();
        foreach (var areaDescription in list) {
            var metaData = areaDescription.GetMetadata();
            if (metaData.m_dateTime > latestMetaData.m_dateTime) {
                latestAreaDescription = areaDescription;
                latestMetaData = metaData;
            }
        }

        application.Startup(latestAreaDescription);
    }

    public void OnTangoServiceConnected()
    {
    }

    public void OnTangoServiceDisconnected()
    {
    }

    public void OnTangoDepthAvailable(TangoUnityDepth tangoDepth)
    {
        isWaitingForDepth_ = false;
    }

    private IEnumerator _WaitForDepthAndFindPlane(Vector2 touchPosition)
    {
        isWaitingForDepth_ = true;

        application.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.MAXIMUM);
        while (isWaitingForDepth_) {
            yield return null;
        }
        application.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.DISABLED);

        var camera = Camera.main;
        Vector3 planeCenter;
        Plane plane;
        if (!pointCloud.FindPlane(camera, touchPosition, out planeCenter, out plane)) {
            yield break;
        }

        Vector3 up = plane.normal;
        Vector3 right = Vector3.Cross(up, camera.transform.forward);
        Vector3 forward = Vector3.Cross(right, up);
        var obj = Instantiate(focusPrefab, planeCenter + plane.normal * placeNormalOffset, Quaternion.LookRotation(forward, up));
        /*
        var focus = obj.GetComponent<FocusObject>();
        if (focus) {
            TangoCoordinateFramePair pair;
            pair.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
            pair.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;

            var pose = new TangoPoseData();
            PoseProvider.GetPoseAtTime(pose, poseController.LastPoseTimestamp, pair);

            var t = pose.translation;
            var q = pose.orientation;
            var devicePos = new Vector3((float)t.x, (float)t.y, (float)t.z);
            var deviceRot = new Quaternion((float)q.x, (float)q.y, (float)q.z, (float)q.w);
            var deviceTRS = Matrix4x4.TRS(devicePos, deviceRot, Vector3.one);

            var focusTRS = Matrix4x4.TRS(obj.transform.position, obj.transform.rotation, Vector3.one);
            focus.deviceLocalTRS = Matrix4x4.Inverse(deviceTRS) * focusTRS;
            focus.timestamp = (float)poseController.LastPoseTimestamp;
        }
        */
    }

    /*
    public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
    {
        if (poseData.framePair.baseFrame ==
                TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION &&
            poseData.framePair.targetFrame ==
                TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE &&
            poseData.status_code == 
                TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID)
        {
            foreach (var focus in FocusObject.instances) {
                TangoCoordinateFramePair pair;
                pair.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
                pair.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;

                var pose = new TangoPoseData();
                PoseProvider.GetPoseAtTime(pose, poseController.LastPoseTimestamp, pair);

                var t = pose.translation;
                var q = pose.orientation;
                var devicePos = new Vector3((float)t.x, (float)t.y, (float)t.z);
                var deviceRot = new Quaternion((float)q.x, (float)q.y, (float)q.z, (float)q.w);
                var deviceTRS = Matrix4x4.TRS(devicePos, deviceRot, Vector3.one);
                var focusTRS = deviceTRS * focus.deviceLocalTRS;

                focus.transform.position = focusTRS.GetColumn(3);
                focus.transform.rotation = Quaternion.LookRotation(focusTRS.GetColumn(2), focusTRS.GetColumn(1));
            }

            //Debug.Log("HOGE: " + Time.realtimeSinceStartup);
            //Debug.Log("HOGE: " + poseController.LastPoseTimestamp);
        }
    }
    */
}