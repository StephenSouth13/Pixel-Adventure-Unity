using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
public class CameraManager : MonoBehaviour
{
   
    public CinemachineCamera cineCam;
    public List<Transform> playerTransforms = new List<Transform>();

    public Transform cameraTarget; // empty object để làm trung tâm
    public static CameraManager Instance;
    void Awake()
    {
        if(Instance == null) Instance = this;    
        cineCam = GetComponent<CinemachineCamera>();
        cameraTarget = gameObject.transform; 
    }
    void Update()
    {
        if (playerTransforms == null || playerTransforms.Count == 0) return;

        if (playerTransforms.Count == 1)
        {
            // chỉ có 1 player
            cameraTarget.position = playerTransforms[0].position;
        }
        else
        {
            // tính trung bình vị trí tất cả player trong mảng
            Vector3 center = Vector3.zero;
            for (int i = 0; i < playerTransforms.Count; i++)
            {
                if (playerTransforms[i] != null)
                    center += playerTransforms[i].position;
            }
            center /= playerTransforms.Count;
            cameraTarget.position = center;
        }

        // gán target cho Cinemachine
        cineCam.Follow = cameraTarget;
    }
}
