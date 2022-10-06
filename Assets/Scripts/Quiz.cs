[System.Serializable]
public class Quiz
{
    public string question;
    public bool answer;
    public Quiz(string question, bool answer)
    {
        this.question = question;
        this.answer = answer;
    }
}