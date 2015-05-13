using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using TerraViewer.CloudCommunitiesUsersAPI;

namespace TerraViewer
{
    class CloudCommunities
    {
        public delegate IPlace getTargetDel();
        public delegate void closeContextMenuDel();
        private ToolStripMenuItem addToCommunityToolStripMenuItem = null;
        private ToolStripMenuItem newCommunityToolStripMenuItem = null;
        private ToolStripMenuItem shareOnFacebookMenuItem = null;
        private getTargetDel getTarget = null;
        private closeContextMenuDel closeContextMenu = null;
        private FolderBrowser explorePane = null;
        private object targetObject = null;
        private CloudShareProgress progressReporting = null;
        public static Dictionary<string, bool> avoidCacheDict = new Dictionary<string, bool>();

        public CloudCommunities(getTargetDel getTargetFn, closeContextMenuDel closeContextMenuFn)
            : this(getTargetFn, closeContextMenuFn, null)
        {
        }
        public CloudCommunities(object target, closeContextMenuDel closeContextMenuFn)
            : this(null, closeContextMenuFn, null)
        {
            targetObject = target;
        }
        public CloudCommunities(getTargetDel getTargetFn, closeContextMenuDel closeContextMenuFn, FolderBrowser exPane)
        {
            this.getTarget = getTargetFn;
            this.closeContextMenu = closeContextMenuFn;
            if (exPane != null)
            {
                this.explorePane = exPane;
            }
            else
            {
                this.explorePane = new FolderBrowser();
            }

            this.addToCommunityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCommunityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shareOnFacebookMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.shareOnFacebookMenuItem.Name = "shareOnFacebookMenuItem";
            this.shareOnFacebookMenuItem.Size = new System.Drawing.Size(164, 22);
            this.shareOnFacebookMenuItem.Text = Language.GetLocalizedText(-1, "Share on Facebook");
            this.shareOnFacebookMenuItem.Click += new EventHandler(shareOnFacebookMenuItem_Click);

            this.newCommunityToolStripMenuItem.Name = "newCommunityToolStripMenuItem";
            this.newCommunityToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.newCommunityToolStripMenuItem.Text = Language.GetLocalizedText(-1, "New Community...");

            this.addToCommunityToolStripMenuItem.DropDownItems.AddRange(
                new System.Windows.Forms.ToolStripItem[] { 
                    this.newCommunityToolStripMenuItem 
                });
            this.addToCommunityToolStripMenuItem.Name = "addToCommunityToolStripMenuItem";
            this.addToCommunityToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addToCommunityToolStripMenuItem.Text = Language.GetLocalizedText(-1, "Add to Community");
            this.addToCommunityToolStripMenuItem.DropDownOpening += new System.EventHandler(this.addToCommunityToolStripMenuItem_DropDownOpening);
            this.addToCommunityToolStripMenuItem.Click += new System.EventHandler(this.addToCommunityToolStripMenuItem_Click);
        }
        public static bool AvoidCache(string url, string filename)
        {
            long CommunityId = 0, ComponentId = 0;
            if (url.IndexOf(Properties.Settings.Default.WWTCommunityServer) != 0)
            {
                return false;
            }
            Match communityId = new Regex(@"CommunityId=(\d+)").Match(url);
            Match componentId = new Regex(@"ComponentId=(\d+)").Match(url);
            if (communityId.Groups.Count == 2 && long.TryParse(communityId.Groups[1].Value, out CommunityId))
            {
                if (componentId.Groups.Count != 2 || !long.TryParse(componentId.Groups[1].Value, out ComponentId))
                {
                    ComponentId = 0;
                }
                string key = CommunityId.ToString() + "-" + ComponentId.ToString();
                if (avoidCacheDict.ContainsKey(key) && avoidCacheDict[key])
                {
                    if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    avoidCacheDict.Remove(key);
                    return true;
                }
            }
            return false;
        }
        public static void SetAvoidCache(long CommunityId, long? ComponentId)
        {
            if (!ComponentId.HasValue)
            {
                ComponentId = 0;
            }
            string key = CommunityId.ToString() + "-" + ComponentId.ToString();
            avoidCacheDict[key] = true;
        }
        public static string GetTokenFromId()
        {
            return GetTokenFromId(false);
        }
        public static string GetTokenFromId(bool asParam)
        {
            return Properties.Settings.Default.LiveIdToken;
        }
        public static void ProcessSoapException(SoapException ex)
        {
            Regex errorReg = new Regex(@"<Error>([^<]+)<ErrorCode>([^<]+)</ErrorCode></Error>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match errorMatch = errorReg.Match(ex.Message);
            if (errorMatch.Success)
            {
                MessageBox.Show("Error (" + 
                    errorMatch.Groups[2].Value + "): " +
                    errorMatch.Groups[1].Value);
            }
            else
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static bool DeleteCommunityObject(object obj)
        {
            if (obj is Folder)
            {
                Folder folder = (Folder)obj;
                if (folder.MSRCommunityId > 0)
                    return DeleteCommunityObject(folder.MSRCommunityId, folder.MSRComponentId);
            }
            else
            {
                if (obj is Place)
                {
                    Place place = (Place)obj;
                    if (place.MSRCommunityId > 0 && place.MSRComponentId > 0)
                    {
                        return DeleteCommunityObject(place.MSRCommunityId, place.MSRComponentId);
                    }
                }
                else
                {
                    if (obj is Tour)
                    {
                        Tour tour = (Tour)obj;
                        if (tour.MSRCommunityId > 0 && tour.MSRComponentId > 0)
                        {
                            return DeleteCommunityObject(tour.MSRCommunityId, tour.MSRComponentId);
                        }
                    }
                }
            }
            return false;
        }
        public static bool DeleteCommunityObject(long CommunityId, long ComponentId)
        {
            try
            {
                if (CommunityId > 0)
                {
                    if (ComponentId > 0)
                    {
                        CloudCommunityComponentsAPI.apiCommunityComponents api = new CloudCommunityComponentsAPI.apiCommunityComponents();
                        long? ownerId = api.Get(CloudCommunities.GetTokenFromId(), CommunityId, ComponentId, false, false).OwnerComponentId;
                        api.Delete(CloudCommunities.GetTokenFromId(), CommunityId, ComponentId);
                        SetAvoidCache(CommunityId, ownerId.HasValue ? ownerId.Value : 0);
                        return true;
                    }
                    else
                    {
                        CloudCommunitiesAPI.apiCommunities api = new CloudCommunitiesAPI.apiCommunities();
                        api.Delete(CloudCommunities.GetTokenFromId(), CommunityId);
                        SetAvoidCache(CommunityId, 0);
                        return true;
                    }
                }
            }
            catch (SoapException ex)
            {
                CloudCommunities.ProcessSoapException(ex);
                return true; // failed but still tried
            }
            return false;
        }
        public static void UpdateCommunityObject(object obj)
        {
            try
            {
                if (obj is Folder)
                {
                    Folder folder = (Folder)obj;
                    if (folder.MSRCommunityId > 0)
                    {
                        if (folder.MSRComponentId > 0)
                        {
                            CloudCommunityComponentsAPI.apiCommunityComponents api = new CloudCommunityComponentsAPI.apiCommunityComponents();
                            api.Update(CloudCommunities.GetTokenFromId(), folder.MSRCommunityId, folder.MSRComponentId, folder.Name,
                                null, null, false, null, null);
                            SetAvoidCache(folder.MSRCommunityId, folder.MSRComponentId);
                        }
                        else
                        {
                            CloudCommunitiesAPI.apiCommunities api = new CloudCommunitiesAPI.apiCommunities();
                            api.Update(CloudCommunities.GetTokenFromId(), folder.MSRCommunityId, folder.Name, null, null, null);
                            SetAvoidCache(folder.MSRCommunityId, 0);
                        }
                    }
                }
                else
                {
                    if (obj is Place)
                    {
                        Place place = (Place)obj;
                        if (place.MSRCommunityId > 0 && place.MSRComponentId > 0)
                            AddUpdObjectToCommunity(place.MSRCommunityId, place.MSRComponentId, obj, true);
                    }
                    else
                    {
                        if (obj is Tour)
                        {
                            Tour tour = (Tour)obj;
                            if (tour.MSRCommunityId > 0 && tour.MSRComponentId > 0)
                                AddUpdObjectToCommunity(tour.MSRCommunityId, tour.MSRComponentId, obj, true);
                        }
                    }
                }
            }
            catch (SoapException ex)
            {
                CloudCommunities.ProcessSoapException(ex);
            }
        }
        private void shareOnFacebookMenuItem_Click(object sender, EventArgs e)
        {
            facebookShare shareForm = new facebookShare(targetObject ?? getTarget());
            shareForm.ShowDialog();
        }
        private void addToCommunityToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void addToCommunityToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            apiUsers api = new apiUsers();
            api.GetCommunitiesCompleted += new GetCommunitiesCompletedEventHandler(api_GetCommunitiesCompleted);
            api.GetCommunitiesAsync(GetTokenFromId(), false, true);
        }
        private void api_GetCommunitiesCompleted(object sender, GetCommunitiesCompletedEventArgs e)
        {
            api_GetCommunitiesCompleted(addToCommunityToolStripMenuItem, e, newCommunityMenu_Click, AddtoCommunityMenu_Click);
        }
        public static void api_GetCommunitiesCompleted(ToolStripMenuItem addToCommunityToolStripMenuItem,
            GetCommunitiesCompletedEventArgs e, EventHandler newCommunityMenu_Click, EventHandler AddtoCommunityMenu_Click)
        {
            addToCommunityToolStripMenuItem.DropDownItems.Clear();

            ToolStripMenuItem newCommunityMenu = new ToolStripMenuItem(Language.GetLocalizedText(-1, "New Community..."));
            newCommunityMenu.Click += new EventHandler(newCommunityMenu_Click);
            addToCommunityToolStripMenuItem.DropDownItems.Add(newCommunityMenu);
            addToCommunityToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem menuItem = addToCommunityToolStripMenuItem;
            if (e.Error == null)
            {
                if (e.Result is WWTCommunitySt[])
                {
                    addToCommunityToolStripMenuItem.Tag = e.Result;
                }
                else
                {
                    addToCommunityToolStripMenuItem.Tag = null;
                }
            }
            else
            {
                addToCommunityToolStripMenuItem.Tag = null;
            }
            CreatePickFolderMenuCommunities(menuItem, AddtoCommunityMenu_Click);
        }
        private static void CreatePickFolderMenuCommunities(ToolStripMenuItem menuItem, EventHandler AddtoCommunityMenu_Click)
        {
            if (menuItem.Tag == null)
            {
                return;
            }
            object[] Objects = null;
            if (menuItem.Tag is WWTCommunitySt[])
            {
                Objects = (WWTCommunitySt[])menuItem.Tag;
            }
            if (menuItem.Tag is WWTCommunitySt)
            {
                Objects = ((WWTCommunitySt)menuItem.Tag).components;
            }
            if (menuItem.Tag is WWTCommunityComponentSt)
            {
                Objects = ((WWTCommunityComponentSt)menuItem.Tag).components;
            }

            if (Objects != null)
            {
                foreach (object obj in Objects)
                {
                    string name = null;
                    Permission viewerPermission = Permission.NONE;
                    if (obj is WWTCommunitySt)
                    {
                        name = ((WWTCommunitySt)obj).Name;
                        viewerPermission = ((WWTCommunitySt)obj).ViewerAccess;
                    }
                    if (obj is WWTCommunityComponentSt)
                    {
                        name = ((WWTCommunityComponentSt)obj).Name;
                        viewerPermission = ((WWTCommunityComponentSt)obj).ViewerAccess;
                    }
                    if (name == null)
                        continue;
                    switch (viewerPermission)
                    {
                        case Permission.OWNER:
                            name += " (Owner)";
                            break;
                        case Permission.WRITE:
                            name += " (Write)";
                            break;
                        default:
                            break;
                    }
                    ToolStripMenuItem tempMenu = new ToolStripMenuItem(name);
                    tempMenu.Click += new EventHandler(AddtoCommunityMenu_Click);
                    tempMenu.Tag = obj;
                    menuItem.DropDownItems.Add(tempMenu);
                    CreatePickFolderMenuCommunities(tempMenu, AddtoCommunityMenu_Click);
                }
            }
        }
        private void newCommunityMenu_Click(object sender, EventArgs e)
        {
            object target = (targetObject == null) ?
                (object)this.getTarget() : (object)targetObject;
            UploadToNewCommunity(target);
        }
        public static void UploadToNewCommunity(object target)
        {
            CloudCommunityCreate newForm = new CloudCommunityCreate();
            newForm.ShowDialog();

            if (CloudCommunityCreate.createdCommunityId.HasValue)
            {
                long communityId = CloudCommunityCreate.createdCommunityId.Value;
                MessageBox.Show(string.Format(Language.GetLocalizedText(-1, "Community {0} created!"), communityId));
                CloudShareProgress progressReporting = new CloudShareProgress(communityId, null, target);
                progressReporting.ShowDialog();
                progressReporting = null;
            }
        }
        public static byte[] ImageToByteArray(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create new bitmap to avoid locking
                Bitmap newBmp = new Bitmap(bmp);
                newBmp.Save(ms, ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                newBmp.Dispose();
                return ms.ToArray();
            }
        }
        private byte[] FileToByteArray(string filename)
        {
            return File.ReadAllBytes(filename);
        }
        private static string FileContentType(string filename)
        {
            string extension = new FileInfo(filename).Extension.ToLower();
            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }
            foreach (string key in Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type").GetSubKeyNames())
            {
                RegistryKey rkey = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + key);
                object regExt = rkey.GetValue("Extension");
                if (regExt != null && regExt.ToString().ToLower() == extension)
                {
                    return key;
                }
            }
            return null;
        }
        public static string CallUploader(string filename, string name, string contentType, bool isImage)
        {
            byte[] fileBytes = File.ReadAllBytes(filename);
            MD5CryptoServiceProvider md5Service = new MD5CryptoServiceProvider();
            string md5 = BitConverter.ToString(md5Service.ComputeHash(fileBytes)).Replace("-", "");
            string url = new apiUsers().getRealFileUrl(GetTokenFromId(), md5, contentType);
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }

            CloudUploader uploader = new CloudUploader(filename, contentType);
            uploader.ShowDialog();
            return uploader.resultUrl;
        }
        public static string CallUploader(byte[] file, string name, string contentType, bool isImage)
        {
            apiUsers uploadApi = new apiUsers();
            MD5CryptoServiceProvider md5Service = new MD5CryptoServiceProvider();
            string md5 = BitConverter.ToString(md5Service.ComputeHash(file)).Replace("-", "");
            string url = new apiUsers().getRealFileUrl(GetTokenFromId(), md5, contentType);
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }

            return uploadApi.UploadFile(GetTokenFromId(), file, name, "image/jpeg", !isImage);
        }
        public static void AddUpdObjectToCommunity(long CommunityId, long? ComponentId, object target, bool isUpdate)
        {
            CloudCommunityComponentsAPI.apiCommunityComponents api =
                new CloudCommunityComponentsAPI.apiCommunityComponents();
            string name = "", thumbnail = "", authorThumbnail = "";
            if (target is Tour)
            {
                Tour tour = (Tour)target;
                name = tour.Name;
                thumbnail = tour.ThumbnailUrl;
                if (string.IsNullOrEmpty(thumbnail) || File.Exists(thumbnail))
                {
                    thumbnail = CallUploader(ImageToByteArray(tour.ThumbNail), tour.Name, "image/jpeg", true);
                }
                authorThumbnail = tour.AuthorImageUrl;
                if ((string.IsNullOrEmpty(authorThumbnail) || File.Exists(authorThumbnail)) && tour.AuthorImage != null)
                {
                    authorThumbnail = CallUploader(ImageToByteArray(tour.AuthorImage), tour.Author, "image/jpeg", true);
                }
            }
            else
            {
                if (target is IPlace)
                {
                    IPlace iplace = (IPlace)target;
                    name = iplace.Name;
                    thumbnail = iplace.Thumbnail;
                    if (string.IsNullOrEmpty(thumbnail) || File.Exists(thumbnail))
                    {
                        thumbnail = CallUploader(ImageToByteArray(iplace.ThumbNail), iplace.Name, "image/jpeg", true);
                    }
                }
                else
                {
                    if (target is ImageSet)
                    {
                        ImageSet imageset = (ImageSet)target;
                        name = imageset.Name;
                        thumbnail = imageset.ThumbnailUrl;
                        if (string.IsNullOrEmpty(thumbnail) || File.Exists(thumbnail))
                        {
                            thumbnail = CallUploader(ImageToByteArray(imageset.ThumbNail), imageset.Name, "image/jpeg", true);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Language.GetLocalizedText(-1, "Error: {0} not supported in client upload at the moment..."), target.GetType()));
                        return;
                    }
                }
            }
            XmlDocument doc = new XmlDocument();
            XmlSerializer xml = new XmlSerializer(target.GetType());
            MemoryStream memStream = new MemoryStream();
            xml.Serialize(memStream, target);
            memStream.Seek(0, SeekOrigin.Begin);
            doc.Load(memStream);
            foreach (XmlNode x in doc.DocumentElement.Attributes)
            {
                bool isImage = (target is IPlace && ((IPlace)target).IsImage) ||
                    x.Name == "Thumbnail" ||
                    x.Name == "ThumbnailUrl";
                switch (x.Name)
                {
                    case "Thumbnail":
                    case "Url":
                    case "TourUrl":
                        if (File.Exists(x.InnerText))
                        {
                            string contentType = FileContentType(x.InnerText);
                            if (contentType != null)
                                x.InnerText = CallUploader(x.InnerText, x.Name, contentType, isImage);
                        }
                        break;
                    case "ThumbnailUrl":
                        x.InnerText = thumbnail;
                        break;
                    case "AuthorImageUrl":
                        x.InnerText = authorThumbnail;
                        break;
                }
            }
            if (isUpdate)
            {
                api.Update(GetTokenFromId(), CommunityId, ComponentId.Value, name, thumbnail, 
                    doc.DocumentElement.OuterXml, true, null, null);
            }
            else
            {
                api.Create(GetTokenFromId(), CommunityId,
                    CloudCommunityComponentsAPI.WWTComponentTypes.WWTSimpleComponent,
                    ComponentId, name, thumbnail, doc.DocumentElement.OuterXml, null);
            }
            CloudCommunities.SetAvoidCache(CommunityId, ComponentId);
        }
        private void AddtoCommunityMenu_Click(object sender, EventArgs e)
        {
            object comp = ((ToolStripMenuItem)sender).Tag;
            object target = (targetObject == null) ?
                (object)this.getTarget() : (object)targetObject;
            progressReporting = new CloudShareProgress(target, comp);
            progressReporting.ShowDialog();
            progressReporting = null;
            this.closeContextMenu();
        }
        public ToolStripMenuItem GetAddToCommunityMenu()
        {
            return addToCommunityToolStripMenuItem;
        }
        public static ToolStripMenuItem GetAddToCommunityMenu(getTargetDel getTargetFn, closeContextMenuDel closeContextMenuFn)
        {
            return new CloudCommunities(getTargetFn, closeContextMenuFn).GetAddToCommunityMenu();
        }
        public static ToolStripMenuItem GetAddToCommunityMenu(object target, closeContextMenuDel closeContextMenuFn)
        {
            return new CloudCommunities(target, closeContextMenuFn).GetAddToCommunityMenu();
        }
        public ToolStripMenuItem GetShareOnFacebookMenu()
        {
            return shareOnFacebookMenuItem;
        }
        public static ToolStripMenuItem GetShareOnFacebookMenu(getTargetDel getTargetFn, closeContextMenuDel closeContextMenuFn)
        {
            return new CloudCommunities(getTargetFn, closeContextMenuFn).GetShareOnFacebookMenu();
        }
        public static ToolStripMenuItem GetShareOnFacebookMenu(object target, closeContextMenuDel closeContextMenuFn)
        {
            return new CloudCommunities(target, closeContextMenuFn).GetShareOnFacebookMenu();
        }
    }
}
