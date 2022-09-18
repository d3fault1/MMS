﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MMS.UI.Helpers;

namespace MMS.UI.Views.AppPages
{
    /// <summary>
    /// Interaction logic for ContentUpload.xaml
    /// </summary>
    public partial class ContentUpload : Page
    {
        public ContentUpload()
        {
            InitializeComponent();
        }

        private void ContentDetailsListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listview = (ListView)sender;
            var gridview = (GridView)listview.View;
            UIHelper.UniformGridViewColumnSize(gridview, e);
        }

        private void deviceOptionsTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void deviceOptionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
