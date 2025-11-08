using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerAttacher : MonoBehaviour
{
    void Awake()
    {
        BootingUp();
    }

    //----------------------------------------------

    private void BootingUp()
    {
        if (SceneManager.GetSceneByName("SceneAttacher").isLoaded == false)
        {
            Debug.Log("Managers Boot Complete");
            SceneManager.LoadScene("SceneAttacher", LoadSceneMode.Additive);
        }
    }
}
