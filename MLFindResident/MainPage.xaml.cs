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
using MLFindResident.Properties;
using static System.Net.Mime.MediaTypeNames;
//using ObjCRuntime;
using System.Resources;
namespace MLFindResident
{

    public class Person //PersonDto replacement
    {
        //public int Id { get; set; }
        public string personName { get; set; }
        public string receiverUnit { get; set; }
        public string receiverAddress { get; set; }

        public Person(string Name, string receiverUnit, string receiverAddress)
        {
            this.personName = Name;
            this.receiverUnit = receiverUnit;
            this.receiverAddress = receiverAddress;
        }
       public Person(string Name)
        {
            this.personName = Name;
        }
       public Person() { }
    }

    public partial class MainPage : ContentPage
	{
        ResourceManager resourceManager = new ResourceManager("MLFindResident.Properties.Resources", 
            typeof(MainPage).Assembly);
        int count = 0;
        List<Person> peopleList; //read all 
        List<Person> receiverList;
        public MainPage()
		{
			InitializeComponent();



            //string jsonString = File.ReadAllText(@"C:\Users\ED777\source\repos\MLFindResident\MLFindResident\Resources\Data\persons.json");
            string jsonString = File.ReadAllText(resourceManager.GetString("persons.json"));
            peopleList = JsonConvert.DeserializeObject<List<Person>>(jsonString);
            jsonString = File.ReadAllText(resourceManager.GetString("receivers.json"));
            receiverList = JsonConvert.DeserializeObject<List<Person>>(jsonString);

            for(int i = 0;i<peopleList.Count;i++)
            {
                peopleList[i].receiverUnit = receiverList[i].receiverUnit;
                peopleList[i].receiverAddress = receiverList[i].receiverAddress;
            }

            //ResidentListView.ItemsSource = peopleList;

            //ocr test Person
            peopleList.Add(new Person("Ashley Flores", "SPRING OAK DR 00035", "PORT JEFFERSON, NY 11777\r\nUNITED STATES"));

        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            string file_path = "scan";
            file_path += count;

            string ocr_input = File.ReadAllText(resourceManager.GetString(file_path));

            List <Person> CopyList = new List<Person>();
            foreach (var item in peopleList)
            {
                CopyList.Add(new Person
                {
                    personName = item.personName,
                    receiverAddress = item.receiverAddress,
                    receiverUnit = item.receiverUnit,
                });
            }


            List<(Person, double)> similarities = ComputeSimilarities(ocr_input, CopyList);



            //List<(Person, double)> similarities = ComputeSimilarities(UserInputEditor.Text, peopleList);

            similarities.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            similarities.Reverse();
            //ResidentListView.ItemsSource = similarities.GetRange(0,5);
            List<Person> matches = similarities.Select((x, y) => x.Item1).ToList();
            for (int i =0;i<matches.Count;i++) 
            {
                    matches[i].personName += " - " + similarities[i].Item2;
            }
            ResidentListView.ItemsSource = matches.GetRange(0,5);

            CounterBtn.Text = "scan" + count;

            SemanticScreenReader.Announce(CounterBtn.Text);
		}

  

        public static List<(Person, double)> ComputeSimilarities(string userInput, ICollection<Person> persons)
        {
            List<(Person, double)> similarities = new List<(Person, double)>();

            //string input_pattern = @"\w+\s\w+";
            //Regex inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);

            //Match m = inputRegex.Match(userInput);

            //string input_for_search = m.Value;

            //input_pattern = @"\d+";
            //inputRegex = new Regex(input_pattern, RegexOptions.IgnoreCase);
            //m = inputRegex.Match(userInput);

            //input_for_search += m.Value; // convert userInput to the following format: Name Surname 12345 (12345 - zip code)



            if (persons != null && persons.Count > 0)
            {
                foreach (var person in persons)
                {

                    string pattern = @"\d+";
                    Regex zipRegex = new Regex(pattern, RegexOptions.IgnoreCase);

                    Match m = zipRegex.Match(person.receiverUnit);

                    string searchText = (person.personName + " " + m.Value); // search text: Name Surname 12345

                    int distance = ComputeLevenshteinDistance(userInput.ToLower(), searchText.ToLower());

                    double maxLength = Math.Max(userInput.Length, searchText.Length);

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
