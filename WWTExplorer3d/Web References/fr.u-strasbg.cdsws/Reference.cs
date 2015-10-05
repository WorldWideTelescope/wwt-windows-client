﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.34014.
// 
#pragma warning disable 1591

namespace TerraViewer.fr.u_strasbg.cdsws {
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SesameSoapBinding", Namespace="urn:Sesame")]
    public partial class SesameService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback sesameOperationCompleted;
        
        private System.Threading.SendOrPostCallback sesame1OperationCompleted;
        
        private System.Threading.SendOrPostCallback sesame2OperationCompleted;
        
        private System.Threading.SendOrPostCallback SesameXMLOperationCompleted;
        
        private System.Threading.SendOrPostCallback SesameOperationCompleted;
        
        private System.Threading.SendOrPostCallback getAvailabilityOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public SesameService() {
            this.Url = global::TerraViewer.Properties.Settings.Default.WWTExplorer_fr_u_strasbg_cdsws_SesameService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event sesameCompletedEventHandler sesameCompleted;
        
        /// <remarks/>
        public event sesame1CompletedEventHandler sesame1Completed;
        
        /// <remarks/>
        public event sesame2CompletedEventHandler sesame2Completed;
        
        /// <remarks/>
        public event SesameXMLCompletedEventHandler SesameXMLCompleted;
        
        /// <remarks/>
        public event SesameCompletedEventHandler SesameCompleted;
        
        /// <remarks/>
        public event getAvailabilityCompletedEventHandler getAvailabilityCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="urn:Sesame", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string sesame(string name, string resultType) {
            object[] results = this.Invoke("sesame", new object[] {
                        name,
                        resultType});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sesameAsync(string name, string resultType) {
            this.sesameAsync(name, resultType, null);
        }
        
        /// <remarks/>
        public void sesameAsync(string name, string resultType, object userState) {
            if ((this.sesameOperationCompleted == null)) {
                this.sesameOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsesameOperationCompleted);
            }
            this.InvokeAsync("sesame", new object[] {
                        name,
                        resultType}, this.sesameOperationCompleted, userState);
        }
        
        private void OnsesameOperationCompleted(object arg) {
            if ((this.sesameCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sesameCompleted(this, new sesameCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.WebMethodAttribute(MessageName="sesame1")]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="urn:Sesame", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string sesame(string name, string resultType, bool all) {
            object[] results = this.Invoke("sesame1", new object[] {
                        name,
                        resultType,
                        all});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sesame1Async(string name, string resultType, bool all) {
            this.sesame1Async(name, resultType, all, null);
        }
        
        /// <remarks/>
        public void sesame1Async(string name, string resultType, bool all, object userState) {
            if ((this.sesame1OperationCompleted == null)) {
                this.sesame1OperationCompleted = new System.Threading.SendOrPostCallback(this.Onsesame1OperationCompleted);
            }
            this.InvokeAsync("sesame1", new object[] {
                        name,
                        resultType,
                        all}, this.sesame1OperationCompleted, userState);
        }
        
        private void Onsesame1OperationCompleted(object arg) {
            if ((this.sesame1Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sesame1Completed(this, new sesame1CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.WebMethodAttribute(MessageName="sesame2")]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="urn:Sesame", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string sesame(string name, string resultType, bool all, string service) {
            object[] results = this.Invoke("sesame2", new object[] {
                        name,
                        resultType,
                        all,
                        service});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void sesame2Async(string name, string resultType, bool all, string service) {
            this.sesame2Async(name, resultType, all, service, null);
        }
        
        /// <remarks/>
        public void sesame2Async(string name, string resultType, bool all, string service, object userState) {
            if ((this.sesame2OperationCompleted == null)) {
                this.sesame2OperationCompleted = new System.Threading.SendOrPostCallback(this.Onsesame2OperationCompleted);
            }
            this.InvokeAsync("sesame2", new object[] {
                        name,
                        resultType,
                        all,
                        service}, this.sesame2OperationCompleted, userState);
        }
        
        private void Onsesame2OperationCompleted(object arg) {
            if ((this.sesame2Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.sesame2Completed(this, new sesame2CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="urn:Sesame", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string SesameXML(string name) {
            object[] results = this.Invoke("SesameXML", new object[] {
                        name});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SesameXMLAsync(string name) {
            this.SesameXMLAsync(name, null);
        }
        
        /// <remarks/>
        public void SesameXMLAsync(string name, object userState) {
            if ((this.SesameXMLOperationCompleted == null)) {
                this.SesameXMLOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSesameXMLOperationCompleted);
            }
            this.InvokeAsync("SesameXML", new object[] {
                        name}, this.SesameXMLOperationCompleted, userState);
        }
        
        private void OnSesameXMLOperationCompleted(object arg) {
            if ((this.SesameXMLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SesameXMLCompleted(this, new SesameXMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="urn:Sesame", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string Sesame(string name) {
            object[] results = this.Invoke("Sesame", new object[] {
                        name});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SesameAsync(string name) {
            this.SesameAsync(name, null);
        }
        
        /// <remarks/>
        public void SesameAsync(string name, object userState) {
            if ((this.SesameOperationCompleted == null)) {
                this.SesameOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSesameOperationCompleted);
            }
            this.InvokeAsync("Sesame", new object[] {
                        name}, this.SesameOperationCompleted, userState);
        }
        
        private void OnSesameOperationCompleted(object arg) {
            if ((this.SesameCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SesameCompleted(this, new SesameCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://DefaultNamespace", ResponseNamespace="urn:Sesame")]
        [return: System.Xml.Serialization.SoapElementAttribute("getAvailabilityReturn")]
        public string getAvailability() {
            object[] results = this.Invoke("getAvailability", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getAvailabilityAsync() {
            this.getAvailabilityAsync(null);
        }
        
        /// <remarks/>
        public void getAvailabilityAsync(object userState) {
            if ((this.getAvailabilityOperationCompleted == null)) {
                this.getAvailabilityOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetAvailabilityOperationCompleted);
            }
            this.InvokeAsync("getAvailability", new object[0], this.getAvailabilityOperationCompleted, userState);
        }
        
        private void OngetAvailabilityOperationCompleted(object arg) {
            if ((this.getAvailabilityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getAvailabilityCompleted(this, new getAvailabilityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void sesameCompletedEventHandler(object sender, sesameCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sesameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sesameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void sesame1CompletedEventHandler(object sender, sesame1CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sesame1CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sesame1CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void sesame2CompletedEventHandler(object sender, sesame2CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class sesame2CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal sesame2CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void SesameXMLCompletedEventHandler(object sender, SesameXMLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SesameXMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SesameXMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void SesameCompletedEventHandler(object sender, SesameCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SesameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SesameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    public delegate void getAvailabilityCompletedEventHandler(object sender, getAvailabilityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getAvailabilityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getAvailabilityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591