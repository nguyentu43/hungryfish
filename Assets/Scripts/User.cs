public class User {

    public int id;
    public string name;
    public int score;

    public User(int a, string b, int c)
    {
        id = a;
        name = b;
        score = c;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", name, score);
    }
}
