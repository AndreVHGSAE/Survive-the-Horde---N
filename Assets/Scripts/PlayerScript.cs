using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerScript : MonoBehaviour
{
    public InputAction inputmovement;
    public InputAction rotateMovement;

    public float speed;
    public float gameTime;

    public float currentTimeS;
    public float maxTimeS;
    private float originalMaxTimeS;

    private bool BigActivated;

	public float currentTimeB=0;
    public float maxTimeB=1;

    public float currentTimeI = 0;
    public float maxTimeI = 1;

    public GameObject bullet;
    public List<GameObject> bulletPool = new List<GameObject>();

    public GameObject bigBullet;
    public List<GameObject> bigBulletPool = new List<GameObject>();

    public Rigidbody2D rb2D;

    UpdateUI uiScript;
    Coroutine revertOriginalFireRateRoutine;
    Coroutine revertOriginalShotsRoutine;

    public int lifes=3;
    bool isDamage = false;

    [SerializeField]
    private AudioSource ShootAudio;
    [SerializeField]
    private AudioClip NormalShoot, BigCharge, BigShotAudio;

    private void OnEnable()
    {
        inputmovement.Enable();
        rotateMovement.Enable();
    }

    private void OnDisable()
    {
        inputmovement.Disable();
        rotateMovement.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 movement = inputmovement.ReadValue<Vector2>();
        rb2D.linearVelocity = movement * 20;
        rb2D.linearVelocity = Vector2.ClampMagnitude(rb2D.linearVelocity, 10);

        Vector2 lookDir = rotateMovement.ReadValue<Vector2>();
        Vector2 Look = Camera.main.ScreenToWorldPoint(lookDir);
        Vector2 Direction = Look - rb2D.position;
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 90;
        rb2D.rotation = angle;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifes = 3;
        originalMaxTimeS = maxTimeS;

		uiScript = GameObject.Find("Canvas").GetComponent<UpdateUI>();
        uiScript.AddLifes(lifes);

        gameTime = 500;
        SetBulletPool(bullet, transform.position, bulletPool);
		SetBulletPool(bigBullet, transform.position, bigBulletPool);
	}

	// Update is called once per frame
	void Update()
    {
        Debug.Log($"<color=cyan>BigActivated is {BigActivated}</color>");

        currentTimeS += Time.deltaTime;
        if (currentTimeS >= maxTimeS)
        {
            if (BigActivated == false)
            {
				Debug.Log("UsingNormalBullet");
                GameObject temp = GetBullet();
                temp.SetActive(true);
                temp.transform.position = transform.position;
                //GameObject temp = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody2D rbtemp = temp.GetComponent<Rigidbody2D>();
                rbtemp.AddForce(transform.up * 50, ForceMode2D.Impulse);
                currentTimeS = 0;
                ShootAudio.clip = NormalShoot;
                ShootAudio.Play();
            }
            else
            {
                Debug.Log("UsingBigBullet");
				GameObject temp = GetBigBullet();
                temp.SetActive(true);
                temp.transform.position = transform.position;
                //GameObject temp = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody2D rbtemp = temp.GetComponent<Rigidbody2D>();
                rbtemp.AddForce(transform.up * 10, ForceMode2D.Impulse);
                currentTimeS = 0;
                ShootAudio.clip = BigShotAudio;
                ShootAudio.Play();
            }

        }

        if (lifes <= 0)
        {
            gameObject.SetActive(false);
            Time.timeScale = 0;
            uiScript.OpenGameOver();
        }

        if (transform.position.y > 5)
        {
            transform.localPosition += new Vector3(0, -0.5f, 0);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            maxTimeS = 500;
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (currentTimeB >= maxTimeB)
            {
                GameObject temp = GetBigBullet();
                temp.SetActive(true);
                temp.transform.position = transform.position;
                //GameObject temp = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody2D rbtemp = temp.GetComponent<Rigidbody2D>();
                rbtemp.AddForce(transform.up * 20, ForceMode2D.Impulse);
                ShootAudio.clip = BigShotAudio;
                ShootAudio.Play();
            }
            currentTimeB = 0;
            currentTimeS = 0;
            maxTimeS = 0.2f;
        }

        if(maxTimeS == 500)
        {
            currentTimeB += Time.deltaTime;

            ShootAudio.clip = BigCharge;
            ShootAudio.Play();
        }

        if(isDamage==true)
        {
            currentTimeI += Time.deltaTime;
            if(currentTimeI >= maxTimeI)
            {
                currentTimeI = 0;
                isDamage = false;
            }
        }

    }

    GameObject GetBullet()
    {
		Debug.Log("GettingNormalBullet");

		foreach (GameObject b in bulletPool)
        {
            if (b.activeInHierarchy == false)
            {
                return b;
            }
        }
        GameObject temp = Instantiate(bullet, transform.position, transform.rotation);
        temp.SetActive(false);
        bulletPool.Add(temp);
        return temp;
    }

    GameObject GetBigBullet()
    {
        foreach (GameObject b in bigBulletPool)
        {
            if (b.activeInHierarchy == false)
            {
                return b;
            }
        }
        GameObject temp = Instantiate(bullet, transform.position, transform.rotation);
        temp.SetActive(false);
        bulletPool.Add(temp);
        return temp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Enemy Bullet"))
        {
            collision.gameObject.SetActive(false);
            if (isDamage == false)
            {
                lifes--;
                uiScript.AddLifes(lifes);
                isDamage = true;
            }
        }
    }

    public void AddExtraLifes(int value)
    {
        lifes += value;
        uiScript.AddLifes(lifes);
    }

    public void IncreaseFireRate(float newMaxTimeS, float duration)
    {
        maxTimeS = newMaxTimeS;

		if(revertOriginalFireRateRoutine != null)
        {
            StopCoroutine(revertOriginalFireRateRoutine);
        }

		revertOriginalFireRateRoutine = StartCoroutine(RevertTOriginalFireRate(duration));
	}

    private void SetBulletPool(GameObject bulletPrefab, Vector3 startPosition, List<GameObject> _bulletPool)
    {
		for (int i = 0; i < 10; i++)
		{
			GameObject temp = Instantiate(bulletPrefab, startPosition, transform.rotation);
			temp.SetActive(false);
			_bulletPool.Add(temp);
		}
	}

	IEnumerator RevertTOriginalFireRate(float duration)
    {
        yield return new WaitForSeconds(duration);
        maxTimeS = originalMaxTimeS;
    }
    //Renombarar variables = (Hold)Crtl + r + r 
    public void BigShots(bool _BigActivated, float duration)
    {
		BigActivated = _BigActivated;


		if (revertOriginalShotsRoutine != null)
        {
            StopCoroutine(revertOriginalShotsRoutine);
        }

        revertOriginalShotsRoutine = StartCoroutine(RevertOriginalShots(duration));
    }

    IEnumerator RevertOriginalShots(float duration)
    {
        yield return new WaitForSeconds(duration);
        BigActivated = false;
    }
}
