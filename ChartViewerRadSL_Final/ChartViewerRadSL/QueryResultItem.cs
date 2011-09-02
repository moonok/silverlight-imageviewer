using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChartViewerRadSL
{
    public class QueryResultItem
    {
        private string _ItemUri;
        public string ItemUri
        {
            get { return this._ItemUri; }
            set { this._ItemUri = value; }
        }

        private string _Description;
        public string Description
        {
            get { return this._Description; }
            set { this._Description = value; }
        }

        private string _Title;
        public string Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        private string _ImageUri;
        public string ImageUri
        {
            get { return this._ImageUri; }
            set { this._ImageUri = value; }
        }

        public QueryResultItem()
        {
        }

        public QueryResultItem(string title, string description, string itemuri, string imageuri)
        {
            this._ItemUri = itemuri;
            this._Description = description;
            this._Title = title;
            this._ImageUri = imageuri;
        }
    }
}
