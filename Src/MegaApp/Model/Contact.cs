using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CG.Web.MegaApiClient; // !
//using Windows.Storage; // !

namespace MegaApp.Model
{
    public class Contact
    {
        private static Random random = new Random();

        // Счетчик 
        public static int MegaCount;

        // ***********************************************************

        // Массив названий файлов
        public static string[] MegaFName = new string[10000];
        
        // Массив ключей (маршрутов) файлов
        public static string[] MegaFKey = new string[10000];

        // Массив групп (категорий) файловых обьектов
        public static string[] MegaFCategory = new string[10000];

        // Массив подгрупп (субкатегорий) файловых обьектов
        public static string[] MegaFSubcategory = new string[10000];


        // ***********************************************************

        #region Properties

        private string initials;

        public string Initials
        {
            get
            {
                if (initials == string.Empty && FirstName != string.Empty && LastName != string.Empty)
                {
                    initials = FirstName[0].ToString() + LastName[0].ToString();
                }
                return initials;
            }
        }

        private string name;

        public string Name
        {
            get
            {
                if (name == string.Empty && FirstName != string.Empty && LastName != string.Empty)
                {
                    name = FirstName + " " + LastName;
                }
                return name;
            }
        }

        private string lastName;

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                initials = string.Empty; // force to recalculate the value 
                name = string.Empty; // force to recalculate the value 
            }
        }

        private string firstName;

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                initials = string.Empty; // force to recalculate the value 
                name = string.Empty; // force to recalculate the value 
            }
        }

        public string Position { get; set; }

        public string PhoneNumber { get; set; }

        public string Biography { get; set; }
        #endregion

        public Contact()
        {
            // default values for each property.
            initials = string.Empty;
            name = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            Position = string.Empty;
            PhoneNumber = string.Empty;
            Biography = string.Empty;
        }
     
        #region Public Methods

        // 
        public static Contact GetNewContact(int Index)
        {
            // Random Generated - off!

            // Manual mode used
            return new Contact()
            {
                FirstName = MegaFName[Index], //"FirstName",//GenerateFirstName(),
                LastName = MegaFKey[Index], //"LastName",//GenerateLastName(),
                Biography = "Bio",//GetBiography(),
                PhoneNumber = "1234567",//GeneratePhoneNumber(),
                Position = "Dev",//GeneratePosition(),
            };
        }

        // OLD 
        public static ObservableCollection<Contact> GetContacts(int numberOfContacts)
        {
            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < numberOfContacts; i++)
            {
                contacts.Add(GetNewContact(i));
            }
            return contacts;
        }




        // New 
        /*
        public static ObservableCollection<Contact> GetMegaContacts()
        {
            // Phase 1
            // Пересчитываем число контактов 
            Contact.RecountMegaContacts();

            // Phase 2
            // определяем число контактов
            int numberOfContacts = Contact.MegaCount;// CountMegaContacts(); 

            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < numberOfContacts; i++)
            {
                contacts.Add(GetNewContact(i));
            }
            return contacts;
        }
        */


        // New - TODO
        /*
        public static ObservableCollection<Contact> GetHybridMegaContacts()
        {
            // определяем число контактов
            int numberOfContacts = Contact.MegaCount;// CountMegaContacts(); 

            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < numberOfContacts; i++)
            {
                contacts.Add(GetNewContact(i));
            }
            return contacts;
        }
        */

        // Рекурсивная процедура пробежки по всем нодам списка..
        /*
        static void DisplayNodesRecursive(IEnumerable<INode> nodes, INode parent, int level = 0)
        {
            IEnumerable<INode> children = nodes.Where(x => x.ParentId == parent.Id);
            foreach (INode child in children)
            {
                //string infos = $"- {child.Name} - {child.Size} bytes - {child.CreationDate}";
                //string infos = $"- {child.Name} [{child.Id}]";
                string infos = $"- {child.Name}";
                // Сохраняем в массив название файла
                MegaFName[MegaCount] = child.Name;

                // Сохраняем в массив ключ (маршрут) файла
                MegaFKey[MegaCount] = child.Id;


                MegaCount++; // Наращиваем счетчик
                

                //SuperWriteLine(infos.PadLeft(infos.Length + level, '\t'));

                //Console.WriteLine(infos.PadLeft(infos.Length + level, '\t'));

                if (child.Type == NodeType.Directory)
                {
                    DisplayNodesRecursive(nodes, child, level + 1);
                }
            }
        }//
        */


        //
        // Функция подсчета числа контактов из Mega.Nz
        // 
        /*
        public static void RecountMegaContacts()
        {
            // Обнуляем (инициализируем счетчик)
            MegaCount = 0;

            var client = new MegaApiClient();

            // "no login"
            client.LoginAnonymous();

            //Uri folderLink = new Uri("https://mega.nz/#F!e1ogxQ7T!ee4Q_ocD1bSLmNeg9B6kBw"); // Test
            Uri folderLink = new Uri("https://mega.nz/#F!SYtigRjB!EhNuflDF9fefSXuolgn0Rw"); // W10M

            

            // FromLink(folderLink) retrieves all files/folders metadata from Mega
            // so this method can be time consuming
            IEnumerable<INode> nodes = client.GetNodesFromLink(folderLink);

            // Get Parent node
            INode parent = nodes.Single(n => n.Type == NodeType.Root);

            DisplayNodesRecursive(nodes, parent);


            client.Logout();
            
        }
        */

        // OLD: GetContactsGrouped
        // Used in simple modes (List and Grid)
        /*
        public static ObservableCollection<GroupInfoList> GetContactsGrouped(int numberOfContacts)
        {
            ObservableCollection<GroupInfoList> groups = new ObservableCollection<GroupInfoList>();

            // QUERY SAMPLE
            var query = from item in GetContacts(numberOfContacts)
                        group item by item.LastName.Substring(0, 1).ToUpper() into g
                        orderby g.Key
                        select new { GroupName = g.Key, Items = g };

            foreach (var g in query)
            {
                GroupInfoList info = new GroupInfoList();

                info.Key = g.GroupName;

                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }

            return groups;
        }//GetContactsGrouped
        */


        #endregion

        #region Helpers

        
       
        #endregion
    }
}
