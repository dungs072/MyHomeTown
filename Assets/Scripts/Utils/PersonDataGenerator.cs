using System.Text;

public class PersonDataGenerator
{
    private static readonly string[] consonantClusters = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "v", "w", "z",
                                                            "ch", "sh", "th", "st", "br", "cr", "dr", "fr", "gr", "pr", "tr", "cl", "gl", "pl", "sl" };

    private static readonly string[] vowelClusters = { "a", "e", "i", "o", "u", "ai", "ea", "ie", "ou", "oo" };

    private static readonly string[] suffixes = { "", "son", "ton", "ford", "man", "ley", "wood", "field", "berg", "well", "ham" };

    public static string GenerateName()
    {
        int length = UnityEngine.Random.Range(2, 4);
        return GenerateName(length);
    }

    public static string GenerateName(int syllables = 2)
    {
        StringBuilder nameBuilder = new();

        for (int i = 0; i < syllables; i++)
        {
            string consonant = consonantClusters[UnityEngine.Random.Range(0, consonantClusters.Length)];
            string vowel = vowelClusters[UnityEngine.Random.Range(0, vowelClusters.Length)];

            nameBuilder.Append(consonant);
            nameBuilder.Append(vowel);
        }

        // 30% chance to add a suffix
        if (UnityEngine.Random.value < 0.3f)
        {
            string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];
            nameBuilder.Append(suffix);
        }

        string name = nameBuilder.ToString();
        name = char.ToUpper(name[0]) + name.Substring(1);
        return name;
    }
    public static int GenerateAge(int minAge = 18, int maxAge = 65)
    {
        return UnityEngine.Random.Range(minAge, maxAge + 1);
    }
}