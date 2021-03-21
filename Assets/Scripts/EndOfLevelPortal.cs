using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevelPortal : MonoBehaviour
{
    public LevelList levels;
    public bool overrideScene;
    public SceneAsset sceneForOverride = null;

    public LayerMask whatIsPlayer;

    private float negativeDistance = 0.1f;

    private Collider2D _collider;
    // Start is called before the first frame update
    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // check if correctly-sized character is in portal

        Bounds bounds = _collider.bounds;
        Vector2[] positivePoints =
        {
            new Vector2(bounds.extents.x, 0f),
            new Vector2(-bounds.extents.x, 0f),
            new Vector2(0f, bounds.extents.y),
            new Vector2(0f, -bounds.extents.y)
        };
        Vector2 center = new Vector2(bounds.center.x, bounds.center.y);

        foreach (Vector2 vec in positivePoints)
        {
            Collider2D c = Physics2D.OverlapCircle(
                vec + center,
                negativeDistance / 2f,
                whatIsPlayer
            );
            if (c is null) return;

            c = Physics2D.OverlapCircle(
                vec + vec.normalized * negativeDistance + center,
                negativeDistance / 2f,
                whatIsPlayer
            );
            if (!(c is null)) return;
        }

        NextLevel();
    }

    void NextLevel()
    {
        // TODO async this maybe
        string sceneName = SceneManager.GetActiveScene().name;

        if (overrideScene)
        {
            SceneManager.LoadScene(sceneForOverride.name);
        }

        for (int i = 0; i < levels.scenes.Count; i++)
        {
            SceneAsset s = levels.scenes[i];
            if (s.name != sceneName) continue;

            int levelNumber = i + 1;
            if (PlayerPrefs.GetInt("maxUnlocked", 1) < levelNumber)
            {
                PlayerPrefs.SetInt("maxUnlocked", levelNumber);
                PlayerPrefs.Save();
            }

            SceneManager.LoadScene(levels.scenes[levelNumber].name, LoadSceneMode.Single);
            break;
        }

    }

}
