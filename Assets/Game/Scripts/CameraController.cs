using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] GameObject thirdPersonCamera;
    [SerializeField] GameObject aimCamera;
    [SerializeField] Transform neckBone;
    [SerializeField] Transform TargetPos;
    [SerializeField] PlayerMovement PlayerScript;
    bool cameraSide;
    float lerpSpeed = 5.0f;
    float currentCameraSide;
    float targetCameraSide;

    // Start is called before the first frame update
    void Start()
    {
        
        cameraSide = false;
        aimCamera.GetComponentInChildren<Cinemachine3rdPersonFollow>().CameraSide = 0.2f;
        PlayerScript = PlayerScript.GetComponent<PlayerMovement>();
        currentCameraSide = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchCamera();
        LerpCameraSide();
    }

    void SwitchCamera()
    {
        if (Input.GetKey(KeyCode.Joystick1Button6))
        {
            thirdPersonCamera.SetActive(false);
            aimCamera.SetActive(true);
            PlayerScript.IsAiming = true;
            CheckCameraSide();
            CheckTargetPosition();
        }
        else
        {
            thirdPersonCamera.SetActive(true);
            aimCamera.SetActive(false);
            PlayerScript.IsAiming = false;
        }
    }

    bool CheckCameraSide()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button11) && !cameraSide)
        {
            targetCameraSide = 0.8f;
            cameraSide = true;
            Debug.Log("cameraside True");
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button11) && cameraSide)
        {
            targetCameraSide = 0.2f;
            cameraSide = false;
            Debug.Log("cameraside False");
        }

        return cameraSide;
    }

    void LerpCameraSide()
    {
        currentCameraSide = Mathf.Lerp(currentCameraSide, targetCameraSide, Time.deltaTime * lerpSpeed);
        aimCamera.GetComponentInChildren<Cinemachine3rdPersonFollow>().CameraSide = currentCameraSide;
    }

    void CheckTargetPosition()
    {
        if (PlayerScript.isStanded)
        {
            TargetPos.position = new Vector3(neckBone.position.x, neckBone.position.y/*1.563f*/, TargetPos.position.z);
            Debug.Log("CAM1");
        }

        if (PlayerScript.isCrouched)
        {
            TargetPos.position = new Vector3(TargetPos.position.x, neckBone.position.y /*+ -0.05f*/, TargetPos.position.z);
            Debug.Log("CAM2");

        }

        if (PlayerScript.isProned)
        {
            TargetPos.position = new Vector3(TargetPos.position.x, neckBone.position.y /*- 1.0f*/, TargetPos.position.z);
        }
    }
}