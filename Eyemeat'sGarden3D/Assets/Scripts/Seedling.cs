using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seedling : MonoBehaviour {
    private World myWorld;
    private Rigidbody rb;
    private Daemon target = null;
    private GameObject myOrigin;
    private bool retreating = false;
    private int speed = 10;
    private int radius = 5;
    private int waterLevel = 150;
    private int maxWaterLevel = 150;

    public SpriteRenderer sprout;
    public GameObject rightLeg;
    public GameObject leftLeg;

    private Coroutine runLegs;

    private ParticleSystem waterIndicator;
    private ParticleSystem waterRadius;
    private ParticleSystem range;
    private int waterTick = 0;
    private bool warning = false;

    private bool sleeping = false;

    public string type;

    private ParticleSystem pitcherWater;
    private ParticleSystem.EmissionModule pitcherEmitter;
    private Seedling pitcherTarget;

    private int laserCounter = 0;
    private List<ParticleSystem> laser = new List<ParticleSystem>();

    // Use this for initialization
    void Awake() {
        rb = GetComponent<Rigidbody>();
        if (type == "Seedling")
            sprout.color = Color.green;
        else if (type == "PitcherPlant")
        {
            waterLevel = 300;
            maxWaterLevel = 300;
            radius = 10;
            speed = 12;
            sprout.color = Color.blue;
            foreach(Transform c in transform)
            {
                if (c.GetComponent<ParticleSystem>() != null)
                    pitcherWater = c.GetComponent<ParticleSystem>();
            }
            if(pitcherWater != null)
            {
                pitcherEmitter = pitcherWater.emission;
                pitcherEmitter.rateOverTime = 0;
            }
        }
        else if (type == "SeedSpitter")
        {
            waterLevel = 100;
            maxWaterLevel = 100;
            radius = 20;
            speed = 2;
            sprout.color = Color.cyan;

            foreach (Transform c in sprout.transform)
            {
                if (c.GetComponent<SpriteRenderer>() != null)
                    c.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
        else if (type == "Stalker")
        {
            waterLevel = 300;
            maxWaterLevel = 300;
            radius = 8;
            speed = 16;
            sprout.color = Color.green;

            foreach (Transform c in sprout.transform)
            {
                if (c.GetComponent<SpriteRenderer>() != null)
                    c.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (sleeping)
        {
            return;
        }

        runAI();

        manageWater();
    }

    private void runAI()
    {
        if(type == "Seedling")
        {
            if (target != null && !checkTargetInRadius())
            {
                retreating = true;
            }
            if (retreating)
            {
                target = null;
                moveToOrigin();
            }
            else if (target == null)
            {
                target = checkNearby();
                if (target == null)
                {
                    retreating = true;
                }
            }

            if (target != null)
            {
                moveTowardsTarget();
            }
            if (target == null && !retreating)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                setRunning(false);
            }
        }
        else if(type == "PitcherPlant")
        {
            if (pitcherTarget != null && !checkAllyInRadius())
            {
                retreating = true;
            }
            if (retreating)
            {
                pitcherTarget = null;
                moveToOrigin();
            }
            else if (pitcherTarget == null)
            {
                pitcherTarget = checkAllyNearby();
                if (pitcherTarget == null)
                {
                    retreating = true;
                }
            }

            if (pitcherTarget != null)
            {
                moveTowardsAlly();
            }
            if (pitcherTarget == null && !retreating)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                setRunning(false);
            }
        }else if(type == "SeedSpitter")
        {
            if (target != null && !checkTargetInRadius())
            {
                retreating = true;
            }
            if (retreating)
            {
                target = null;
                moveToOrigin();
            }
            else if (target == null)
            {
                target = checkNearbyRanged();
                if (target == null)
                {
                    retreating = true;
                }
            }

            if (target != null && laserCounter <= 0)
            {
                shootAtTarget();
            }
            if (target == null && !retreating)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                setRunning(false);
            }
            laserCounter--;
        }else if (type == "Stalker")
        {
            if (target != null && !checkTargetInRadius())
            {
                retreating = true;
            }
            if (retreating)
            {
                target = null;
                moveToOrigin(3);
            }
            else if (target == null)
            {
                target = checkNearby();
                if (target == null)
                {
                    retreating = true;
                }
            }

            if (target != null)
            {
                moveTowardsTarget();
            }
            if (target == null && !retreating)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                setRunning(false);
            }
        }
    }

    private void manageWater()
    {
        waterTick++;
        if (waterTick >= 20)
        {
            waterLevel--;
            if (waterLevel <= 0)
            {
                die();
                return;
            }
            updateWaterLevel();
            waterTick = 0;
        }
    }

    private Daemon checkNearby()
    {
        foreach (Daemon d in myWorld.getDaemons())
        {
            if (d.transform.position.x <= myOrigin.transform.position.x + radius && d.transform.position.x >= myOrigin.transform.position.x - radius)
            {
                if (d.transform.position.z <= myOrigin.transform.position.z + radius && d.transform.position.z >= myOrigin.transform.position.z - radius)
                {
                    if (!d.getMarked() && d.getType() != "Spirit")
                    {
                        d.setMarked(true);
                        Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/TargetFound"), transform);
                        return d;
                    }
                }
            }
        }
        return null;
    }

    private Seedling checkAllyNearby()
    {
        int lowest = 1000;
        Seedling lowestTarget = null;
        foreach (Seedling d in myWorld.getSeedlings())
        {
            if (d.transform.position.x <= myOrigin.transform.position.x + radius && d.transform.position.x >= myOrigin.transform.position.x - radius)
            {
                if (d.transform.position.z <= myOrigin.transform.position.z + radius && d.transform.position.z >= myOrigin.transform.position.z - radius)
                {
                    if (d.getWaterLevel() <= d.getMaxWaterLevel() - 20 && d.getWaterLevel() < lowest && d != this)
                    {
                        lowest = d.getWaterLevel();
                        lowestTarget = d;
                    }
                }
            }
        }
        if(lowestTarget != null)
        {
            Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/TargetFound"), transform);
        }
        return lowestTarget;
    }

    private Daemon checkNearbyRanged()
    {
        foreach (Daemon d in myWorld.getFlying())
        {
            if (d.transform.position.x <= myOrigin.transform.position.x + radius && d.transform.position.x >= myOrigin.transform.position.x - radius)
            {
                if (d.transform.position.z <= myOrigin.transform.position.z + radius && d.transform.position.z >= myOrigin.transform.position.z - radius)
                {
                    Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/TargetFound"), transform);
                    return d;
                }
            }
        }
        foreach (Daemon d in myWorld.getDaemons())
        {
            if (d.transform.position.x <= myOrigin.transform.position.x + radius && d.transform.position.x >= myOrigin.transform.position.x - radius)
            {
                if (d.transform.position.z <= myOrigin.transform.position.z + radius && d.transform.position.z >= myOrigin.transform.position.z - radius)
                {
                    Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/TargetFound"), transform);
                    return d;
                }
            }
        }
        return null;
    }

    private bool checkTargetInRadius()
    {
        if (target == null)
            return false;
        if (target.transform.position.x <= myOrigin.transform.position.x + radius && target.transform.position.x >= myOrigin.transform.position.x - radius)
        {
            if (target.transform.position.z <= myOrigin.transform.position.z + radius && target.transform.position.z >= myOrigin.transform.position.z - radius)
            {
                return true;
            }
        }
        target.setMarked(false);
        return false;
    }

    private bool checkAllyInRadius()
    {
        if (pitcherTarget == null || pitcherTarget.getWaterLevel() > pitcherTarget.getMaxWaterLevel()-10)
            return false;
        if (pitcherTarget.transform.position.x <= myOrigin.transform.position.x + radius && pitcherTarget.transform.position.x >= myOrigin.transform.position.x - radius)
        {
            if (pitcherTarget.transform.position.z <= myOrigin.transform.position.z + radius && pitcherTarget.transform.position.z >= myOrigin.transform.position.z - radius)
            {
                return true;
            }
        }
        return false;
    }

    private void moveTowardsTarget()
    {
        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        bool xOnTarget = false;
        bool zOnTarget = false;

        if (target.transform.position.x > transform.position.x + 0.5f)
        {

        }
        else if (target.transform.position.x < transform.position.x - 0.5f)
        {

        }
        else
        {
            xOnTarget = true;
        }

        if (target.transform.position.z > transform.position.z + 0.5f)
        {

        }
        else if (target.transform.position.z < transform.position.z - 0.5f)
        {

        }
        else
        {
            zOnTarget = true;
        }

        if (xOnTarget && zOnTarget)
        {
            myWorld.destroyDaemon(target);
            target = null;
            retreating = true;
        }
        //rb.MovePosition(transform.position + transform.forward * 15 * Time.deltaTime);
        setRunning(true);
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }

    private void moveTowardsAlly()
    {
        transform.LookAt(pitcherTarget.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        bool xOnTarget = false;
        bool zOnTarget = false;

        if (pitcherTarget.transform.position.x > transform.position.x + 1f)
        {

        }
        else if (pitcherTarget.transform.position.x < transform.position.x - 1f)
        {

        }
        else
        {
            xOnTarget = true;
        }

        if (pitcherTarget.transform.position.z > transform.position.z + 1f)
        {

        }
        else if (pitcherTarget.transform.position.z < transform.position.z - 1f)
        {

        }
        else
        {
            zOnTarget = true;
        }

        if (xOnTarget && zOnTarget)
        {
            if (pitcherTarget.getWaterLevel() > pitcherTarget.getMaxWaterLevel() - 5)
            {
                pitcherEmitter.rateOverTime = 0;
                pitcherTarget = null;
                retreating = true;
            }
            else
            {
                pitcherEmitter.rateOverTime = 100;
                pitcherTarget.incrementWater();
            }
        }
        else
        {
            if(pitcherEmitter.rateOverTime.constant != 0)
                pitcherEmitter.rateOverTime = 0;
            setRunning(true);
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        }
    }

    private void shootAtTarget()
    {
        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        float projSpeed = 0.5f;

        for (int i = 0; i < 60; i++)
        {
            ParticleSystem temp = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/Spit"));
            temp.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            temp.transform.LookAt(target.transform);
            temp.transform.position = temp.transform.position + temp.transform.forward * i * projSpeed;
        }

        myWorld.destroyDaemon(target);
        target = null;
        retreating = true;
        laserCounter = 100;
    }

    public int getWaterLevel()
    {
        return waterLevel;
    }

    public int getMaxWaterLevel()
    {
        return maxWaterLevel;
    }

    private void moveToOrigin()
    {
        moveToOrigin(1);
    }

    private void moveToOrigin(int num)
    {
        if (!(transform.position.x < myOrigin.transform.position.x + num && transform.position.x > myOrigin.transform.position.x - num) || !(transform.position.z < myOrigin.transform.position.z + num && transform.position.z > myOrigin.transform.position.z - num))
        {
            setRunning(true);
            transform.LookAt(myOrigin.transform);
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        }
        else
        {
            setRunning(false);
            retreating = false;
        }
    }

    public void setWorld(World w)
    {
        myWorld = w;
        setOrigin();
    }

    private void teleportToOrigin()
    {
        if (!(transform.position.x < myOrigin.transform.position.x + 0.1f && transform.position.x > myOrigin.transform.position.x - 0.1f) || !(transform.position.z < myOrigin.transform.position.z + 0.1f && transform.position.z > myOrigin.transform.position.z - 0.1f))
        {
            Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/Smoke"), transform.position, transform.rotation);
            transform.position = myOrigin.transform.position;
            Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/Smoke"), transform.position, transform.rotation);
        }
    }

    private void setOrigin()
    {
        if (type == "Stalker")
        {
            myOrigin = myWorld.pl.gameObject;
        }
        else
        {
            myOrigin = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Origin"));
            myOrigin.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }

        

        range = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/Radius"), myOrigin.transform);
        ParticleSystem.ShapeModule ps = range.shape;
        ps.radius = radius;

        if (type == "Stalker")
        {
            range.transform.localPosition = new Vector3(0,-0.75f,0);
            ParticleSystem.MainModule m = range.main;
            m.simulationSpace = ParticleSystemSimulationSpace.Local;
        }

        waterIndicator = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/WaterLevel"), transform);
        ParticleSystem.ShapeModule psTwo = waterIndicator.shape;
        psTwo.radius = waterLevel / (float)maxWaterLevel;

        waterRadius = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/WaterRadius"), transform);
        ParticleSystem.ShapeModule psThree = waterRadius.shape;
        psThree.radius = maxWaterLevel / (float)maxWaterLevel;
    }

    private void updateWaterLevel()
    {
        ParticleSystem.ShapeModule psTwo = waterIndicator.shape;
        psTwo.radius = waterLevel / (float)maxWaterLevel;

        if (waterLevel < 10 && !warning)
        {
            ParticleSystem.MainModule psThree = waterRadius.main;
            psThree.startColor = new Color(100 / 255f, 0, 0);
            warning = true;
        }
        else if (waterLevel >= 10 && warning)
        {
            ParticleSystem.MainModule psThree = waterRadius.main;
            psThree.startColor = Color.black;
            warning = false;
        }
    }

    private void setRunning(bool run)
    {
        if (run)
        {
            if (runLegs != null)
                return;
            runLegs = StartCoroutine(doLegs());
            if(type != "SeedSpitter")
                sprout.transform.rotation = Quaternion.Euler(sprout.transform.rotation.eulerAngles.x, sprout.transform.rotation.eulerAngles.y, -55);
        }
        else
        {
            if (runLegs == null)
                return;
            StopCoroutine(runLegs);
            runLegs = null;
            sprout.transform.rotation = Quaternion.Euler(sprout.transform.rotation.eulerAngles.x, sprout.transform.rotation.eulerAngles.y, -45);
            rightLeg.transform.rotation = Quaternion.Euler(rightLeg.transform.rotation.eulerAngles.x, rightLeg.transform.rotation.eulerAngles.y, 0);
            leftLeg.transform.rotation = Quaternion.Euler(leftLeg.transform.rotation.eulerAngles.x, leftLeg.transform.rotation.eulerAngles.y, 0);
        }
    }

    private IEnumerator doLegs()
    {
        int i = 0;
        while (true)
        {
            i++;
            if (i == 45)
            {
                i = -45;
            }
            rightLeg.transform.rotation = Quaternion.Euler(rightLeg.transform.rotation.eulerAngles.x, rightLeg.transform.rotation.eulerAngles.y, rightLeg.transform.rotation.eulerAngles.z + i);
            leftLeg.transform.rotation = Quaternion.Euler(leftLeg.transform.rotation.eulerAngles.x, leftLeg.transform.rotation.eulerAngles.y, leftLeg.transform.rotation.eulerAngles.z - i);
            yield return new WaitForFixedUpdate();
        }
    }

    public void incrementWater()
    {
        if (waterLevel < maxWaterLevel)
            waterLevel++;
    }

    public void setSleeping(bool s)
    {
        sleeping = s;
        if (sleeping)
        {
            if (type == "Stalker")
            {
                Destroy(range.gameObject);
            }
            else
            {
                Destroy(myOrigin.gameObject);
            }
            Destroy(waterRadius);
            Destroy(waterIndicator);
        }
        else
        {
            setOrigin();
        }
    }

    public void die()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        if (type == "Stalker")
        {
            Destroy(range.gameObject);
        }
        else
        {
            Destroy(myOrigin.gameObject);
        }

        Destroy(this.gameObject);
        myWorld.destroySeedling(this);
    }
}
