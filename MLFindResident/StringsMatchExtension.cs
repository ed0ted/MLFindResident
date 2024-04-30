//using System.Text.RegularExpressions;

//using static System.Net.Mime.MediaTypeNames;





//public class StringsMatchExtension
//{
//    public static List<(PersonDto, double)> ComputeSimilarities(string userInput, ICollection<PersonDto> persons)
//    {
//        List<(PersonDto, double)> similarities = new List<(PersonDto, double)>();

//        string input_pattern = @"\w+\s\w+";
//        Regex inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);

//        Match m = inputRegex.Match(userInput);

//        string input_for_search = m.Value;

//        input_pattern = @"\d+";
//        inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);
//        m = inputRegex.Match(userInput);

//        input_for_search += m.Value; // convert userInput to the following format: Name Surname 12345 (12345 - zip code)

//        if (persons != null && persons.Count > 0)
//        {
//            foreach (var person in persons)
//            {
//                //var matches = str.match(/\b\d{ 5}\b / g);

//                string pattern = @"\d+";
//                Regex zipRegex = new Regex(pattern, RegexOptions.IgnoreCase);

//                m = zipRegex.Match(person.receiverUnit);

//                string searchText = (person.Name + " " + m.Value); // search text: Name Surname 12345

//                int distance = ComputeLevenshteinDistance(input_for_search, searchText.ToLower());

//                double maxLength = Math.Max(input_for_search.Length, searchText.Length);

//                double similarityPercentage = 1.0 - (distance / maxLength);

//                similarities.Add((person, similarityPercentage));
//            }
//        }

//        return similarities;
//    }
//    private static int ComputeLevenshteinDistance(string source, string target)
//    {
//        int sourceLength = source.Length;
//        int targetLength = target.Length;

//        int[,] distance = new int[sourceLength + 1, targetLength + 1];

//        for (int i = 0; i <= sourceLength; i++)
//            distance[i, 0] = i;

//        for (int j = 0; j <= targetLength; j++)
//            distance[0, j] = j;

//        for (int i = 1; i <= sourceLength; i++)
//        {
//            for (int j = 1; j <= targetLength; j++)
//            {
//                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

//                distance[i, j] = Math.Min(
//                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
//                    distance[i - 1, j - 1] + cost
//                );
//            }
//        }

//        return distance[sourceLength, targetLength];
//    }
//}