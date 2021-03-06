﻿using ProgrammingParadigms;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DomainAbstractions
{
    /// <summary>
    /// A menu item of a menu that can be clicked. Has a IEvent port on the RHS. 
    /// When the item is clicked, it generates an event.
    /// ------------------------------------------------------------------------------------------------------------------
    /// Ports:
    /// 1. IUI wpfElement: The input IUI for get WPF element
    /// 2. IDataFlow<bool> visible: toggles the visibility of the menu item
    /// 3. IEvent eventOutput: output event for when the menu item is clicked
    /// 4. IDataFlow_B<bool> dataFlowBOutput: data flow is false, background colour of the menu item will be set to grey
    /// </summary>
    public class MenuItem : IUI, IDataFlow<bool> // wpfElement, visible
    {
        // properties
        public string InstanceName { get; set; }  = "Default";


        public string IconName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    menuItem.Icon = new System.Windows.Controls.Image()
                    {
                        Source = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + @";component/Application/" + string.Format("Resources/{0}", value), UriKind.Absolute)),
                        Height = 20,
                        MaxWidth = 25
                    };
                }
            }
        }

        // ports
        private IEvent output;
        private IDataFlow_B<bool> enableInput;  // greys out the menu item when disabled

        // private fields
        private System.Windows.Controls.MenuItem menuItem;

        /// <summary>
        /// An IUI element which is a windows-desktop application style MenuItem. 
        /// </summary>
        /// <param name="title">the text displayed on the menu item</param>
        public MenuItem(string title, bool visible = true)
        {
            menuItem = new System.Windows.Controls.MenuItem();
            menuItem.Header = title;
            menuItem.VerticalAlignment = VerticalAlignment.Center;
            menuItem.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            menuItem.FontSize = 14;
            menuItem.Click += (object sender, RoutedEventArgs e) => output?.Execute();
            menuItem.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        // By having this name convention, this method gets called by WireTo immediately after the correspeonding port is wired
        private void enableInputInitialize()
        {
                enableInput.DataChanged += () =>
                {
                    menuItem.IsEnabled = enableInput.Data;
                    menuItem.Foreground = menuItem.IsEnabled ? new SolidColorBrush(Color.FromRgb(0, 0, 0)) : Brushes.DarkGray;
                };
        }

        // IUI implmentation -----------------------------------------------------------
        UIElement IUI.GetWPFElement() => menuItem;

        // IDataFlow<bool> implementation -----------------------------------------------
        bool IDataFlow<bool>.Data { set => menuItem.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }
}
