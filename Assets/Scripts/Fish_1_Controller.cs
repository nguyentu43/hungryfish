using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_1_Controller : MonoBehaviour {

    private float width, height;

    public int score = 20;

    public float speed = 0.3f;

    [HideInInspector]
    public float width_bg, height_bg;

    private Animator ani;

    private Color colorDefault;

    private bool collider_jelly;

    private GameManager manager;

    private float y;

	// Use this for initialization
	void Awake () {
        height_bg = GameManager.manager.height_bg;
        width_bg = GameManager.manager.width_bg;

        width = GetComponent<Renderer>().bounds.size.x;
        height = GetComponent<Renderer>().bounds.size.y;
        ani = GetComponent<Animator>();
        collider_jelly = false;
        colorDefault = GetComponent<SpriteRenderer>().color;
        transform.localPosition = RandomPosition();

        y = Random.Range(-0.02f, 0.02f);
    }

    Vector3 RandomPosition()
    {
        float x = width_bg / 2;

        if (Random.Range(-1, 1) == 0)
            x = -x;

        float y = Random.Range(-height_bg / 2, height_bg / 2);

        return new Vector3(x, y, 0);
    }
	
	// Update is called once per frame
	void Update () {

        if (GameManager.manager.isPause) return;

        if (collider_jelly)
        {
            return;
        }

        Vector3 localPosition = transform.localPosition;
        Vector3 localScale = transform.localScale;
        Vector3 move = Vector3.zero;

        if ((localScale.x > 0 && localPosition.x > width_bg / 2) ||
            (localScale.x < 0 && localPosition.x < -width_bg / 2))
        {
            reverse();
        }
        else
        {
            int i = UnityEngine.Random.Range(0, 500);
            if (i == 1) reverse();
        }

        if (Mathf.Abs(localPosition.y) > height_bg / 2 + height / 2)
        {
            y = -y;
        }

        if (transform.localScale.x > 0)
        {
            move = new Vector3(0.1f, y, 0);
        }
        else if(transform.localScale.x < 0)
        {
            move = new Vector3(-0.1f, y, 0);
        }

        transform.Translate(move * speed);
	}

    private void reverse()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
        y = Random.Range(-0.02f, 0.02f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetType() == typeof(CircleCollider2D))
        {
            if (other.tag == "Player" && !collider_jelly)
            {
                reverse();
            }
        }
        else if (other.GetType() == typeof(BoxCollider2D) && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            if (other.tag == "Jelly_Fish")
            {
                StartCoroutine(Collider_Jelly_Fish());
            }
        }
    }

    private IEnumerator Collider_Jelly_Fish()
    {
        collider_jelly = true;
        ani.SetBool("Swim", false);
        ani.SetBool("Idle", true);
        GetComponent<SpriteRenderer>().color = Color.green;
        yield return new WaitForSeconds(4);
        GetComponent<SpriteRenderer>().color = colorDefault;
        collider_jelly = false;
        ani.SetBool("Swim", true);
        ani.SetBool("Idle", false);
    }
}
