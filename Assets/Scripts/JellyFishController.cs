using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishController : MonoBehaviour {

    public float speed;

    public float width_bg, height_bg;

    private float height, width;

    private Animator ani;

	// Use this for initialization
	void Awake() {
        ani = GetComponent<Animator>();
        width = GetComponent<Renderer>().bounds.size.x;
        height = GetComponent<Renderer>().bounds.size.y;

        width_bg = GameManager.manager.width_bg;
        height_bg = GameManager.manager.height_bg;

        transform.localPosition = new Vector3(Random.Range(-width_bg / 2 + height/2, width_bg / 2 - height/2), -height_bg/2 - height, 0);
	}
	
	// Update is called once per frame
	void Update () {

        if (GameManager.manager.isPause) return;

        if (transform.localPosition.y < height_bg/2 + height)
        {
            transform.Translate(new Vector3(0, 0.1f, 0) * speed);
        }
        else
        {
            gameObject.SetActive(false);
        }
	}
}
