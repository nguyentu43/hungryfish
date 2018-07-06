using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    public float width_bg, height_bg;
    public float speed;
    public AudioClip audioGrowth, audioEat_1, audioEat_2, audioEatStar, audioFishStart, audioFail;
    public List<Sprite> spriteFish = new List<Sprite>();
    public GameObject crump;
    public bool isFail;
    public int shield_count;
    public int speed_count;

    private bool shield;
    private int level;
    private int growth;
    public int score;
    private Animator ani;
    private AudioSource audioSource;
    private float width, height;
    private bool collider_jelly;
    private Color colorDefault;
    public bool begin;

    public int life;

    public int growth_total;

    public int Level
    {
        get { return this.level; }
    }

    public int Growth
    {
        get { return this.growth; }
    }

    public bool Shield
    {
        get { return this.shield; }
    }

	// Use this for initialization
	void Awake () {
        audioSource = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();
        audioSource.PlayOneShot(audioFishStart);

        width = GetComponent<Renderer>().bounds.size.x;
        height = GetComponent<Renderer>().bounds.size.y;

        width_bg = GameManager.manager.width_bg;
        height_bg = GameManager.manager.height_bg;

        growth = 0;
        isFail = false;
        collider_jelly = false;
        shield = false;
        speed_count = 0;
        life = 3;
        level = 1;
        colorDefault = gameObject.GetComponent<SpriteRenderer>().color;

        List<int> fish_type = GameManager.manager.fish_type;

        for(int i=0; i<3; ++i)
            spriteFish.Add(GameManager.manager.fish_pf[fish_type[i]].GetComponent<SpriteRenderer>().sprite);

        growth_total = GameManager.manager.growth;
        shield_count = 0;

        begin = true;
        transform.localPosition = new Vector3(0, height_bg / 2 - height / 2, 0);

        StartCoroutine(BubbleShield(5));
        StartCoroutine(Message(0, 5));
	}
	
	// Update is called once per frame
	void Update () {

        if (GameManager.manager.isPause) return;

        if (begin)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 10 * Time.deltaTime);

            if (transform.localPosition == Vector3.zero)
                begin = false;
            return;
        }

        if (collider_jelly)
        {
            ani.SetBool("Swim", false);
            ani.SetBool("Idle", true);
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            return;
        }

        if (shield)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        Move();
	}

    private void Move()
    {
        if (isFail) return;

        Vector3 localScale = transform.localScale;
        Vector3 localPosition = transform.localPosition;

        if (localPosition.x > width_bg / 2)
        {
            transform.localPosition = new Vector3(width_bg / 2, localPosition.y, 0);
            return;
        }

        if (localPosition.x < -width_bg / 2)
        {
            transform.localPosition = new Vector3(-width_bg / 2, localPosition.y, 0);
            return;
        }

        if (localPosition.y > height_bg / 2)
        {
            transform.localPosition = new Vector3(localPosition.x, height_bg/2, 0);
            return;
        }

        if (localPosition.y < -height_bg / 2)
        {
            transform.localPosition = new Vector3(localPosition.x, -height_bg / 2, 0);
            return;
        }

        float move_X = 0, move_Y = 0;

        if (Input.GetMouseButtonDown(0))
        {
            if (localScale.x > 0) 
                transform.Translate(new Vector3(1f, 0, 0));
            else 
                transform.Translate(new Vector3(-1f, 0, 0));
            return;
        }

#if UNITY_STANDALONE

        move_X = Input.GetAxis("Mouse X");
        move_Y = Input.GetAxis("Mouse Y");

#elif UNITY_ANDROID

        move_X = CrossPlatformInputManager.GetAxis("Horizontal") / 3.25f;
        move_Y = CrossPlatformInputManager.GetAxis("Vertical") / 3.25f;

#endif

        ani.SetBool("Swim", true);
        ani.SetBool("Idle", false);

        if (move_X > 0)
        {
            if (localScale.x < 0)
            {
                localScale.x *= -1f;
            }
        }

        if (move_X < 0)
        {
            if (localScale.x > 0)
            {
                localScale.x *= -1f;
            }
        }

        transform.localScale = localScale;

        Vector3 move = new Vector3(move_X, move_Y, 0f);

        transform.Translate(move * speed);
    }

    void GrowthFish(int i) {
        Vector3 localScale = transform.localScale;
        switch (i)
        {
            case 2: localScale.x = 1.5f;
                    localScale.y = 1.5f;
                    this.level = 2;
                    break;
            case 3: localScale.x = 2.5f;
                    localScale.y = 2.5f;
                    this.level = 3;
                    break;
        }

        StartCoroutine(Message(level - 1, 5));
        transform.localScale = localScale;
        audioSource.PlayOneShot(audioGrowth);

        width = GetComponent<Renderer>().bounds.size.x;
        height = GetComponent<Renderer>().bounds.size.y;
    }

    void EatFish(int score, bool f) {
        if (!f)
        {
            audioSource.PlayOneShot(audioEat_1);
        }
        else
        {
            audioSource.PlayOneShot(audioEat_2);
        }

        this.growth += score/2;
        this.score += score;

        if (this.growth >= growth_total)
        {
            GameManager.manager.GameWin();
        }

        if (this.growth > growth_total * 0.4 && level == 1)
        {
            GrowthFish(2);
        }

        if (this.growth > growth_total * 0.7 && level == 2)
        {
            GrowthFish(3);
        }

        ani.SetTrigger("Eat");
    }

    void EatGold(int score)
    {
        this.score += score;
        audioSource.PlayOneShot(audioEatStar);

        ani.SetTrigger("Eat");
    }

    void EatBronze()
    {
        this.life++;
        audioSource.PlayOneShot(audioEatStar);

        ani.SetTrigger("Eat");
    }

    void EatShield()
    {
        if (!shield)
            StartCoroutine(BubbleShield(GameManager.manager.protect_time));
        else
            this.shield_count++;
        audioSource.PlayOneShot(audioEatStar);

        ani.SetTrigger("Eat");
    }

    void EatLightning()
    {
        if (speed >= 0.6)
        {
            speed_count++;
        }
        else
            StartCoroutine(incSpeed(GameManager.manager.speed_time));
        audioSource.PlayOneShot(audioEatStar);

        ani.SetTrigger("Eat");
    }

    public void AudioOn(bool f)
    {
        if (f)
            audioSource.volume = 1f;
        else
            audioSource.volume = 0;
    }

    public void GameFail()
    {
        audioSource.PlayOneShot(audioFail);
        this.life--;
        this.isFail = true;

        shield_count = 0;
        speed_count = 0;

        switch (this.level)
        {
            case 1: growth = 0; break;
            case 2: growth = (int)(growth_total * 0.4f); break;
            case 3: growth = (int)(growth_total * 0.7f); break;
        }

        if (this.life == 0)
        {
            GameManager.manager.GameOver();
            return;
        }

        if (collider_jelly) collider_jelly = false;

        StartCoroutine(HoiSinh(3));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetType() == typeof(BoxCollider2D) && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            if (!collider_jelly)
            {
                if (other.tag == "Fish_L0")
                {
                    int score = other.gameObject.GetComponent<Fish_1_Controller>().score;
                    other.gameObject.SetActive(false);
                    StartCoroutine(Crump(0.2f));
                    EatFish(score, false);
                }
                else if ((level > 1 && other.tag == "Fish_L1") || (level > 2 && other.tag == "Fish_L2"))
                {
                    int score = other.gameObject.GetComponent<Fish_2_Controller>().score;
                    other.gameObject.SetActive(false);
                    StartCoroutine(Crump(0.2f));
                    EatFish(score, true);
                }

                if (other.tag == "Gold")
                {
                    other.gameObject.SetActive(false);
                    EatGold(GameManager.manager.coin_point);
                }

                if (other.tag == "Bronze")
                {
                    other.gameObject.SetActive(false);
                    EatBronze();
                }

                if(other.tag == "Bubble_Shield")
                {
                    other.gameObject.SetActive(false);
                    EatShield();
                }

                if (other.tag == "Lightning")
                {
                    other.gameObject.SetActive(false);
                    EatLightning();
                }

                if (!shield && other.tag == "Jelly_Fish")
                {
                    StartCoroutine(Collider_Jelly_Fish());
                }
            }
        }
    }

    private IEnumerator Collider_Jelly_Fish()
    {
        collider_jelly = true;
        yield return new WaitForSeconds(4);
        collider_jelly = false;
        gameObject.GetComponent<SpriteRenderer>().color = colorDefault;
    }

    private IEnumerator HoiSinh(float time)
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.clear;
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<SpriteRenderer>().material.color = colorDefault;
        isFail = false;

        begin = true;
        transform.localPosition = new Vector3(0, height_bg / 2 - height / 2, 0);

        audioSource.PlayOneShot(audioFishStart);
        StartCoroutine(BubbleShield(5));
    }

    private IEnumerator BubbleShield(float time)
    {
        shield = true;
        yield return new WaitForSeconds(time);
        if (shield_count > 0)
        {
            shield_count--;
            StartCoroutine(BubbleShield(time));
        }
        else
            shield = false;
    }

    private IEnumerator incSpeed(float time)
    {
        speed += 0.2f;
        yield return new WaitForSeconds(time);
        speed -= 0.2f;

        if (speed_count > 0)
        {
            speed_count--;
            StartCoroutine(incSpeed(time));
        }
    }

    private IEnumerator Crump(float time)
    {
        float x = 0;

        if (transform.localScale.x > 0)
            x = transform.localPosition.x + width / 2 + 0.2f;
        else
            x = transform.localPosition.x + -width / 2 - 0.2f;

        GameObject gObject = Instantiate(crump, new Vector3(x, transform.localPosition.y + 0.2f, 0), Quaternion.identity);
        yield return new WaitForSeconds(time);
        GameObject.Destroy(gObject);
    }

    private IEnumerator Message(int index, float time)
    {
        transform.GetChild(1).gameObject.SetActive(true);

        Transform fish = transform.GetChild(1).GetChild(0);

        fish.GetComponent<SpriteRenderer>().sprite = spriteFish[index];

        if (spriteFish[index].name == "fish_1_1_0")
            fish.localScale = new Vector3(1f, 1f, fish.localScale.z);
        else
            fish.localScale = new Vector3(0.5f, 0.5f, fish.localScale.z);

        yield return new WaitForSeconds(time);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
