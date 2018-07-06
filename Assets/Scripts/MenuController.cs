using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public Button btn_level;

    public Sprite[] bg;

    private int n;
    private int m;

    const int MAX_PAGE = 2;
    const int MIN_PAGE = 1;

	// Use this for initialization
	void Start () {
        Button btn_resume = GameObject.FindGameObjectWithTag("Button_Resume").GetComponent<Button>();
        if (PlayerPrefs.GetInt("Score", 0) == 0)
            btn_resume.interactable = false;
        CheckAudio();

        n = 2;
        m = 5;
	}

    private void CheckAudio()
    {
        Button audio = GameObject.FindGameObjectWithTag("Button_Audio").GetComponent<Button>();

        AudioSource audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("AudioOn") == 0)
        {
            audio.transform.GetChild(0).GetComponent<Text>().text = "AUDIO OFF";
            audioSource.volume = 0;
        }
        else
        {
            audio.transform.GetChild(0).GetComponent<Text>().text = "AUDIO ON";
            audioSource.volume = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadScene(string name) { 
        SceneManager.LoadScene(name);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void NewGame(Canvas canvas)
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Life", 3);

        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = bg[0];

        GameObject.Find("canvas_menu").SetActive(false);
        canvas.gameObject.SetActive(true);
        MainLevel();
    }

    public void Resume(Canvas canvas)
    {
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = bg[0];
        GameObject.Find("canvas_menu").SetActive(false);
        canvas.gameObject.SetActive(true);
        MainLevel();
    }

    public void AudioOn()
    {
        if (PlayerPrefs.GetInt("AudioOn", 1) == 1)
        {
            PlayerPrefs.SetInt("AudioOn", 0);
        }
        else
        {
            PlayerPrefs.SetInt("AudioOn", 1);
        }

        CheckAudio();
    }

    public void CloseWindow(Image window)
    {
        window.gameObject.SetActive(false);
    }

    public void ReturnMainMenu(Canvas canvas)
    {
        canvas.gameObject.SetActive(true);
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = bg[0];
        Canvas level = GameObject.Find("canvas_level").GetComponent<Canvas>();

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
            {
                int index = m * i + j;
                GameObject.Destroy(level.transform.GetChild(index + 3).gameObject);
            }
        }

        level.gameObject.SetActive(false);
    }

    public void CloseBoardHighScore(Image window)
    {
        CloseWindow(window);
        GameObject.Find("btn_exit").GetComponent<Button>().interactable = true;
    }

    public void HighScore(Image broad)
    {
        broad.gameObject.SetActive(true);

        GameObject.Find("btn_exit").GetComponent<Button>().interactable = false;

        Text text = GameObject.Find("list_score").GetComponent<Text>();
        text.text = ReadScore();
    }

    string ReadScore()
    {
        string s = string.Empty;

        List<User> list = SqliteDB.GetAll();

        if (list.Count > 0)
        {

            for (int i = 0; i < list.Count; ++i)
            {
                s += (i + 1).ToString() + ". " + list[i].ToString();
            }
        }
        else
        {
            s = "None";
        }

        return s;
    }

    public void SelectLevel(Button btn)
    {
        int n = System.Convert.ToInt32(btn.transform.GetChild(0).GetComponent<Text>().text);
        int stage = n / 10 + 1;
        int level = n % 10;

        PlayerPrefs.SetInt("P_Stage", stage);
        PlayerPrefs.SetInt("P_Level", level);

        SceneManager.LoadScene("main");
    }

    void ChangePage(int p)
    {
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = bg[p];

        int stage = PlayerPrefs.GetInt("Stage", 1);
        int level = PlayerPrefs.GetInt("Level", 1);

        Canvas canvas = GameObject.Find("canvas_level").GetComponent<Canvas>();

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
            {

                int index = m * i + j + 1;

                Button btn = canvas.transform.GetChild(index + 2).GetComponent<Button>();

                btn.transform.GetChild(0).GetComponent<Text>().text = (index + (p - 1) * 10).ToString();

                if (index + (p - 1) * 10 > ((stage - 1) * 10 + level))
                {
                    btn.GetComponent<Button>().interactable = false;
                }
                else
                {
                    btn.GetComponent<Button>().interactable = true;
                }
            }

            Button btn_previous = GameObject.Find("btn_previous").GetComponent<Button>();
            Button btn_next = GameObject.Find("btn_next").GetComponent<Button>();

            if (p > MIN_PAGE)
            {
                btn_previous.interactable = true;
                btn_previous.onClick.AddListener(() => ChangePage(p - 1));
            }
            else
            {
                btn_previous.interactable = false;
            }

            if (p >= MAX_PAGE)
            {
                btn_next.interactable = false;
            }
            else
            {
                btn_next.interactable = true;
                btn_next.onClick.AddListener(() => ChangePage(p + 1));
            }
        }
    }

    public void MainLevel()
    {
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite = bg[1];

        Canvas canvas = GameObject.Find("canvas_level").GetComponent<Canvas>();
        canvas.gameObject.SetActive(true);

        Button btn_next = GameObject.Find("btn_next").GetComponent<Button>();
        Button btn_previous = GameObject.Find("btn_previous").GetComponent<Button>();

        int n = 2;
        int m = 5;

        float x = btn_level.transform.localPosition.x;
        float y = btn_level.transform.localPosition.y;

        float w = btn_level.GetComponent<RectTransform>().rect.width;
        float h = btn_level.GetComponent<RectTransform>().rect.height;

        int stage = PlayerPrefs.GetInt("Stage", 1);
        int level = PlayerPrefs.GetInt("Level", 1);

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
            {
                GameObject btn = Instantiate(btn_level).gameObject;

                btn.GetComponent<Button>().onClick.AddListener(() => SelectLevel(btn.GetComponent<Button>()));

                btn.transform.SetParent(canvas.transform);

                btn.transform.GetChild(0).GetComponent<Text>().text = (m * i + j + 1).ToString();

                btn.transform.localPosition = new Vector3(x + w / 2 + w * j * 1.5f, y - h / 2 - h * i * 1.5f);

                btn.transform.localScale = new Vector3(1, 1, 1);

                if ((m * i + j + 1) > ((stage - 1) * 10 + level))
                {
                    btn.GetComponent<Button>().interactable = false;
                }
            }
        }

        btn_previous.interactable = false;

        btn_next.interactable = true;
        btn_next.onClick.AddListener(() => ChangePage(2));
    }
}
