using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Moment3
{
    public class Blog
    {
        // Lista för att lagra inlägg
        private List<Post> posts = new List<Post>();

        // Laddar fil med inlägg när klassen initieras
        public Blog()
        {
            if (File.Exists(@"blogposts.data") == true)
            {
                // Formatter för att deserialisera innehållet i filen
                IFormatter formatter = new BinaryFormatter();
                // Öppnar filestream för att läsa filer
                Stream stream = new FileStream(@"blogposts.data", FileMode.Open, FileAccess.Read);
                // Loopar igenom filen och lägger till inläggen som finns i filen i listan posts
                while (stream.Position < stream.Length)
                {
                    Post obj = (Post)formatter.Deserialize(stream);
                    posts.Add(obj);
                }
                // Stänger streamen
                stream.Close();
            }
        }
        // Lägga till nytt inlägg
        public Post addPost(Post post)
        {
            posts.Add(post);
            marshall();
            return post;
        }
        // Ta bort inlägg vid specifierat index
        public int delPost(int ind)
        {
            posts.RemoveAt(ind);
            marshall();
            return ind;
        }
        // Hämta inlägg
        public List<Post> getPosts()
        {
            return posts;
        }
        // Spara inlägg
        private void marshall()
        {
            IFormatter formatter = new BinaryFormatter();
            // Öppnar en filestream, men med i detta fall för att skriva till en fil
            Stream stream = new FileStream(@"blogposts.data", FileMode.Create, FileAccess.Write);
            foreach(Post obj in posts)
            {
                // Serialiserar datan 
                formatter.Serialize(stream, obj);
            }
            // Stänger filestreamen
            stream.Close();
        }
    }

    // Den data som ska serialiseras
    [Serializable]
    public class Post
    {
        private string name;
        private string data;
        // Namnet som är kopplad till ett inlägg
        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }
        // Själva inlägget
        public string Data
        {
            set { this.data = value; }
            get { return this.data; }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Skapar en ny instans av klassen blog
            Blog blog = new Blog();
            // Variabel för att visa inläggens index
            int i = 0;
            // "Program-loop", körs tills programmet avslutas
            while (true)
            {
                // Rensar konsollen och skriver ut alternativ för användaren
                Console.Clear();
                Console.WriteLine("1. Lägg till inlägg");
                Console.WriteLine("2. Ta bort inlägg");
                Console.WriteLine("X. Avsluta");

                i = 0;
                // Skriver ut inläggen som finns lagrade
                foreach(Post post in blog.getPosts())
                {
                    Console.WriteLine("[" + i++ + "]. Användare: " + post.Name + " - Inlägg: " + post.Data);
                }
                // Lagrar inmatning från användare och konverterar svaret till gemener
                string input = Console.ReadLine().ToLower();
                // Switch-sats för att hantera inmatningen
                switch (input)
                {
                    // Lägga till nytt
                    case "1":
                        bool correct = false;
                        // Loop för att kunna mata in information igen ifall man skrivit fel första gången
                        while (!correct)
                        {
                            // Inmatning av namn åt den som skapat inlägget
                            Console.Write("Ange ditt namn: ");
                            string name = Console.ReadLine();
                            // Kollar så att inte namnet inte är tomt
                            if (name.Length > 0)
                            {
                                while (!correct)
                                {
                                    Console.Write("Ange ditt inlägg: ");
                                    string data = Console.ReadLine();
                                    if (data.Length > 0)
                                    {
                                        // Skapar en instans av klassen Post
                                        Post obj = new Post();
                                        // Tilldelar namn och innbehåll utifrån klassen
                                        obj.Name = name;
                                        obj.Data = data;
                                        // Använder klassen posts metod addPost för att spara inlägget
                                        blog.addPost(obj);
                                        // Sätter boolen correct till true för att komma ur loopen
                                        correct = true;
                                    }
                                    // Felmeddelande
                                    else
                                    {
                                        Console.WriteLine("Ditt inlägg måste minst vara ett tecken långt.");
                                    }
                                }
                            }
                            // Felmeddelande
                            else
                            {
                                Console.WriteLine("Ditt namn måste minst vara ett tecken långt.");
                            }
                        }
                        break;
                    // Ta bort
                    case "2":
                        int ind;
                        correct = false;
                        // Kollar ifall det finns inlägg att ta bort
                        if(blog.getPosts().Count > 0)
                        {
                            while (!correct)
                            {
                                Console.Write("Ange index för det inlägg som du vill radera: ");
                                // Kollar ifall inmatningen är möjligt att tolka som en integer, ifall den är det sparas den i variabeln ind
                                if (int.TryParse(Console.ReadLine(), out ind))
                                {
                                    // Felhantering ifall man skriver en siffra mindre än 0, eller mer än antalet inlägg som finns i listan
                                    if (ind < blog.getPosts().Count && ind >= 0)
                                    {
                                        // Rimlig siffra är angiven
                                        // Anropar metoden delPost i klassen blog för att ta bort inlägg
                                        blog.delPost(ind);
                                        // Går ur loopen
                                        correct = true;
                                    }
                                    // Felmeddelande
                                    else
                                    {
                                        Console.WriteLine("Kunde inte hitta inlägg med index " + ind + ". Försök igen.");
                                    }

                                }
                                // Felmeddelande
                                else
                                {
                                    Console.WriteLine("Du måste ange en siffra/nummer.");
                                }
                            }
                        }
                        // Felmeddelande
                        else
                        {
                            Console.WriteLine("Det finns för närvarande inga inlägg. Tryck valfri knapp för att gå tillbaka.");
                            Console.ReadKey();
                        }
                        break;
                    // Avsluta
                    case "x":
                        // Avslutar applikationen
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
