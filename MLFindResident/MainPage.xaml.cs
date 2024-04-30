//using AddressBook;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Debug;
namespace MLFindResident
{
    public class Person //PersonDto replacement
    {
        //public int Id { get; set; }
        public string personName { get; set; }
        public string receiverUnit { get; set; }
        public string receiverAddress { get; set; }

        Person(string Name, string receiverUnit, string receiverAddress)
        {
            this.personName = Name;
            this.receiverUnit = receiverUnit;
            this.receiverAddress = receiverAddress;
        }
        Person(string Name)
        {
            this.personName = Name;
        }
        Person() { }
    }

    public partial class MainPage : ContentPage
	{
		int count = 0;
        List<Person> peopleList; //read all 
        List<Person> receiverList;
        public MainPage()
		{
			InitializeComponent();



            string jsonString = File.ReadAllText(@"C:\Users\ED777\source\repos\MLFindResident\MLFindResident\Resources\Data\persons.json");
            
            peopleList = JsonConvert.DeserializeObject<List<Person>>(jsonString);
            jsonString = File.ReadAllText(@"C:\Users\ED777\source\repos\MLFindResident\MLFindResident\Resources\Data\receivers.json");
            receiverList = JsonConvert.DeserializeObject<List<Person>>(jsonString);

            for(int i = 0;i<peopleList.Count;i++)
            {
                peopleList[i].receiverUnit = receiverList[i].receiverUnit;
                peopleList[i].receiverAddress = receiverList[i].receiverAddress;
            }

            //ResidentListView.ItemsSource = peopleList;

        }

        private void OnCounterClicked(object sender, EventArgs e)
		{
            List<(Person, double)> similarities = ComputeSimilarities(UserInputEditor.Text, peopleList);

            similarities.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            similarities.Reverse();
            //ResidentListView.ItemsSource = similarities.GetRange(0,5);
            List<Person> matches = similarities.Select((x) => x.Item1).ToList();
            ResidentListView.ItemsSource = matches.GetRange(0,5);
            SemanticScreenReader.Announce(CounterBtn.Text);
		}

  

        public static List<(Person, double)> ComputeSimilarities(string userInput, ICollection<Person> persons)
        {
            List<(Person, double)> similarities = new List<(Person, double)>();

            string input_pattern = @"\w+\s\w+";
            Regex inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);

            Match m = inputRegex.Match(userInput);

            string input_for_search = m.Value;

            input_pattern = @"\d+";
            inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);
            m = inputRegex.Match(userInput);

            input_for_search += m.Value; // convert userInput to the following format: Name Surname 12345 (12345 - zip code)

            if (persons != null && persons.Count > 0)
            {
                foreach (var person in persons)
                {

                    string pattern = @"\d+";
                    Regex zipRegex = new Regex(pattern, RegexOptions.IgnoreCase);

                    m = zipRegex.Match(person.receiverUnit);

                    string searchText = (person.personName + " " + m.Value); // search text: Name Surname 12345

                    int distance = ComputeLevenshteinDistance(input_for_search, searchText.ToLower());

                    double maxLength = Math.Max(input_for_search.Length, searchText.Length);

                    double similarityPercentage = 1.0 - (distance / maxLength);

                    


                    similarities.Add((person, similarityPercentage));
                }
            }

            return similarities;
        }
        private static int ComputeLevenshteinDistance(string source, string target)
        {
            int sourceLength = source.Length;
            int targetLength = target.Length;

            int[,] distance = new int[sourceLength + 1, targetLength + 1];

            for (int i = 0; i <= sourceLength; i++)
                distance[i, 0] = i;

            for (int j = 0; j <= targetLength; j++)
                distance[0, j] = j;

            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost
                    );
                }
            }

            return distance[sourceLength, targetLength];
        }
    }

}
