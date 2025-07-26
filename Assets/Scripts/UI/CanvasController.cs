using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    //Make sure there is only one instance of the game manager
    public static CanvasController instance;
    public Image AmmoImage;
   
    void Awake()
    {
        //Needed or game manager loses all it's references!
        if (instance == null)
        {
            //Keep this copy and only this copy
            instance = this;
            //Don't destroy between scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(WaitForGameManager());
    }


    private IEnumerator WaitForGameManager()
    {
        while (GameManager.instance == null || GameManager.instance.AmmoSprite == null)
        {
            yield return null; // Wait until GameManager and AmmoSprite are ready
        }

        
        AmmoImage.sprite = GameManager.instance.AmmoSprite;
    }
}
