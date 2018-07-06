using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_2_Controller : MonoBehaviour {

    private float width, height;

    public int score;
    public float speed;

    private Animator ani;

    [HideInInspector]
    public float width_bg, height_bg;

    private bool collider_jelly;
    private Color colorDefault;

    private float y;

	// Use this for initialization
    void Awake()
    {

        width_bg = GameManager.manager.width_bg;
        height_bg = GameManager.manager.height_bg;

        width = GetComponent<Renderer>().bounds.size.x;
        height = GetComponent<Renderer>().bounds.size.y;
        ani = GetComponent<Animator>();
        transform.localPosition = RandomPosition();
        collider_jelly = false;

        colorDefault = GetComponent<SpriteRenderer>().color;

        if (tag == "Fish_L1")
        {
            score = 40;
            speed = 0.4f;
        }
        else if (tag == "Fish_L2")
        {
            score = 60;
            speed = 0.6f;
        }
        else if (tag == "Fish_L3")
        {
            speed = 0.8f;
        }

        y = UnityEngine.Random.Range(-0.02f, 0.02f);
    }

    Vector3 RandomPosition()
    {
        float x = width_bg / 2 + width / 2;

        if(UnityEngine.Random.Range(-1, 1) == 0)
            x = -x;

        float y = UnityEngine.Random.Range(-height_bg / 2 + height / 2, height_bg / 2 - height / 2);

        return new Vector3(x, y, 0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (GameManager.manager.isPause) return;

        if (collider_jelly)
        {
            ani.SetBool("Idle", true);
            ani.SetBool("Swim", false);
            return;
        }

        Vector3 localPosition = transform.localPosition;
        Vector3 localScale = transform.localScale;
        Vector3 move = Vector3.zero;

        if ((localScale.x > 0 && localPosition.x > width_bg / 2 + width / 2) ||
            (localScale.x < 0 && localPosition.x < -width_bg / 2 - width / 2))
        {
            reverseX();
        }
        else
        {
            int i = UnityEngine.Random.Range(0, 500);
            if (i == 1) reverseX();
        }

        if (Mathf.Abs(localPosition.y) > height_bg / 2 + height / 2)
        {
            y = -y;
        }

        if (transform.localScale.x > 0)
        {
            move = new Vector3(0.1f, y, 0);
        }
        else if (transform.localScale.x < 0)
        {
            move = new Vector3(-0.1f, y, 0);
        }

        transform.Translate(move * speed);

        ani.SetBool("Swim", true);
        ani.SetBool("Idle", false);
	}

    void reverseX()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
        y = UnityEngine.Random.Range(-0.02f, 0.02f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetType() == typeof(BoxCollider2D) && gameObject.GetComponent<BoxCollider2D>().IsTouching(other) )
        {
            if (!collider_jelly)
            {
                int l1 = Convert.ToInt32(tag[tag.Length - 1]);
                int l2 = Convert.ToInt32(other.tag[other.tag.Length - 1]);
                if (l1 > l2)
                {
                    other.gameObject.SetActive(false);
                    ani.SetTrigger("Eat");
                }

                if (other.tag == "Player")
                {
                    PlayerController player = other.gameObject.GetComponent<PlayerController>();

                    if (!player.isFail)
                    {
                        if (!player.Shield)
                        {
                            int levelPlayer = player.Level;

                            if (levelPlayer < 2 || (gameObject.tag == "Fish_L2" && levelPlayer < 3))
                            {
                                other.gameObject.GetComponent<PlayerController>().GameFail();
                                ani.SetTrigger("Eat");
                            }
                        }
                    }
                }
            }

            if (other.tag == "Jelly_Fish")
            {
                StartCoroutine(Collider_Jelly_Fish());
            }
        }

        if (other.tag == "Fish_L3") return;

        if(other.GetType() == typeof(CircleCollider2D))
        {
            if (other.tag.CompareTo("Player") == 0)
            {
                int levelPlayer = other.gameObject.GetComponent<PlayerController>().Level;

                if (!collider_jelly && ( levelPlayer > 2 || (gameObject.tag == "Fish_L1" && levelPlayer > 1)))
                {
                    reverseX();
                }
            }
        }
    }

    private IEnumerator Collider_Jelly_Fish()
    {
        collider_jelly = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        yield return new WaitForSeconds(4);
        collider_jelly = false;
        gameObject.GetComponent<SpriteRenderer>().color = colorDefault;
    }
}
