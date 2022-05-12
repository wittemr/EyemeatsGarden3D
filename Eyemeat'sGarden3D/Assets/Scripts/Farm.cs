using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {
    private World myWorld;
    public GameObject growth;
    public ParticleSystem growthEffect;

    private Coroutine growing;

    // Use this for initialization
    void Awake () {
        myWorld = FindObjectOfType<World>();
        ParticleSystem.EmissionModule gE = growthEffect.emission;
        gE.rateOverTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Item c = collision.gameObject.GetComponent<Item>();
        if(c != null && c.getSeed())
        {
            if (growing == null)
            {
                growing = StartCoroutine(doGrow(c));
                Destroy(c.gameObject);
            }
        }
    }

    private void spawnFromSeed(Item seed)
    {
        myWorld.spawnSeedling(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), seed.getSeedType());
    }

    private IEnumerator doGrow(Item seed)
    {
        float speed = 1.001f;
        growth.gameObject.SetActive(true);

        ParticleSystem.EmissionModule gE = growthEffect.emission;
        gE.rateOverTime = 100;

        for (int i = 0; i < 1000; i++)
        {
            growth.transform.localScale *= speed;
            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < 1000; i++)
        {
            growth.transform.localScale /= speed;
        }

        gE.rateOverTime = 0;

        growth.gameObject.SetActive(false);
        spawnFromSeed(seed);
        growing = null;
    }
}
