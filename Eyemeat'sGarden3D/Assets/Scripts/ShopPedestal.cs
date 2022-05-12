using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPedestal : MonoBehaviour {
    public Text priceDisplay;
    public string selling;

    private Item mySeeds;

    private Coroutine doRed;
    private bool isRed = false;

    // Use this for initialization
    void Awake() {
        spawnSeeds();
    }

    // Update is called once per frame
    void Update() {

    }

    public void spawnSeeds()
    {
        mySeeds = Instantiate<Item>(Resources.Load<Item>("Prefabs/Seeds"));
        mySeeds.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        mySeeds.setSeedType(selling);
        mySeeds.setShop(this);
    }

    public int getCost()
    {
        return int.Parse(priceDisplay.text.Substring(1));
    }

    public void flashRed()
    {
        if (doRed != null)
        {
            StopCoroutine(doRed);
            if (isRed)
                toggleRed();
        }
        doRed = StartCoroutine(doFlashRed());
    }

    public IEnumerator doFlashRed()
    {
        for(int i = 0; i < 50; i++)
        {
            if (i % 10 == 0)
                toggleRed();
            yield return new WaitForEndOfFrame();
        }
        if (isRed)
            toggleRed();
    }

    private void toggleRed()
    {
        if (isRed)
        {
            priceDisplay.color = Color.black;
            isRed = false;
        }
        else
        {
            priceDisplay.color = Color.red;
            isRed = true;
        }
    }
}
