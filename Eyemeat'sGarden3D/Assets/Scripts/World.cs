using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour {
    private GameObject groundPrefab;
    private GameObject grassPrefab;
    private GameObject hillPrefab;

    private List<Daemon> allDaemons = new List<Daemon>();
    private List<Seedling> allSeedlings = new List<Seedling>();

    private List<Daemon> justFlying = new List<Daemon>();

    public Player pl;
    public GameObject rift;

    private int enemyLevel = 0;
    private int maxEnemyLevel = 15;
    private int health = 30;

    private int enemyCooldown = 200;

    

    void Awake () {
        groundPrefab = Resources.Load<GameObject>("Prefabs/Ground");
        grassPrefab = Resources.Load<GameObject>("Prefabs/Grass");
        hillPrefab = Resources.Load<GameObject>("Prefabs/Hill");

        Cursor.visible = false;

        int radius = 0;

        for (int i = 0; i < radius; i++)
        {
            for(int j = 0; j < radius; j++)
            {
                if (Random.Range(0,20) == 21)
                {
                    GameObject tempHill = Instantiate<GameObject>(hillPrefab);
                    tempHill.transform.position = new Vector3(radius / 2 - i - 0.5f, 1, radius / 2 - j - 0.5f);
                }
                else {
                    GameObject tempGrass = Instantiate<GameObject>(grassPrefab);
                    tempGrass.transform.position = new Vector3(radius / 2 - i - 0.5f, 0.501f, radius / 2 - j - 0.5f);
                }
            }
        }

        increaseDifficulty(0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Random.Range(1, 10) == 1 && enemyCooldown < 0)
        {
            spawnDaemon("Imp");
            if (enemyLevel > 1)
            {
                for (int i = 0; i < Random.Range(1, (enemyLevel + 3)*(enemyLevel/3)); i++)
                {
                    spawnDaemon("Imp");
                }
            }
            if(enemyLevel > 5)
            {
                for (int i = 0; i < Random.Range(1, enemyLevel - 3); i++)
                {
                    spawnDaemon("Devil");
                }
            }
            if (enemyLevel > 7)
            {
                for (int i = 0; i < Random.Range(1, enemyLevel - 5); i++)
                {
                    spawnDaemon("Spirit");
                }
            }
            if (enemyLevel == 1)
                enemyCooldown = 400;
            else
            {
                enemyCooldown = 400 - 20 * enemyLevel;
                if (enemyCooldown < 25)
                    enemyCooldown = 60;
            }
        }
        enemyCooldown--;
	}

    public void spawnSeedling()
    {
        Seedling temp = Instantiate<Seedling>(Resources.Load<Seedling>("Prefabs/Seedling"));
        moveObjectInFrontOfPlayer(temp.gameObject);
        temp.setWorld(this);
        allSeedlings.Add(temp);
    }

    public void spawnSeedling(Vector3 pos)
    {
        spawnSeedling(pos, "Seedling");
    }

    public void spawnSeedling(Vector3 pos, string type)
    {
        Seedling temp = Instantiate<Seedling>(Resources.Load<Seedling>("Prefabs/"+type));
        temp.transform.position = pos;
        temp.setWorld(this);
        allSeedlings.Add(temp);
    }

    public void spawnSeedling(string type)
    {
        Seedling temp = Instantiate<Seedling>(Resources.Load<Seedling>("Prefabs/" + type));
        moveObjectInFrontOfPlayer(temp.gameObject);
        temp.setWorld(this);
        allSeedlings.Add(temp);
    }

    public void spawnDaemon()
    {
        Daemon temp = Instantiate<Daemon>(Resources.Load<Daemon>("Prefabs/Imp"));
        moveObjectInFrontOfPlayer(temp.gameObject);
        temp.setWorld(this);
        allDaemons.Add(temp);
    }

    public void spawnDaemon(Vector3 pos, string ai)
    {
        Daemon temp = Instantiate<Daemon>(Resources.Load<Daemon>("Prefabs/Imp"));
        temp.transform.position = pos;
        temp.setWorld(this);
        temp.setAI(ai);
        allDaemons.Add(temp);
    }

    public void spawnDaemon(string type)
    {
        Daemon temp = Instantiate<Daemon>(Resources.Load<Daemon>("Prefabs/"+type));
        if (type == "Spirit")
        {
            temp.transform.position = new Vector3(Random.Range(-45, -30), 2, Random.Range(-14, 14));
            justFlying.Add(temp);
        }
        else
        {
            temp.transform.position = new Vector3(Random.Range(-45, -30), 1, Random.Range(-14, 14));
        }
        temp.setWorld(this);
        if (type == "Imp")
            temp.setAI("runner");
        else if (type == "Devil")
            temp.setAI("chase");
        else if (type == "Spirit")
            temp.setAI("runner");
        allDaemons.Add(temp);
    }

    public void moveObjectInFrontOfPlayer(GameObject temp)
    {
        temp.transform.localPosition = pl.transform.position + Camera.main.transform.forward * 2;
        if(temp.transform.position.y < 0)
        {
            temp.transform.position = new Vector3(temp.transform.position.x, 0.5f, temp.transform.position.z);
        }
        temp.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y + 180, Camera.main.transform.rotation.eulerAngles.z);
    }

    public List<Daemon> getDaemons()
    {
        return allDaemons;
    }

    public List<Seedling> getSeedlings()
    {
        return allSeedlings;
    }

    public void destroyDaemon(Daemon d)
    {
        destroyDaemon(d, true);
    }

    public void destroyDaemon(Daemon d, bool money)
    {
        if(money)
            pl.incrementMoney(d.getValue());
        allDaemons.Remove(d);
        if (justFlying.Contains(d))
            justFlying.Remove(d);
        Destroy(d.gameObject);
    }

    public void destroySeedling(Seedling s)
    {
        allSeedlings.Remove(s);
        Destroy(s.gameObject);
    }

    public void increaseDifficulty(int increment)
    {
        enemyLevel += increment;
        if(enemyLevel > maxEnemyLevel)
        {
            enemyLevel = maxEnemyLevel;
        }
        pl.incrementDifficulty(enemyLevel);
    }

    public void incrementHealth(int increment)
    {
        health += increment;
        if (health <= 0)
        {
            PlayerPrefs.SetInt("lastNum", (int)pl.getTimeSoFar());
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
        pl.incrementHealth(health);
    }

    public GameObject getRift()
    {
        return rift;
    }

    public List<Daemon> getFlying()
    {
        return justFlying;
    }
}
