using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static GameManager manager;
    public List<GameObject> player_pf;
    public List<GameObject> fish_pf;
    public GameObject jelly_fish_pf;
    public List<GameObject> item;
    public GameObject bg;
    public List<Sprite> sprite_bg;
    public Image progress_bar;
    public Text count_life;
    public Text score_text;
    public Text level_text;

    [HideInInspector]
    public bool isPause;

    public AudioClip audio_bg, audio_btnClick, audio_complete;

    public AudioSource audioSource;

    public Image window_gameover;
    public Image window_win;
    public Image window_pause;
    public Image window_input;
    public Image win_complete;
    
    public Text text_name;
    public List<Sprite> sprite_btn_audio;

    public int growth;
    public int stage;
    public int level;

    public int bg_type;
    public int player_type;
    public List<int> fish_type;
    public List<int> fish_count;
    public int item_count;
    public int jfish_count;

    [HideInInspector]
    public float width_bg, height_bg;

    public int coin_point = 100;
    public int speed_time = 15;
    public int protect_time = 15;
    public int random_time = 60;

    int k;
    string path = "highscore.txt";

	// Use this for initialization
	void Awake () {
        manager = this;
        InitGame();
	}

    private void InitGame()
    {
        stage = PlayerPrefs.GetInt("P_Stage", 1);
        level = PlayerPrefs.GetInt("P_Level", 1);
        SetupLevel(stage, level);

        level_text.text = string.Format("Level: {0}", (stage - 1 ) * 10 + level);

        isPause = false;
        width_bg = bg.GetComponent<Renderer>().bounds.size.x;
        height_bg = bg.GetComponent<Renderer>().bounds.size.y;

        audioSource = GetComponent<AudioSource>();

        Instantiate(player_pf[player_type]);
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        player.score = PlayerPrefs.GetInt("Score", player.score);
        player.life = PlayerPrefs.GetInt("Life", player.life);

        bg.GetComponent<SpriteRenderer>().sprite = sprite_bg[bg_type];
        Instantiate(bg, Vector3.zero, Quaternion.identity);

        CheckAudio();

        SetFishProgressBar();

        GenerateFish();

        GenerateItem();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CheckAudio()
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (PlayerPrefs.GetInt("AudioOn") == 0)
        {
            audioSource.volume = 0;
            player.AudioOn(false);
            window_pause.transform.GetChild(3).GetComponent<Image>().sprite = sprite_btn_audio[0];
        }
        else
        {
            audioSource.volume = 1f;
            player.AudioOn(true);
            window_pause.transform.GetChild(3).GetComponent<Image>().sprite = sprite_btn_audio[1];
        }
    }

    public void AudioOn()
    {
        if (PlayerPrefs.GetInt("AudioOn") == 0)
        {
            PlayerPrefs.SetInt("AudioOn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("AudioOn", 0);
        }

        CheckAudio();
    }

    void SetupLevel(int s, int i)
    {
        if (s == 1)
        {
            bg_type = 0;
            player_type = 0;

            fish_type[0] = 0;
            fish_type[1] = 2;
            fish_type[2] = 4;
            fish_type[3] = 6;

            fish_count[0] = 40 - (i - 1)*2;
            fish_count[1] = 15 + i / 2;
            fish_count[2] = 10 + i / 2;
            fish_count[3] = i / 5;

            growth = 1000 + (i - 1) * 20;

            item_count = 3;

            jfish_count = 2;
        }
        else if(s == 2)
        {
            bg_type = 1;
            player_type = 1;

            fish_type[0] = 1;
            fish_type[1] = 3;
            fish_type[2] = 5;
            fish_type[3] = 6;

            fish_count[0] = 35 - (i - 1)*2;
            fish_count[1] = 20 + i / 2;
            fish_count[2] = 15 + i / 2;
            fish_count[3] = i / 3;

            growth = 1200 + i * 40;

            item_count = 2;

            jfish_count = 4;
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void PlayAgain()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Life", 3);

        LoadScene("main");
    }

    public void NextLevel()
    {
        if (level == 10)
        {
            PlayerPrefs.SetInt("Stage", stage + 1);
            PlayerPrefs.SetInt("Level", 1);

            PlayerPrefs.SetInt("P_Stage", stage + 1);
            PlayerPrefs.SetInt("P_Level", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level", level + 1);

            PlayerPrefs.SetInt("P_Level", level + 1);
        }

        PlayerController player = GameObject.FindObjectOfType<PlayerController>();

        PlayerPrefs.SetInt("Score", player.score);
        PlayerPrefs.SetInt("Life", player.life);
    }

    private void GenerateFish()
    {
        StartCoroutine(Fish(fish_type[0], 0.5f));

        StartCoroutine(Fish(fish_type[1], 5f));

        StartCoroutine(Fish(fish_type[2], 7f));

        StartCoroutine(Fish(fish_type[3], 7f));

        StartCoroutine(Jelly_Fish(10f));
    }

    private void GenerateItem()
    {
        StartCoroutine(ItemRandom(random_time));
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (window_gameover.gameObject.activeInHierarchy || window_win.gameObject.activeInHierarchy) return;

            if (window_pause.gameObject.activeInHierarchy)
            {
                isPause = false;
                CloseWindow(window_pause);
            }
            else
            {
                isPause = true;
                OpenWindow(window_pause);
            }
        }

        UpdateTopBar();
    }

    public void GameWin()
    {
        if (stage == 2 && level == 10)
        {
            GameComplete();
            return;
        }

        isPause = true;
        OpenWindow(window_win);
        audioSource.clip = audio_complete;
        audioSource.Play();
        NextLevel();
    }

    void GameComplete()
    {
        isPause = true;
        OpenWindow(win_complete);
        CheckScore();
    }

    public void OpenWindow(Image win)
    {
        win.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseWindow(Image win)
    {
        win.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ClosePause()
    {
        isPause = false;
        CloseWindow(window_pause);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateTopBar()
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        progress_bar.transform.GetChild(0).GetComponent<Image>().fillAmount = (float) (player.Growth) / growth;

        count_life.text = "x " + player.life;

        score_text.text = "Score: " + player.score;
    }

    void SetFishProgressBar()
    {
        for(int i=0; i<3; ++i)
            progress_bar.transform.GetChild(i+1).GetComponent<Image>().sprite = fish_pf[fish_type[i]].GetComponent<SpriteRenderer>().sprite;
    }

    private IEnumerator Fish(int index, float time)
    {
        if(GameObject.FindGameObjectsWithTag(fish_pf[index].tag).Length < fish_count[fish_type.IndexOf(index)])
            Instantiate(fish_pf[index]);
        yield return new WaitForSeconds(time);
        StartCoroutine(Fish(index, time));
    }

    private IEnumerator Jelly_Fish(float time)
    {
        for (int i = 0; i < jfish_count; ++i)
            Instantiate(jelly_fish_pf);
        yield return new WaitForSeconds(time);
        StartCoroutine(Jelly_Fish(time));
    }

    private IEnumerator ItemRandom(float time)
    {
        int index;
        for (int i = 0; i < item_count; ++i)
        {
            index = Random.Range(0, item.Count);
            Instantiate(item[index]);
        }
        yield return new WaitForSeconds(time);
        StartCoroutine(ItemRandom(time));
    }

    public void GameOver()
    {
        OpenWindow(window_gameover);
        CheckScore();
    }

    private void CheckScore()
    {
        int score = GameObject.FindObjectOfType<PlayerController>().score;
        if (score == 0) return;

        List<User> list = SqliteDB.GetAll();

        k = -1;

        for (int i = 0; i < list.Count; ++i)
        {
            if (score >= list[i].score)
            {
                k = i;
                break;
            }
        }

        if (list.Count == 0 || k > -1 || list.Count < 5)
        {
            OpenWindow(window_input);
        }
        else
        {
            PlayerPrefs.SetInt("Score", 0);
        }
    }

    public void SaveScore()
    {
        string name = text_name.text;

        List<User> list = SqliteDB.GetAll();

        int score = GameObject.FindObjectOfType<PlayerController>().score;

        PlayerPrefs.SetInt("Score", 0);

        User user = new User(0, name, score);

        SqliteDB.Insert(user);

        window_input.gameObject.SetActive(false);
    }
}
