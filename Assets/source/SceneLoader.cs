using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private AsyncOperation async = null; // When assigned, load is in progress.
    private ProgressBar m_loading = null;
    
    // Use this for initialization
    void Start () {
        m_loading = GetComponent<ProgressBar>();
        StartCoroutine(LoadALevel("scene_singleplayer"));
	}
	
	// Update is called once per frame
	void Update () {
        if (m_loading != null && async != null)
        {
            m_loading.setValue(async.progress);
        }
	}
    
    private IEnumerator LoadALevel(string scenename)
    {
        async = SceneManager.LoadSceneAsync("scene_singleplayer", LoadSceneMode.Single);
        while (!async.isDone)
        {
            yield return async;
        }
        async.allowSceneActivation = true;
    }
}
