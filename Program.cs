using System;
using System.Text;
using System.Collections.Generic;
using CsvHelper;
using System.IO;

namespace mergemail
{
    public class TinySubscriber
    {
        public string Email { get; set; }
        public string SubscribeDate { get; set; }
        public string Notes { get; set; }
    }
    public class Subscriber
    {
        public string Contact { get; set; }
        public string Group { get; set; }
        public string TinyStatus { get; set; }
        public string TinyLang { get; set; }
        public string TinySubscribeDate { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Lang { get; set; }
        public string Tags { get; set; }
        public string ProfileLinkedIn { get; set; }
        public string Phone { get; set; }
    }
    class Program
    {
        static void Main(string[] args)  {
            var tinyFrFileInput = "c:\\tmp\\tinyletter_contacts_2019_03_5_fr.csv";
            var tinyEnFileInput = "c:\\tmp\\tinyletter_contacts_2019_03_5_en.csv";
            var contactFileInput = "c:\\tmp\\ContactsAll.csv";
            
            // Reading French Tiny subscriber csv
            var tinyFrDictionary = new Dictionary<string, string>();
            using (var tinyFrReader = new StreamReader(tinyFrFileInput, Encoding.UTF8))
            using (var tinyFrCsv = new CsvReader(tinyFrReader))
            {
                var tinySubscribers = tinyFrCsv.GetRecords<TinySubscriber>();
                foreach (TinySubscriber subscriber in tinySubscribers) 
                {
                    subscriber.Email = subscriber.Email.ToLower();
                    tinyFrDictionary.Add(subscriber.Email, subscriber.SubscribeDate);                
                } 
            }

            // Reading English Tiny subscriber csv
            var tinyEnDictionary = new Dictionary<string, string>();
            using (var tinyEnReader = new StreamReader(tinyEnFileInput, Encoding.UTF8))
            using (var tinyEnCsv = new CsvReader(tinyEnReader))
            {
                var tinySubscribers = tinyEnCsv.GetRecords<TinySubscriber>();
                foreach (TinySubscriber subscriber in tinySubscribers) 
                {
                    subscriber.Email = subscriber.Email.ToLower();
                    tinyEnDictionary.Add(subscriber.Email, subscriber.SubscribeDate);                
                } 
            }

            // Creating output list
            var contactRecords = new List<Subscriber>();
            using (var contactReader = new StreamReader(contactFileInput, Encoding.UTF8))
            using (var contactReaderCsv = new CsvReader(contactReader))
            {
                var subscribers = contactReaderCsv.GetRecords<Subscriber>();
                foreach (Subscriber subscriber in subscribers) 
                {
                    subscriber.Email = subscriber.Email.ToLower();
                    string subscribeDate;
                    // Search French Tiny subscriber list
                    if(tinyFrDictionary.TryGetValue(subscriber.Email, out subscribeDate)) {
                        subscriber.TinyStatus = "Subscribe";
                        subscriber.TinyLang = "Fr";
                        subscriber.TinySubscribeDate = subscribeDate;
                        tinyFrDictionary.Remove(subscriber.Email);
                        Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", subscriber.FirstName, subscriber.LastName, subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang, subscriber.TinySubscribeDate);
                    }
                    else  { // cannot find                    
                        if (subscriber.TinyLang == "Fr") {
                            subscriber.TinyStatus = "Unsubscribe";
                            Console.WriteLine("{0}, {1}, {2}, {3}, {4}", subscriber.FirstName, subscriber.LastName, subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang);
                        }
                    }
                    // Search English Tiny subscriber list
                    if(tinyEnDictionary.TryGetValue(subscriber.Email, out subscribeDate)) {
                        subscriber.TinyStatus = "Subscribe";
                        subscriber.TinyLang = "En";
                        subscriber.TinySubscribeDate = subscribeDate;
                        tinyEnDictionary.Remove(subscriber.Email);
                        Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", subscriber.FirstName, subscriber.LastName, subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang, subscriber.TinySubscribeDate);
                    }
                    else  { // cannot find                    
                        if (subscriber.TinyLang == "En") {
                            subscriber.TinyStatus = "Unsubscribe";
                            Console.WriteLine("{0}, {1}, {2}, {3}, {4}", subscriber.FirstName, subscriber.LastName, subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang);
                        }
                    }

                    // Add subscriber to output list
                    contactRecords.Add(subscriber);
                } 
            }

            // writing output list to csv
            var contactFileOutput = "c:\\tmp\\Contacts"+ DateTime.Today.ToString("_yyyy_MM_dd") + ".csv";  
            using (var writer = new StreamWriter(contactFileOutput, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {    
                csv.WriteRecords(contactRecords);
            }

            // Add Tiny subscriber not in contact csv to orphan list
            var orphanRecords = new List<Subscriber>();
            foreach(KeyValuePair<string, string> entry in tinyFrDictionary)
            {
                // do something with entry.Value or entry.Key
                Subscriber subscriber = new Subscriber {
                    Email = entry.Key.ToLower(),
                    Lang = "Fr",
                    TinyStatus = "Subscribe",
                    TinyLang = "Fr",
                    TinySubscribeDate = entry.Value,
                };
                Console.WriteLine("{0}, {1}, {2}, {3}", subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang, subscriber.TinySubscribeDate);
                orphanRecords.Add(subscriber);
            }
            foreach(KeyValuePair<string, string> entry in tinyEnDictionary)
            {
                // do something with entry.Value or entry.Key
                Subscriber subscriber = new Subscriber {
                    Email = entry.Key.ToLower(),
                    Lang = "En",
                    TinyStatus = "Subscribe",
                    TinyLang = "En",
                    TinySubscribeDate = entry.Value,
                };
                Console.WriteLine("{0}, {1}, {2}, {3}", subscriber.Email, subscriber.TinyStatus, subscriber.TinyLang, subscriber.TinySubscribeDate);
                orphanRecords.Add(subscriber);
            }
            // writing orphan list to csv
            var orphanFileOutput = "c:\\tmp\\Orphans"+ DateTime.Today.ToString("_yyyy_MM_dd") + ".csv";  
            using (var writer = new StreamWriter(orphanFileOutput, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {    
                csv.WriteRecords(orphanRecords);
            }
        }
    }

}
