namespace Horse.Scripts.Models;

public enum Mood
{
    VeryBad = 0,
    Bad = 1 ,
    Normal = 2,
    Good = 3,
    VeryGood = 4
}

public static class MoodState
{
    public static float GetMoodMult(this Mood m)
    {
        return m switch
        {
            Mood.VeryBad => 0.7f,
            Mood.Bad => 0.8f,
            Mood.Normal => 1.0f,
            Mood.Good => 1.2f,
            Mood.VeryGood => 1.35f
        };
    }
}