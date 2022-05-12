using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody rb;
    Camera mainCam;
    public GameObject target;
    GameObject targetBody;

    public TextManager tM;

    int maxSpaceTime = 10;
    int spaceTime;
    bool passedArc = true;
    bool pouring = false;

    public Seedling mySeedling;

    int money = 0;

    Item currentlyHolding;

    bool stopClick = false;

    ParticleSystem pour;
    public ParticleSystem splash;

    public World myWorld;

	void Awake () {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        spaceTime = maxSpaceTime;
        pour = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/PourWater"), mainCam.transform);
        ParticleSystem.EmissionModule pEmiss = pour.emission;
        pEmiss.rateOverTime = 0;

        foreach(Transform t in target.transform)
        {
            targetBody = t.gameObject;
        }

        incrementMoney(20);
    }
	
	// Update is called once per frame
	void Update () {
        processMovement();
        processMouseMovement();
        processOtherInputs();

        if (stopClick)
        {
            stopClick = false;
        }
	}

    private void processMovement()
    {
        Vector3 xVel = new Vector3(0, 0, 0);
        Vector3 zVel = new Vector3(0, 0, 0);
        int speed = 20;
        int forwardSpeed = speed;
        int jumpSpeed = 5;

        if (Input.GetKeyDown(KeyCode.LeftShift) && spaceTime  == maxSpaceTime)
        {
            StartCoroutine(shiftFOV(70));
        }
        if (Input.GetKey(KeyCode.LeftShift) && spaceTime == maxSpaceTime)
        {
            forwardSpeed = 30;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartCoroutine(shiftFOV(60));
        }


        if (Input.GetKey(KeyCode.W))
        {
            xVel = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z) * forwardSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            zVel = mainCam.transform.right * -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            xVel = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z) * -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            zVel = mainCam.transform.right * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            xVel = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            zVel = new Vector3(0, 0, 0);
        }


        rb.MovePosition(transform.position + xVel + zVel);
        

        if (Input.GetKey(KeyCode.Space) && (rb.velocity.y < 0.5f && rb.velocity.y > -0.5f) && passedArc && spaceTime == maxSpaceTime)
        {
            rb.velocity = new Vector3(0, jumpSpeed, 0);
            spaceTime--;
            passedArc = false;
        }
        if (Input.GetKey(KeyCode.Space) && spaceTime != 0 && spaceTime != maxSpaceTime)
        {
            rb.velocity = new Vector3(0, jumpSpeed, 0);
            spaceTime--;
            passedArc = false;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            spaceTime = 0;
        }
        //print(rb.velocity.y);
        if (rb.velocity.y < 0.5f && rb.velocity.y > -0.5f)
        {
            if (!passedArc)
            {
                passedArc = true;
                rb.velocity = new Vector3(0, -0.5f, 0);
            }
            else
            {
                spaceTime = maxSpaceTime;
            }
        }
    }

    private void processMouseMovement()
    {
        float xVel = 0;
        float yVel = 0;
        float speed = 1;
        if (Input.GetAxis("Mouse X") < 0)
        {
            yVel = Input.GetAxis("Mouse X");
        }
        else if (Input.GetAxis("Mouse X") > 0)
        {
            yVel = Input.GetAxis("Mouse X");
        }

        if (Input.GetAxis("Mouse Y") < 0)
        {
            xVel = -Input.GetAxis("Mouse Y");
        }
        else if (Input.GetAxis("Mouse Y") > 0)
        {
            xVel = -Input.GetAxis("Mouse Y");
        }

        if(xVel * speed + mainCam.transform.rotation.eulerAngles.x > 80 && xVel * speed + mainCam.transform.rotation.eulerAngles.x < 100 || xVel * speed + mainCam.transform.rotation.eulerAngles.x > 250 && xVel * speed + mainCam.transform.rotation.eulerAngles.x < 270)
        {
            xVel = 0;
        }

        mainCam.transform.rotation = Quaternion.Euler(xVel*speed+mainCam.transform.rotation.eulerAngles.x,yVel * speed + mainCam.transform.rotation.eulerAngles.y, 0);
        target.transform.rotation = Quaternion.Euler(target.transform.rotation.eulerAngles.x, yVel * speed + target.transform.rotation.eulerAngles.y, 0);
        if(xVel + mainCam.transform.rotation.eulerAngles.x < 200)
            targetBody.transform.localPosition = new Vector3(targetBody.transform.localPosition.x, targetBody.transform.localPosition.y, 4-(xVel * speed + mainCam.transform.rotation.eulerAngles.x)/15f);
    }

    private void processOtherInputs()
    {
        if (Input.GetMouseButtonDown(2))
        {
            myWorld.spawnSeedling("Stalker");
        }
        if (Input.GetMouseButtonDown(0) && getCurrentItem() != null && mySeedling != null)
        {
            getCurrentItem().pickUpSeedling(mySeedling);
            getCurrentItem().highlight(false);
            mySeedling = null;
        }
        else if (Input.GetMouseButtonDown(0) && getCurrentItem() != null && !stopClick)
        {
            getCurrentItem().doFunction();
        }else if (Input.GetMouseButtonUp(0) && getCurrentItem() != null && !stopClick)
        {
            getCurrentItem().endFunction();
        }else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonDown(1) && getCurrentItem() != null)
        {
            getCurrentItem().readyToThrow();
        }
        else if (Input.GetMouseButtonUp(1) && getCurrentItem() != null)
        {
            getCurrentItem().endFunction();
            getCurrentItem().drop();
            holdItem(null);
        }

        if (Input.GetKeyDown(KeyCode.Plus))
        {
            incrementMoney(20);
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            myWorld.increaseDifficulty(1);
        }
    }


    private IEnumerator shiftFOV(int target)
    {
        int increment = 0;
        if(target > mainCam.fieldOfView)
        {
            increment = 1;
        }
        else
        {
            increment = -1;
        }
        while (mainCam.fieldOfView != target)
        {
            mainCam.fieldOfView += increment;
            yield return new WaitForFixedUpdate();
        }
    }

    private void pourWater()
    {
        ParticleSystem.EmissionModule pEmiss = pour.emission;
        ParticleSystem.EmissionModule sEmiss = splash.emission;
        if (pouring)
        {
            pEmiss.rateOverTime = 100;
            sEmiss.rateOverTime = 100;
        }
        else
        {
            pEmiss.rateOverTime = 0;
            sEmiss.rateOverTime = 0;
        }

    }

    public void pourWater(bool p)
    {
        pouring = p;
        pourWater();
    }

    public bool isSprinkling()
    {
        return pouring;
    }

    public void incrementMoney(int val)
    {
        money += val;
        tM.updateMoney(money);
    }

    public void incrementDifficulty(int d)
    {
        tM.updateDifficulty(d);
    }

    public void incrementHealth(int h)
    {
        tM.updateHealth(h);
    }

    public Item getCurrentItem()
    {
        return currentlyHolding;
    }

    public void holdItem(Item toHold)
    {
        currentlyHolding = toHold;
        if (toHold != null)
        {
            stopClick = true;
            toHold.setBeingHeld(true);
        }
    }

    public int getMoney()
    {
        return money;
    }

    public float getTimeSoFar()
    {
        return tM.getTimeSoFar();
    }
}
