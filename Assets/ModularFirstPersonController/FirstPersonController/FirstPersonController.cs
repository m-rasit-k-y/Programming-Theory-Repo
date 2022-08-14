using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 weaponPos;

    [SerializeField] private float raydistance;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private Transform weapon;


    [Header("Camera Settings")]
    public Camera playerCamera;
    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    [Header("Crosshair Settings")]
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    [Header("Zoom Settings")]
    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    [Header("Movement Settings")]
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
    }

    void Start()
    {
        weaponPos = weapon.transform.position;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        if(cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        if (enableZoom)
        {

            if(Input.GetKeyDown(zoomKey) && !holdToZoom)
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }

            if(holdToZoom)
            {
                if(Input.GetKeyDown(zoomKey))
                {
                    isZoomed = true;
                    var ZoomPos = new Vector3(0, -1, 1.75f);

                    weapon.transform.localPosition = Vector3.Lerp(weapon.transform.localPosition, ZoomPos, zoomStepTime * Time.deltaTime);
                }
                else if(Input.GetKeyUp(zoomKey))
                {
                    isZoomed = false;

                    weapon.transform.position = Vector3.Lerp(weapon.position, weaponPos, zoomStepTime * Time.deltaTime);
                }
            }

            if(isZoomed)
            {

                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        ShowName();
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        #endregion
    }

    private void ShowName()
    {

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, raydistance))
        {
            if (hit.transform.gameObject.CompareTag("Weapon"))
            {
                weaponText.gameObject.SetActive(true);

                weaponText.text = hit.transform.name;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    WeaponChange(hit.transform);
                }
            }
        }
        else
        {
            weaponText.gameObject.SetActive(false);
        }

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * raydistance,Color.red);
    }

    private void WeaponChange(Transform obj)
    {
        var mainweapon = weapon.GetChild(0).transform;

        mainweapon.GetComponent<BoxCollider>().isTrigger = false;
        mainweapon.SetParent(obj.parent);
        mainweapon.SetPositionAndRotation(obj.position, obj.rotation);

        obj.GetComponent<BoxCollider>().isTrigger = true;
        obj.SetParent(weapon);
        obj.localRotation = Quaternion.identity;
        obj.localPosition = new Vector3(0, 0, 0.7f);
    }
}