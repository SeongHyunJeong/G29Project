using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject selectionCanvas;

    void Start()
    {
        mainMenuCanvas.SetActive(true);
        selectionCanvas.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        mainMenuCanvas.SetActive(false);

        selectionCanvas.SetActive(true);
    }
}
