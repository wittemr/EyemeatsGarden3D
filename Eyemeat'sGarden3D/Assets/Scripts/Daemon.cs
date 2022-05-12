using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daemon : MonoBehaviour {
    private World myWorld;
    private Rigidbody rb;
    private int speed = 5;
    public string AI = "random";
    private bool marked = false;
    private bool forming = true;

    public string type;

    public int slowTurn = 0;

    private GameObject rift;

    private Vector3 angle;

    public GameObject devilRArm;
    public GameObject devilLArm;

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        angle = new Vector3(0, 90, 0);
        transform.localEulerAngles = angle;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (forming)
            return;
        if (AI == "random")
        {
            doRandomMovements();
        }
        else if (AI == "runner")
        {
            transform.localEulerAngles = angle;
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        }else if(AI == "goal")
        {
            chaseGoal();
        }else if(AI == "chase")
        {
            chasePlayer();
        }
	}

    private void doRandomMovements()
    {
        bool change = false;
        if(Random.Range(1,20) == 4)
        {
            change = true;
        }
        if (change)
        {
            transform.rotation = Quaternion.Euler(0,Random.Range(45,315),0);
        }
        else
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }
    }

    private void chaseGoal()
    {
        if (rift == null)
            return;
        transform.LookAt(rift.transform);
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }

    private void chasePlayer()
    {
        if (slowTurn == 0)
        {
            transform.LookAt(myWorld.pl.transform);
            slowTurn = 5;
        }
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        slowTurn--;
    }

    public void setWorld(World w)
    {
        myWorld = w;
        StartCoroutine(doForm());
    }

    public void endForming()
    {
        forming = false;
        if (type == "Imp")
        {
            StartCoroutine(walk());
        }
        else if (type == "Devil")
        {
            speed = 12;
            StartCoroutine(run());
        }else if(type == "Spirit")
        {
            StartCoroutine(fly());
        }
    }

    public void setAI(string ai)
    {
        AI = ai;
    }

    public int getValue()
    {
        return 1;
    }

    public bool getMarked()
    {
        return marked;
    }

    public void setMarked(bool m)
    {
        marked = m;
    }

    private IEnumerator walk()
    {
        transform.localEulerAngles = angle;
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(0, 90, transform.localEulerAngles.z + i);
                yield return new WaitForFixedUpdate();
            }

            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(0, 90, transform.localEulerAngles.z - i);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private IEnumerator run()
    {
        transform.localEulerAngles = angle;
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, transform.localEulerAngles.z + i/2);
                devilLArm.transform.localEulerAngles = new Vector3(devilLArm.transform.localEulerAngles.x + i, 0,0);
                devilRArm.transform.localEulerAngles = new Vector3(devilRArm.transform.localEulerAngles.x - i, 0,0);
                yield return new WaitForFixedUpdate();
            }

            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, transform.localEulerAngles.z - i/2);
                devilLArm.transform.localEulerAngles = new Vector3(devilLArm.transform.localEulerAngles.x - i, 0, 0);
                devilRArm.transform.localEulerAngles = new Vector3(devilRArm.transform.localEulerAngles.x + i, 0, 0);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private IEnumerator fly()
    {
        transform.localEulerAngles = angle;
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, transform.localEulerAngles.z + i / 2);
                devilLArm.transform.localEulerAngles = new Vector3(devilLArm.transform.localEulerAngles.x, devilLArm.transform.localEulerAngles.x+i*2, 0);
                devilRArm.transform.localEulerAngles = new Vector3(devilRArm.transform.localEulerAngles.x, devilRArm.transform.localEulerAngles.x+i*2, 0);
                yield return new WaitForFixedUpdate();
            }

            for (int i = 0; i < 10; i++)
            {
                transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, transform.localEulerAngles.z - i / 2);
                devilLArm.transform.localEulerAngles = new Vector3(devilLArm.transform.localEulerAngles.x, devilLArm.transform.localEulerAngles.x-i*2, 0);
                devilRArm.transform.localEulerAngles = new Vector3(devilRArm.transform.localEulerAngles.x, devilRArm.transform.localEulerAngles.x-i*2, 0);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private IEnumerator doForm()
    {
        transform.localEulerAngles = angle;
        transform.localScale *= 0.1f;
        ParticleSystem p = Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Prefabs/ParticleSystems/FormingDemon"));
        p.transform.position = transform.position;
        int timeToCook = Random.Range(900,1000);

        for (int i = 0; i < timeToCook*2; i++)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y+5, transform.localEulerAngles.z);
            if (i % 8 == 0)
                transform.localScale *= 1.01f;
            yield return new WaitForFixedUpdate();
        }

        transform.localEulerAngles = angle;
        Destroy(p.gameObject);
        endForming();
    }   

    private void OnCollisionEnter(Collision collide)
    {
        if(collide.gameObject.name == "Rift")
        {
            myWorld.incrementHealth(-1);
            myWorld.destroyDaemon(this, false);
        }

        if(collide.gameObject.name == "Player" && type == "Devil")
        {
            //collide.gameObject.GetComponent<Player>().getCurrentItem().drop();
            collide.rigidbody.velocity = (new Vector3(transform.forward.x, -transform.forward.y, transform.forward.z)).normalized * 50;
        }
    }

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.gameObject.name == "RiftTrigger" && type != "Devil")
        {
            rift = myWorld.getRift();
            setAI("goal");
        }
    }

    public string getType()
    {
        return type;
    }
}
