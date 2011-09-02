using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace ChartViewerRadSL
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            string param1 = "SiteUrl";
            string param2 = "ListName";
            string additionalLink = string.Empty;

            if (App.Current.Host.InitParams.ContainsKey(param1)
                || App.Current.Host.InitParams.ContainsKey(param2))
            {
                string _siteUrl = HttpUtility.UrlDecode(App.Current.Host.InitParams[param1]);
                string _listName = HttpUtility.UrlDecode(App.Current.Host.InitParams[param2]);
                additionalLink = string.Format("{0}/{1}", _siteUrl, _listName);
            }

            this.mainpage_hyperlinkbutton_moreCharts.NavigateUri = new Uri(additionalLink);
        }
    }
}
