using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;

    private GameObject player;

    private float width_bg, height_bg;

    private float height, width;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        width_bg = GameManager.manager.width_bg;
        height_bg = GameManager.manager.height_bg;

        height = 2f * Camera.main.orthographicSize;
        width = height * Camera.main.aspect;
	}
	
	// Update is called once per frame
	void Update () {

        if (player.GetComponent<PlayerController>().begin)
        {
            transform.position = new Vector3(0, 0, transform.position.z);
            return;
        }

        float x = player.transform.position.x;
        float y = player.transform.position.y;

        if (Mathf.Abs(x) >= (width_bg / 2 - width / 2))
        {
            if (transform.position.x < 0)
                x = -(width_bg / 2 - width / 2);
            else
                x = width_bg / 2 - width / 2;
        }

        if (Mathf.Abs(y) >= (height_bg / 2 - height / 2))
        {
            if (transform.position.y < 0)
                y = -(height_bg / 2 - height / 2);
            else
                y = height_bg / 2 - height / 2;
        }

        Vector3 target = new Vector3(x, y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
	}
}
