using System.Windows.Media;
using Telerik.Windows.Controls;

namespace ChartViewerRadSL
{
    public class ViewModelItem : ViewModelBase
    {
        private string _Title;
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                if (value != this._Title)
                {
                    this._Title = value;
                    this.OnPropertyChanged(() => this.Title);
                }
            }
        }

        private string _ItemUri;
        public string ItemUri
        {
            get
            {
                return this._ItemUri;
            }
            set
            {
                if (value != this._ItemUri)
                {
                    this._ItemUri = value;
                    this.OnPropertyChanged(() => this.ItemUri);
                }
            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                if (value != this._Description)
                {
                    this._Description = value;
                    this.OnPropertyChanged(() => this.Description);
                }
            }
        }

        private string _ChartImageUri;
        public string ChartImageUri
        {
            get
            {
                return this._ChartImageUri;
            }
            set
            {
                if (value != this._ChartImageUri)
                {
                    this._ChartImageUri = value;
                    this.OnPropertyChanged(() => this.ChartImageUri);
                }
            }
        }
    }
}
