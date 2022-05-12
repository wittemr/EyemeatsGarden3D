using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watering : MonoBehaviour {
    Player mainPlayer;
    private Item lastItem;
    private bool growIndicator = false;

    void Awake()
    {
        GameObject p = GameObject.Find("Player");
        mainPlayer = p.GetComponent<Player>();
    }

    void FixedUpdate () {
		
	}

    private void OnTriggerStay(Collider collision)
    {
        Seedling checkSeedling = collision.gameObject.GetComponent<Seedling>();
        if(checkSeedling != null && mainPlayer.isSprinkling())
        {
            checkSeedling.incrementWater();
        }else if(checkSeedling != null && mainPlayer.getCurrentItem() != null && mainPlayer.getCurrentItem().gameObject.name == "Wheelbarrow" && mainPlayer.getCurrentItem().getCurrentSeedling() == null)
        {
            mainPlayer.getCurrentItem().highlight(true);
            mainPlayer.mySeedling = checkSeedling;
        }
        else if (checkSeedling != null && mainPlayer.getCurrentItem() != null && mainPlayer.getCurrentItem().gameObject.name == "Clippers")
        {
            mainPlayer.getCurrentItem().highlight(true);
            if (Input.GetMouseButton(0))
            {
                checkSeedling.die();
                mainPlayer.getCurrentItem().highlight(false);
            }
        }

        Item checkItem = collision.gameObject.GetComponent<Item>();
        if(checkItem != null && mainPlayer.getCurrentItem() == null)
        {
            lastItem = checkItem;
            checkItem.highlight(true);
            if (Input.GetMouseButton(0))
            {
                mainPlayer.holdItem(checkItem);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Seedling checkSeedling = collision.gameObject.GetComponent<Seedling>();
        if (checkSeedling != null && mainPlayer.getCurrentItem() != null && mainPlayer.getCurrentItem().gameObject.name == "Wheelbarrow" && mainPlayer.getCurrentItem().getCurrentSeedling() == null)
        {
            mainPlayer.getCurrentItem().highlight(false);
            mainPlayer.mySeedling = null;
        }else if (checkSeedling != null && mainPlayer.getCurrentItem() != null && mainPlayer.getCurrentItem().gameObject.name == "Clippers")
        {
            mainPlayer.getCurrentItem().highlight(false);
        }

        Item checkItem = collision.gameObject.GetComponent<Item>();
        if (checkItem != null)
        {
            checkItem.highlight(false);
        }
    }
}
