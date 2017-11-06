using Windows.UI.Xaml;
using System.Threading.Tasks;
using AnyTimeT10.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using AnyTimeT10.DAL;
using Microsoft.EntityFrameworkCore;
using Windows.Storage;
using System.Collections.Generic;
using AnyTimeT10.Models;

namespace AnyTimeT10
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            using (var db = new DatabaseContext())
            {
                db.Database.Migrate();
                PrepareCategories();
            }

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion
        }

        public async System.Threading.Tasks.Task FetchDefaultImagesAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
                 new Uri("ms-appx:///Assets/Avatars/emptyAvatar.png"));
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder avatarFolder = await localFolder.CreateFolderAsync("Avatars", CreationCollisionOption.OpenIfExists);
            await file.CopyAsync(avatarFolder, "emptyAvatar.png", NameCollisionOption.ReplaceExisting);

        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async System.Threading.Tasks.Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // TODO: add your long-running task here
            await FetchDefaultImagesAsync();
            await NavigationService.NavigateAsync(typeof(Views.LoginPage));

        }

        public void PrepareCategories()
        {
            using (var db = new DatabaseContext())
            {
                if (!db.TaskCategories.Any())
                {
                    List<TaskCategory> categoryList = new List<TaskCategory>();
                    categoryList.Add(new TaskCategory { Name = "Social", Icon = "&#xE716;", ColorCode = "#aabd8c" });
                    categoryList.Add(new TaskCategory { Name = "Education", Icon = "&#xE8F1;", ColorCode = "#4c6085" });
                    categoryList.Add(new TaskCategory { Name = "Business", Icon = "&#xE774;", ColorCode = "#381d2a" });
                    categoryList.Add(new TaskCategory { Name = "Household", Icon = "&#xE719;", ColorCode = "#f39b6d" });
                    categoryList.Add(new TaskCategory { Name = "Entertainment", Icon = "&#xE76E;", ColorCode = "#1b9aaa" });

                    db.TaskCategories.AddRange(categoryList);
                    db.SaveChanges();
                }
            }

        }


    }
}
