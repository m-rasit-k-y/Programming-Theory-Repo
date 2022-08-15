using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

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


    [Header("Shooting")]
    public KeyCode shootKey = KeyCode.Mouse0;
    private float _shotTime;
    private float _shotPower;
    private Animator _anim;
    private Vector3 _stockPos;
    private Vector3 _shotPos;

    [Header("Zoom Settings")]
    public bool enableZoom = true;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

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
        _stockPos = weapon.GetComponentInChildren<Weapon>().stockPos;
        _shotTime = weapon.GetComponentInChildren<Weapon>().shotMultipler;
        _shotPower = weapon.GetComponentInChildren<Weapon>().shotPower;
        _anim = weapon.GetComponentInChildren<Animator>();


        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }


        if (crosshair)
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


        ShowName();
        Shooting();

        Zoom(weapon.GetChild(0).transform);
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
    }

    private void WeaponChange(Transform obj)
    {
        _anim.applyRootMotion = true;

        _stockPos = obj.GetComponent<Weapon>().stockPos;
        _shotTime = obj.GetComponent<Weapon>().shotMultipler;
        _shotPower = obj.GetComponent<Weapon>().shotPower;
        _anim = obj.GetComponent<Animator>();
        _anim.applyRootMotion = false;
        var mainweapon = weapon.GetChild(0).transform;

        mainweapon.GetComponent<BoxCollider>().isTrigger = false;
        mainweapon.SetParent(obj.parent);
        mainweapon.SetPositionAndRotation(obj.position, obj.rotation);

        obj.GetComponent<BoxCollider>().isTrigger = true;
        obj.SetParent(weapon);
        obj.localRotation = Quaternion.identity;
        obj.localPosition = Vector3.zero;

        weapon.localPosition = obj.GetComponent<Weapon>().stockPos;
    }

    private void Zoom(Transform obj)
    {
        if (enableZoom)
        {
            var _zoomPos = obj.GetComponent<Weapon>().zoomPos;

            if (Input.GetKeyDown(zoomKey))
            {
                weapon.DOLocalMove(_zoomPos, zoomStepTime * Time.deltaTime);
                playerCamera.DOFieldOfView(zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if(Input.GetKeyUp(zoomKey))
            {

                weapon.DOLocalMove(_stockPos, zoomStepTime * Time.deltaTime);
                playerCamera.DOFieldOfView(fov, zoomStepTime * Time.deltaTime);
            }
        }
    }

    private void Shooting()
    {

        _anim.SetFloat("ShotMultipler", _shotTime);


        if (Input.GetKeyDown(shootKey))
        {
            _anim.SetBool("Shot", true);
        }
        else if (Input.GetKeyUp(shootKey))
        {
            _anim.SetBool("Shot", false);

        }
    }
}