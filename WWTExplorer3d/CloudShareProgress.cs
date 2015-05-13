using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using TerraViewer.CloudCommunitiesUsersAPI;

namespace TerraViewer
{
    public partial class CloudShareProgress : Form
    {
        private long CommunityId = 0;
        private object target = null;
        private long? ComponentId = null;
        private List<string> errorList = null;
        private Permission perm = Permission.OWNER;
        public CloudShareProgress(object target, object comp)
        {
            this.target = target;
            this.CommunityId = getCommunityId(comp);
            this.ComponentId = getComponentId(comp);
            this.perm = getComponentPermission(comp);
            InitializeComponent();
        }
        public CloudShareProgress(long CommunityId, long? ComponentId, object target)
        {
            this.target = target;
            this.CommunityId = CommunityId;
            this.ComponentId = ComponentId;
            InitializeComponent();
        }
        private void CloudShareProgress_Load(object sender, System.EventArgs e)
        {
            progress.Value = 0;
            errorList = new List<string>();
            backgroundUpdater.WorkerReportsProgress = true;
            backgroundUpdater.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundUpdater_ProgressChanged);
            backgroundUpdater.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundUpdater_RunWorkerCompleted);
            backgroundUpdater.RunWorkerAsync();
        }
        void backgroundUpdater_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progress.Value = 100;
            if (errorList.Count > 0)
            {
                string errors = "Errors:\n";
                foreach (string error in errorList)
                {
                    errors += error + "\n";
                }
                MessageBox.Show(errors);
            }
            this.Close();
            this.Dispose();
        }
        void backgroundUpdater_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }
        private long getCommunityId(object comp)
        {
            if (comp is WWTCommunitySt)
            {
                return ((WWTCommunitySt)comp).CommunityId;
            }
            if (comp is WWTCommunityComponentSt)
            {
                return ((WWTCommunityComponentSt)comp).CommunityId;
            }
            return 0;
        }
        private long? getComponentId(object comp)
        {
            if (comp is WWTCommunityComponentSt)
            {
                return ((WWTCommunityComponentSt)comp).ComponentId;
            }
            return null;
        }
        private Permission getComponentPermission(object comp)
        {
            if (comp is WWTCommunitySt)
            {
                return ((WWTCommunitySt)comp).ViewerAccess;
            }
            if (comp is WWTCommunityComponentSt)
            {
                return ((WWTCommunityComponentSt)comp).ViewerAccess;
            }
            return Permission.NONE;
        }
        private void AddObjectToCommunity(long _CommunityId, long? _ComponentId, object _target, int maxProgress)
        {
            try
            {
                apiUsers uploadApi = new apiUsers();
                if (_target is Folder)
                {
                    Folder folder = (Folder)_target;
                    string thumbnail = string.IsNullOrEmpty(folder.Thumbnail) ?
                        CloudCommunities.CallUploader(CloudCommunities.ImageToByteArray(folder.ThumbNail), folder.Name, "image/jpeg", true) :
                        folder.Thumbnail;
                    CloudCommunityComponentsAPI.apiCommunityComponents api =
                        new CloudCommunityComponentsAPI.apiCommunityComponents();
                    CloudCommunityComponentsAPI.WWTCommunityComponentSt f =
                        api.Create(CloudCommunities.GetTokenFromId(), _CommunityId,
                        CloudCommunityComponentsAPI.WWTComponentTypes.WWTCollection,
                        _ComponentId, folder.Name, thumbnail, "", null);
                    int childNumber = 0, progActual = progress.Value,
                        progDiff = maxProgress - progress.Value;
                    foreach (object obj in folder.Children)
                    {
                        int maxProgChild = (++childNumber) * progDiff / folder.Children.Length;
                        AddObjectToCommunity(f.CommunityId, f.ComponentId, obj, progActual + maxProgChild);
                    }
                    CloudCommunities.SetAvoidCache(_CommunityId, _ComponentId);
                }
                else
                {
                    CloudCommunities.AddUpdObjectToCommunity(_CommunityId, _ComponentId, _target, false);
                }
            }
            catch (Exception e)
            {
                errorList.Add(_target.GetType().ToString() + ": " + getTargetName(_target) + " - Error: " + e.Message);
            }
            backgroundUpdater.ReportProgress(maxProgress);
        }
        private string getTargetName(object _target)
        {
            if (_target is Folder)
            {
                return ((Folder)_target).Name;
            }
            else
            {
                if (_target is Tour)
                {
                    return ((Tour)_target).Name;
                }
                else
                {
                    if (_target is IPlace)
                    {
                        return ((IPlace)_target).Name;
                    }
                    else
                    {
                        if (_target is ImageSet)
                            return ((ImageSet)_target).Name;
                    }
                }
            }
            return "N/A";
        }
        private void backgroundUpdater_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if (perm == Permission.WRITE || perm == Permission.OWNER)
                {
                    AddObjectToCommunity(CommunityId, ComponentId, target, 99);
                }
                else
                {
                    MessageBox.Show("You need to own or have writing permissions to do that!");
                }
            }
            catch (SoapException ex)
            {
                CloudCommunities.ProcessSoapException(ex);
            }
            backgroundUpdater.ReportProgress(100);
        }
    }
}
