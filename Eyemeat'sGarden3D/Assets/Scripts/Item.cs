using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    Player mainPlayer;
    bool beingHeld = false;
    SpriteRenderer sr;
    bool currentHighlight = false;
    Coroutine clickMethod = null;
    Vector3 holdingPosition;
    Vector3 holdingAngle;
    Seedling mySeedling;
    int cooldown = 0;
    bool isSeed = false;
    string seedType = "none";
    ShopPedestal shop;

	// Use this for initialization
	void Awake () {
        GameObject p = GameObject.Find("Player");
        mainPlayer = p.GetComponent<Player>();
        sr = GetComponent<SpriteRenderer>();
        holdingPosition = new Vector3(1.5f, -0.5f, 2);
        holdingAngle = new Vector3(0,40,0);
    }
	
	// Update is called once per frame
	void Update () {
        if (beingHeld)
        {
            stayWithPlayer();
        }
        if (cooldown > 0)
            cooldown--;
	}

    public void setBeingHeld(bool held)
    {
        beingHeld = held;
        if(held)
            goToPlayer();
    }

    public void highlight(bool h)
    {
        if (currentHighlight == h)
            return;
        currentHighlight = h;
        if (h)
            sr.color = new Color(80 / 255f, 80 / 255f, 80 / 255f);
        else
        {
            sr.color = Color.white;
            if (isSeed)
            {
                setSeedType(seedType);
            }
        }
    }

    public void goToPlayer()
    {
        if (shop != null)
        {

            if (mainPlayer.getMoney() < shop.getCost())
            {
                mainPlayer.holdItem(null);
                setBeingHeld(false);
                shop.flashRed();
                return;
            }
            else
            {
                mainPlayer.incrementMoney(-shop.getCost());
                shop.spawnSeeds();
                shop = null;
            }
        }

        GameObject pivot = null;

        foreach(Transform t in mainPlayer.transform)
        {
            if(t.gameObject.name == "Main Camera")
            {
                pivot = t.gameObject;
            }
        }

        if(pivot != null)
        {
            transform.parent = pivot.transform;
            transform.localPosition = new Vector3(0, 0, 2);
        }

        holdingPosition = new Vector3(1.5f, -0.5f, 2);
    }

    public void stayWithPlayer()
    {
        if (clickMethod != null)
            return;
        transform.localPosition = holdingPosition;
        transform.localRotation = Quaternion.Euler(holdingAngle.x, holdingAngle.y, holdingAngle.z);
        if(mySeedling != null)
        {
            mySeedling.transform.localPosition = new Vector3(0,0,0);
            mySeedling.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void click()
    {
        transform.localPosition = holdingPosition;
        if (clickMethod == null)
        {
            clickMethod = StartCoroutine(doClick());
        }
        else
        {
            StopCoroutine(clickMethod);
            clickMethod = StartCoroutine(doClick());
        }
    }

    public void stopClick()
    {
        if(clickMethod != null)
        {
            StopCoroutine(clickMethod);
        }
        transform.localPosition = holdingPosition;
        clickMethod = null;
    }

    public void doFunction()
    {
        if (cooldown == 0)
        {
            if (name == "Wheelbarrow" && mySeedling != null)
            {
                cooldown = 2;
                Seedling temp = mySeedling;
                mySeedling = null;
                holdingPosition = new Vector3(1.5f, -0.5f, 2);
                temp.transform.localPosition = new Vector3(holdingPosition.x, 0.5f, holdingPosition.z);
                temp.transform.parent = null;
                temp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                mainPlayer.myWorld.moveObjectInFrontOfPlayer(temp.gameObject);
                temp.setSleeping(false);
            }
            if(name == "WateringCan")
            {
                mainPlayer.pourWater(true);
                holdingPosition = new Vector3(0, 0, 2);
                holdingAngle = new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y + 10, transform.localRotation.eulerAngles.z+40);
            }
        }
    }

    public void endFunction()
    {
        if (name == "WateringCan")
        {
            mainPlayer.pourWater(false);
            holdingPosition = new Vector3(1.5f, -0.5f, 2);
            holdingAngle = new Vector3(0, 40, 0);
        }
    }

    public void pickUpSeedling(Seedling s)
    {
        cooldown = 2;
        mySeedling = s;
        mySeedling.setSleeping(true);
        mySeedling.transform.SetParent(transform);
        mySeedling.transform.localPosition = new Vector3(0, 0, 0);
        mySeedling.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    private IEnumerator doClick()
    {
        float xSpeed = 0.01f;
        float ySpeed = 0.01f;
        transform.localPosition = holdingPosition;
        for (int i = 0; i < 20; i++)
        {
            transform.localPosition = new Vector3(holdingPosition.x- i * xSpeed, holdingPosition.y + i* ySpeed, holdingPosition.z);
            yield return new WaitForFixedUpdate();
        }
        for (int i = 20; i >= 0; i--)
        {
            transform.localPosition = new Vector3(holdingPosition.x - i * xSpeed, holdingPosition.y + i* ySpeed, holdingPosition.z);
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = holdingPosition;
        clickMethod = null;
    }

    public void drop()
    {
        stopClick();
        beingHeld = false;
        holdingPosition = new Vector3(0, 0, 2);
        transform.localPosition = holdingPosition;
        transform.parent = null;

        GetComponent<Rigidbody>().velocity = transform.forward * 10;
    }

    public void readyToThrow()
    {
        holdingPosition = new Vector3(0, 0, 2);
        holdingAngle = new Vector3(0, 0, 0);
    }

    public Seedling getCurrentSeedling()
    {
        return mySeedling;
    }

    public void setSeed(bool seed)
    {
        isSeed = seed;
    }

    public bool getSeed()
    {
        return isSeed;
    }

    public void setSeedType(string seed)
    {
        seedType = seed;
        if(seed != null && seed != "none")
        {
            setSeed(true);
            switch (seedType)
            {
                case "Seedling":
                    sr.color = Color.green;
                    break;
                case "PitcherPlant":
                    sr.color = Color.blue;
                    break;
                case "SeedSpitter":
                    sr.color = Color.cyan;
                    break;
                case "Stalker":
                    sr.color = Color.yellow;
                    break;
            }
        }
        else
        {
            setSeed(false);
        }
    }

    public void setShop(ShopPedestal s)
    {
        shop = s;
    }
    
    public ShopPedestal getShop()
    {
        return shop;
    }

    public string getSeedType()
    {
        return seedType;
    }
}
