using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMap : MonoBehaviour
{
    public void OnStartButtonClicked_School()
    {
        SceneManager.LoadScene("School_NotComplete");
    }
    public void OnStartButtonClicked_Fun()
    {
        SceneManager.LoadScene("FunctionalTestRoom");
    }
}