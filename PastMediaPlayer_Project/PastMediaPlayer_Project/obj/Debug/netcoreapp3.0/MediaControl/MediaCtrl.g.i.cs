﻿#pragma checksum "..\..\..\..\MediaControl\MediaCtrl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A911741754C31DF9D8CEF3ACF2370CBAB7647F11"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using PastMediaPlayer_Project.MediaControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PastMediaPlayer_Project.MediaControl {
    
    
    /// <summary>
    /// MediaCtrl
    /// </summary>
    public partial class MediaCtrl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CurTime;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LastTime;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Back_btn;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Play_btn;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Fore_btn;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\MediaControl\MediaCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement MainMedia;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PastMediaPlayer_Project;component/mediacontrol/mediactrl.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CurTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.LastTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.Back_btn = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.Back_btn.Click += new System.Windows.RoutedEventHandler(this.Back_btn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Play_btn = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.Play_btn.Click += new System.Windows.RoutedEventHandler(this.Play_btn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Fore_btn = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.Fore_btn.Click += new System.Windows.RoutedEventHandler(this.Fore_btn_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MainMedia = ((System.Windows.Controls.MediaElement)(target));
            
            #line 37 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.MainMedia.Loaded += new System.Windows.RoutedEventHandler(this.MainMedia_Loaded);
            
            #line default
            #line hidden
            
            #line 38 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.MainMedia.MediaOpened += new System.Windows.RoutedEventHandler(this.MainMedia_MediaOpened);
            
            #line default
            #line hidden
            
            #line 39 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.MainMedia.MediaEnded += new System.Windows.RoutedEventHandler(this.MainMedia_MediaEnded);
            
            #line default
            #line hidden
            
            #line 40 "..\..\..\..\MediaControl\MediaCtrl.xaml"
            this.MainMedia.MediaFailed += new System.EventHandler<System.Windows.ExceptionRoutedEventArgs>(this.MainMedia_MediaFailed);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

