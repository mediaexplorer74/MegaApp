using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MegaApp.Model; // !!
using CG.Web.MegaApiClient; // !
using Windows.Storage;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MegaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Model.DataItem> MegaItems;

        static string CurrectFCategory;     // текущая ф. категория (группа файл. обьектов, каталог)
        static string CurrectFSubcategory;  // текущая ф. подкатегория (подгруппа, подкаталог)

        static int DirectoryCount;          // counter для каталогов
        int SubdirectoryCount;              // counter для каталогов

        static string megaStorageURL;       // Mega.nz root "directory" URL
        //static bool isBusy;                 // Status of Task 

        public MainPage()
        {
            this.InitializeComponent();
            
            MegaItems = new ObservableCollection<Model.DataItem>();
        }

       
        // Preprocess: Collect and prepare Mega.nz data...
        private void Preprocess(string MegaSharedFolderURL)
        {
            Contact.MegaCount = 0; // counter init 

            CurrectFCategory = "<Root>"; // 'parent' group (file category) init  
            DirectoryCount = 0;

            CurrectFSubcategory = "<Root>"; // 'parent' sub-group (sub-folder) init  
            SubdirectoryCount = 0;

            //ListView01.Items.Clear(); // clear ListView 


            // "no login"
            MegaClient.client.LoginAnonymous(); // "Users only" =)

            // GetNodes retrieves all files/folders metadata from Mega
            // so this method can be time consuming

            //IEnumerable<INode> nodes = MegaClient.client.GetNodes();


            // Reconstruct URI
            Uri folderLink = new Uri(MegaSharedFolderURL);

            // Get nodes (folders and files in level 0)
            MegaClient.nodes = MegaClient.client.GetNodesFromLink(folderLink);

            // Get parent node 
            MegaClient.parent = MegaClient.nodes.Single(n => n.Type == NodeType.Root);

            // Get sub-nodes (folders and files in level 1, 2, etc...)
             ProcessAllSubNodes(MegaClient.nodes, MegaClient.parent);

            // Logout
            MegaClient.client.Logout();

        }//Preprocess() end



        // Preprocess: Collect and prepare Mega.nz data...
        private async Task PreprocessAsync(string MegaSharedFolderURL)
        {
            Contact.MegaCount = 0; // counter init 

            Contact.MegaCount = 0; // counter init 

            CurrectFCategory = "<Root>"; // 'parent' group (file category) init  
            DirectoryCount = 0;

            CurrectFSubcategory = "<Root>"; // 'parent' sub-group (sub-folder) init  
            SubdirectoryCount = 0;

            //ListView01.Items.Clear(); // clear ListView 


            // "no login"
            await MegaClient.client.LoginAnonymousAsync(); // "Users only" =)

            // GetNodes retrieves all files/folders metadata from Mega
            // so this method can be time consuming

            //IEnumerable<INode> nodes = MegaClient.client.GetNodes();


            // Reconstruct URI
            Uri folderLink = new Uri(MegaSharedFolderURL);

            // Get nodes (folders and files in level 0)
            MegaClient.nodes = MegaClient.client.GetNodesFromLink(folderLink);

            // Get parent node 
            MegaClient.parent = MegaClient.nodes.Single(n => n.Type == NodeType.Root);

            // Get sub-nodes (folders and files in level 1, 2, etc...)
            ProcessAllSubNodes(MegaClient.nodes, MegaClient.parent);

            // Logout
            await MegaClient.client.LogoutAsync();

        }//PreprocessAsync() end

        // Process All Sub Nodes (Level 1, 2, etc...)
        void ProcessAllSubNodes(IEnumerable<INode> nodes, INode parent, int level = 0)
        {
            
            IEnumerable<INode> children = nodes.Where(x => x.ParentId == parent.Id);

            foreach (INode child in children)
            {
               
                if (child.Type.ToString() == "Directory")
                {

                    // *** FOLDER object found ***

                    // New Category found? Is Level = 0?? OK!
                    if ((child.Name != CurrectFCategory && child.Size == 0) && (level == 0))
                    {
                        // Change category !
                        CurrectFCategory = $"{child.Name.ToUpper()}";

                        DirectoryCount++; // текущая категория файлов указатель на группу)
                    }

                    // New Category found? Is Level = 0?? OK!
                    if ((child.Name != CurrectFSubcategory && child.Size == 0) && (level <= 1))
                    {
                        // Change category !
                        CurrectFSubcategory = $"{child.Name.ToUpper()}";

                        SubdirectoryCount++; // текущая категория файлов указатель на группу)
                    }


                }
                else
                {
                    //*** FILE object found ***

                   
                    Contact.MegaFName[Contact.MegaCount]
                    = $"{child.Name}";


                    Contact.MegaFCategory[Contact.MegaCount]
                        = $"{CurrectFCategory}";

                    Contact.MegaFSubcategory[Contact.MegaCount]
                        = $"{CurrectFSubcategory}";


                    Contact.MegaFKey[Contact.MegaCount]
                        = child.Id;

                    // Store ALL INode object !
                    MegaClient.arNodes[Contact.MegaCount] = child;

                    
                    Contact.MegaCount++;

                }




                // Если нашли "потомков" класса "Папка", повторяем маневры... 
                if (child.Type == NodeType.Directory)
                {
                    ProcessAllSubNodes(nodes, child, level + 1);
                }
            }
        }//ProcessAllSubNodes



        // Hamburger Menu Button Click handler
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }


        // Hamburger Menu "List" (ListBox) Item Selection handler
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home.IsSelected)
            {
                MegaManager.GetAllNews(MegaItems);
                TitleTextBlock.Text = $"Home";
            }
            else if (APPX.IsSelected)
            {
                MegaManager.GetItemsbyCategory("APPX", MegaItems);
                TitleTextBlock.Text = $"APPX";
            }
            else if (APPXGames.IsSelected)
            {
                MegaManager.GetItemsbyCategory("APPX Games", MegaItems);
                TitleTextBlock.Text = $"APPX Games";
            }
            else if (Camera.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Camera", MegaItems);
                TitleTextBlock.Text = $"Camera";
            }
            else if (Dependencies.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Dependencies", MegaItems);
                TitleTextBlock.Text = $"Dependancies";
            }
            else if (Extensions.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Extensions", MegaItems);
                TitleTextBlock.Text = $"Extensions";
            }
            else if (Extras.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Extras", MegaItems);
                TitleTextBlock.Text = $"Extras";
            }
            else if (MicrosoftApps.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Microsoft", MegaItems);
                TitleTextBlock.Text = $"Microsoft apps";
            }
            else if (TelegramGroup.IsSelected)
            {
                MegaManager.GetItemsbyCategory("Telegram group", MegaItems);
                TitleTextBlock.Text = $"Telegram group";
            }
            else if (XAP.IsSelected)
            {
                MegaManager.GetItemsbyCategory("XAP", MegaItems);
                TitleTextBlock.Text = $"XAP";
            }
            //else if (Productivity.IsSelected)
            //{
            //    MegaManager.GetItemsbyCategory("Productivity", MegaItems);
            //    TitleTextBlock.Text = $"Productivity";
            //}
            else if (XAPGames.IsSelected)
            {
                MegaManager.GetItemsbyCategory("XAP Games", MegaItems);
                TitleTextBlock.Text = $"XAP Games";
            }
            //else if (Tweaks.IsSelected)
            //{
            //    MegaManager.GetItemsbyCategory("Tools and Tweaks", MegaItems);
            //    TitleTextBlock.Text = $"Tools and Tweaks";
            //}
            //else if (Travel.IsSelected)
            //{
            //    MegaManager.GetItemsbyCategory("Travel, Weather, News, Sports, Navigation", MegaItems);
            //    TitleTextBlock.Text = "Travel, Weather, etc.";
            //}
            //else if (W10M.IsSelected)
            //{
            //    MegaManager.GetItemsbyCategory("W10M PC Tools", MegaItems);
            //    TitleTextBlock.Text = "W10M PC Tools";
            //}
            //else if (XBox.IsSelected)
            //{
            //    MegaManager.GetItemsbyCategory("Xbox and Non Xbox live Games", MegaItems);
            //    TitleTextBlock.Text = "Xbox and Non Xbox live Games";
            //}
            
            SearchBox.Text = ""; // clear SearchBox =)

        }//


        // "Text is searchbar changed" event handler
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var FItems = MegaManager.getMegaItems();

            if (SearchBox.Text != "")
            {
                // All 
                FItems = (from s in FItems where s.Headline.Contains(SearchBox.Text) select s).ToList();//LINQ Query  
            }
            else
            {
                // do nothing ;)
            }

            MegaItems.Clear();
            FItems.ForEach(p => MegaItems.Add(p));
        }//sbar_TextChanged


        // "Search button click" event handler
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var FItems = MegaManager.getMegaItems();

            if (SearchBox.Text != "")
            {
                // All 
                FItems = (from s in FItems where s.Headline.Contains(SearchBox.Text) select s).ToList();//LINQ Query  
            }
            else
            {
                // do nothing ;)
            }

            MegaItems.Clear();
            FItems.ForEach(p => MegaItems.Add(p));
        }//


        // ** FIRST PROCESS ** 
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;


            // ******************************************************************

            // phase 1 
            // !!
            //MegaItems = new ObservableCollection<Model.DataItem>();

            //TEST 0
            //megaStorageURL = "https://mega.nz/#F!SYtigRjB!EhNuflDF9fefSXuolgn0Rw"; // Old W10M Group repo (obsolete/disabled)

            //TEST 1
            //megaStorageURL = "https://mega.nz/folder/YdlWiaxD#7qcjO0mtYukRBCuDzoIwGA"; // ME repo (TEST)

            //Astoria appx repo
            megaStorageURL = "https://mega.nz/folder/om4kkKCa#Nv7RCZOhCXlCfzRnKpfCXg";//"https://mega.nz/folder/SKZxnQAR#EvlQqjMIVQwoxcje9r-jAw"; // New W10M Group repo (PROD)

            // phase 2 
            // Collect and prepare Mega.nz data...
            await PreprocessAsync(megaStorageURL); // non-async

          

            MegaManager.GetAllNews(MegaItems);

            // HOME is pre-select category (when 1st page load) =)
            Home.IsSelected = true;

            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;

        }//Page_Loaded


        // MegaItem_Tapped
        private async void MegaItemGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Phase 1 

            //Temp: "Click / Tap " handle with Diagnostics output (message box only)

            int idx = -1;

            try
            {
                var T1 = (DataItem)e.ClickedItem;

                idx = T1.Id; // int
            }
            catch (Exception Exp1)
            {
                // Little Error Diagnostics
                Debug.WriteLine(Exp1.Message);
            }

            // FoolishProof *******************
            //1
            if (idx < 0 || idx > 10000) return;

            //2
            //if (isBusy) return;

            //3 

            // Modernize it! =)
            if (MegaClient.arNodes[idx].Type.ToString() != "File") return;
            // ********************************

            /*
            ContentDialog SimpleDialog = new ContentDialog()
            {
                Title = "Operation Request",
                Content = "Download into Image folder & launch \n" +
                MegaClient.arNodes[idx].Name +
                " \n" +
                "[file size: " +
                MegaClient.arNodes[idx].Size.ToString() + " bytes\n"
                + "mod. date: " +
                MegaClient.arNodes[idx].ModificationDate +
                "] ?"
                ,
                PrimaryButtonText = "ОК",
                SecondaryButtonText = "Cancel"
            };
            */

            /*
            bool Doubling = false;
            bool FirstFound = false;
            bool SecondFound = false;

            // find index key...
            for(var i = 0; i < 10000; i++)
            {
                if (MegaClient.arNodes[i].Id == Contact.MegaFKey[sel_idx])
                {
                    idx = i;

                    if (!FirstFound)
                    {
                        FirstFound = true;

                    }

                    if (FirstFound)
                    {
                        if (!SecondFound)
                        {
                            SecondFound = true;
                            Doubling = true;
                        }
                    }
                }
            }
            */

            ContentDialog SimpleDialog = new ContentDialog()
            {
                Title = "Operation Request",
                Content = "Do you want to download (into Image folder) & launch file \n" +
               MegaClient.arNodes[idx].Name +
               " \n" +
               "[file size: " +
               MegaClient.arNodes[idx].Size.ToString() + " bytes\n"
               + "mod. date: " +
               MegaClient.arNodes[idx].ModificationDate +
               "] ?"
               ,
                PrimaryButtonText = "ОК",
                SecondaryButtonText = "Cancel"
            };
            ContentDialogResult result = await SimpleDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Change status...
                //filesList.Text = "Downloading...";
            }
            else
            {
                //if (result == ContentDialogResult.Secondary)


                // Change status to empty
                //filesList.Text = "";
                return;
            }

            // Phase 2

            //"MEGA login"
            MegaClient.client.LoginAnonymous();



            // Скачиваем файл и получаем полный путь к нему...
            string FullLSPath = MegaClient.client.DownloadFile
            (
                MegaClient.arNodes[idx],
                MegaClient.arNodes[idx].Name
            );

            //"MEGA logout"
            MegaClient.client.Logout();


            

            // 1 Copy to Image folder
            
            // 
            string ShortFName = MegaClient.arNodes[idx].Name;

            StorageFolder folder = ApplicationData.Current.LocalFolder;//KnownFolders.VideosLibrary;

            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

            // Пробегаемся по всем файлам в хранилище... ищем скачанный файлик
            // TODO: че-то придумать с избавлением от цикла!!!
            foreach (StorageFile ffile in files)
            {
                if (ffile.Path == FullLSPath)
                {
                    // DEBUG and TEST ONLY -- comment after all !
                    //Debug.WriteLine(FullLSPath);

                    StorageFolder fLibrary = await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);


                    StorageFile fileCopy = await ffile.CopyAsync(fLibrary, ShortFName, NameCollisionOption.ReplaceExisting);

                    //подчищаем мусор =)
                    ffile.DeleteAsync();
                }
            }


            // 2  File Launching

            // Path to the file in the app package to launch
            string appxFile = ShortFName;


            // Detect current Local Storage...
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder imageFolder =
                await KnownFolders.GetFolderForUserAsync(null, KnownFolderId.PicturesLibrary);

            // Construct full path...
            //string FullPath = localFolder.Path + "\\" + appxFile;

            var appFile = await imageFolder.GetFileAsync(appxFile);

            if (appFile != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(appFile);
            }


        }//MegaItemGrid_ItemClick end

    }// partial class end   

}// namespace end
