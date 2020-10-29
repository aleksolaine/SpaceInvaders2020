using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Inputs.IPlayerActions
{

    public Rigidbody2D playerRB;
    public SpriteRenderer playerSprite;
    public GameObject basicAmmo;
    public Sprite[] states;
    public Inputs controls;
    public float horizontalForce;
    private GameObject equippedAmmo;
    public bool canControl;
    private bool invincible;
    private Coroutine movementProcess;
    private Coroutine shootProcess;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Inputs();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        equippedAmmo = basicAmmo;
        canControl = true;
    }

    private void Update()
    {
        if (canControl)
        {
            playerRB.AddForce(controls.Player.Move.ReadValue<Vector2>() * Time.deltaTime * horizontalForce);
            if (controls.Player.Fire.triggered)
            {
                if (shootProcess != null)
                {
                    StopCoroutine(shootProcess);
                }

                shootProcess = StartCoroutine(ShootProcess());
            }
        }
    }

    //private IEnumerator MovementProcess(InputAction.CallbackContext callbackContext)
    //{
    //    Vector2 delta = callbackContext.ReadValue<Vector2>();

    //    while (delta.sqrMagnitude > 0.01f)
    //    {
    //        Move(delta);

    //        yield return null;

    //        delta = callbackContext.ReadValue<Vector2>();
    //    }
    //}

    private IEnumerator ShootProcess()
    {
        float shootCooldown = 1000;
        while (controls.Player.Fire.ReadValue<float>() > 0 && canControl)
        {
            if (shootCooldown > 0.2f)
            {
                shootCooldown = 0;
                Instantiate(equippedAmmo, transform.position, Quaternion.identity);
            }
            yield return null;
            shootCooldown += Time.deltaTime;
        }
        Debug.Log("Exit");
    }

    //void Move(Vector2 dir)
    //{
    //    if (canControl)
    //    playerRB.AddForce(dir * Time.deltaTime * horizontalForce);
    //}

    public void TakeDamage()
    {
        if (!invincible)
        {
            StartCoroutine(Explode());
        }
    }
    private IEnumerator Explode()
    {
        invincible = true;
        canControl = false;
        GameManager.manager.tempData.hp--;
        playerSprite.sprite = states[1];
        yield return new WaitForSeconds(1f);
        playerSprite.sprite = states[0];
        
        GameManager.manager.levelManager.HPHolder.transform.GetChild(GameManager.manager.tempData.hp)
            .gameObject.SetActive(false);
        if (GameManager.manager.tempData.hp < 1)
        {
            GameManager.manager.levelManager.GameOver();
        }
        canControl = true;
        invincible = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //if (movementProcess != null)
        //{
        //    StopCoroutine(movementProcess);
        //}

        //movementProcess = StartCoroutine(MovementProcess(context));
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Tää on tällanen tyhmä tyhjä
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        //if (shootProcess != null)
        //{
        //    StopCoroutine(shootProcess);
        //}

        //shootProcess = StartCoroutine(ShootProcess(context));
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        GameManager.manager.levelManager.Pause();
    }
}