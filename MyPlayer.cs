using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class MyPlayer : MonoBehaviourPun,IPunObservable
{
    [Space]
    public float MoveSpeed = 3f;
    public float smoothRotationTime = 0.12f;
    public float JumpForce;
    public bool enableMobileInputs = false;
    [Space]
    //sounds
    public AudioSource shootSound;
    public AudioSource runSound;
    [Space]
    //health
    public GameObject healthBar;
    public Image fillImage;
    public float playerHealth = 1f;
    public float damage = 1f;
    [Space]
    public GameObject chatSystem;
    public Text teamText;
    
    [Space]
    float currentVeclocity;
    float currentSpeed;
    float speedVelocity;
    [Space]
    private FixedJoystick joystick;
    private GameObject crossHairPrefab;
    public Transform rayOrigin;
    private Animator anim;
    private Transform cameraTransform;
    private Vector3 crossHairVel;
    public ParticleSystem muzzle;

    private bool fire;
    [HideInInspector]
    public bool isDead = false;
   
    private void Awake()
    {
        if (photonView.IsMine)
        { 
            joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            crossHairPrefab = Resources.Load("CrosshairCanvas") as GameObject;
            chatSystem.SetActive(true); 
        }
    }
    private void Start()
    {
        
        if (photonView.IsMine)
        {
            anim = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;
            GameObject.Find("FireBtn").GetComponent<FireBtnScript>().SetPlayer(this);
            GameObject.Find("JumpBtn").GetComponent<FixedButton>().SetPlayer(this);
            crossHairPrefab = Instantiate(crossHairPrefab);
            healthBar.SetActive(true);
            teamText.text = "Team : " + PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        }
        else {
            GetComponent<BetterJump>().enabled = false;
            teamText.enabled = false;
        }
    }
    void Update()
    {
       

        if (photonView.IsMine)
        {
            if(!isDead)
            LocalPlayerUpdate();
        }
       
    }
    void LocalPlayerUpdate()
    {
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            Jump();
        }
        Vector2 input = Vector2.zero;
        if (enableMobileInputs)
        {
            input = new Vector2(joystick.input.x, joystick.input.y);
        }
        else
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {

            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVeclocity, smoothRotationTime);

            if (!runSound.isPlaying)
                runSound.Play();
        }
        else
        {
            //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y, ref currentVeclocity, smoothRotationTime);
            runSound.Stop();
        }
        if (fire)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * rotation;
        }
        float tragetSpeed = MoveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, tragetSpeed, ref speedVelocity, 0.02f);

        if (inputDir.magnitude > 0)
        {
            anim.SetBool("running", true);
        }
        else if (inputDir.magnitude == 0)
        {
            anim.SetBool("running", false);
        }

        if (!fire)
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        
            PositionCrossHair();
        
    }
     /*   
    private void LateUpdate()
    {
       /* if (photonView.IsMine)
        {
            PositionCrossHair();
        }*/
    //}

    void PositionCrossHair()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        int layer_mask = LayerMask.GetMask("Default");
        if (Physics.Raycast(ray, out hit, 100f, layer_mask))
        {
            Vector3 start = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
           crossHairPrefab.transform.position = Vector3.SmoothDamp(crossHairPrefab.transform.position, ray.GetPoint(10), ref crossHairVel, 0.03f);
          // crossHairPrefab.transform.position = ray.GetPoint(10);
            crossHairPrefab.transform.LookAt(Camera.main.transform);
        }
    }
    public void Fire()
    { 
        fire = true;
       
        anim.SetBool("fire 0", true);
        RaycastHit hit;
       
        if (Physics.Raycast(rayOrigin.position, Camera.main.transform.forward, out hit, 25f))
        {
            PhotonView pv = hit.transform.GetComponent<PhotonView>();

            if (pv!= null && !hit.transform.GetComponent<PhotonView>().IsMine && hit.transform.tag == "Player")
            {
               
                hit.transform.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, damage);
                
            }
        }
        shootSound.loop = true;
       
        if(!shootSound.isPlaying)
        shootSound.Play();
        MuzzleFlash();
       // Debug.DrawRay(rayOrigin.position, Camera.main.transform.forward * 25f, Color.green);
    }
    public void FireUp()
    {
        fire = false;
        if(photonView.IsMine)
        anim.SetBool("fire 0", false);

        shootSound.loop = false;
        shootSound.Stop();

        if (muzzle == null)
            muzzle = rayOrigin.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
        muzzle.Stop();
    }
    public void MuzzleFlash()
    {
        
        if(muzzle == null)
            muzzle = rayOrigin.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
       if(!muzzle.isPlaying)
        muzzle.Play();
    }
    public void Jump()
    {
        anim.SetTrigger("jump");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
     //  rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void GetDamage(float amount)
    {
        playerHealth -= amount;

        if (playerHealth <= 0 && photonView.IsMine)
        {   
            
            Death();
        }
        if (photonView.IsMine)
        {
            fillImage.fillAmount = playerHealth;
        }
        
    }
    [PunRPC]
    public void HideShowPlayer(bool hide)
    {    
        transform.gameObject.SetActive(hide);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fire);
        }
        else {
            if ((bool)stream.ReceiveNext())
            {
                MuzzleFlash();
            }
            else {
                FireUp();
            }
        }
    }
    [PunRPC]
    public void HideplayerMesh()
    {
        isDead = true;
        transform.Find("Soldier").gameObject.SetActive(false);
    }
    // will be called from GetDamage[RPC]
    void Death()
    {
        if (isDead)
            return;
        cameraTransform.gameObject.SetActive(false);
      
        anim.SetTrigger("death");
        photonView.RPC("HideplayerMesh", RpcTarget.All);
        GameObject.Find("GameManager").GetComponent<GameManager>().Spectate();
    }
}
