using System.Windows.Input;
using Telerik.Windows.Controls;
using System.ComponentModel;
using Telerik.Windows.Controls.TransitionControl;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.ServiceModel;
using System;
using System.Windows.Browser;
using System.Windows.Threading;
using MVVMSupport;

namespace ChartViewerRadSL
{
    public class ViewModel : ViewModelBase, ISupportInitialize
    {
        string _siteUrl = string.Empty;
        string _listGuid = string.Empty;
        string _listName = string.Empty;
        DispatcherTimer _timer = new DispatcherTimer();

        const string _queryString1 = "termlabel";
        const string _queryString2 = "region";
        const string param1 = "SiteUrl";
        const string param2 = "ListGuid";
        const string param3 = "ListName";

        #region Functions to interact with SP webservice
        public void BindListItems()
        {
            if (_siteUrl == string.Empty
                || _listGuid == string.Empty
                || _listName == string.Empty
                )
            {
                return;
            }

            //string webserviceUri = "http://brown-moss/MoonSilverlight/_vti_bin/lists.asmx";
            //string sharepointListGuid = "C1F5B195-51E5-4F82-A3E4-039D6A83BEC6";

            //string queryS = GetQueryWithFilter();
            string webserviceUri = _siteUrl + "/_vti_bin/lists.asmx" ;
            string sharepointListGuid = _listGuid;

            XElement query = GetQueryWithFilter(); //new XElement("Query");
            XElement queryOptions = new XElement("QueryOptions");
            XElement viewFields = new XElement("ViewFields");

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endPointAddress = new EndpointAddress(webserviceUri);

            basicHttpBinding.MaxBufferSize = 2147483647;
            basicHttpBinding.MaxReceivedMessageSize = 2147483647;

            SpProxy.ListsSoapClient proxy = new SpProxy.ListsSoapClient(basicHttpBinding, endPointAddress);
            proxy.GetListItemsCompleted += new EventHandler<SpProxy.GetListItemsCompletedEventArgs>(proxy_GetListItemsCompleted);
            proxy.GetListItemsAsync(sharepointListGuid, null, query, viewFields, null, queryOptions, null);
        }

        void proxy_GetListItemsCompleted(object sender, SpProxy.GetListItemsCompletedEventArgs e)
        {
            // get the set of data from results
            XNamespace ns = "#RowsetSchema";
            var query = from x in e.Result.Descendants()
                        where x.Name == ns + "row"
                        select new QueryResultItem
                        (
                            x.Attribute("ows_Title") == null ? "no title" : x.Attribute("ows_Title").Value,
                            x.Attribute("ows_Description") == null ? string.Empty : x.Attribute("ows_Description").Value,
                            x.Attribute("ows_ID") == null ? string.Empty : x.Attribute("ows_ID").Value,
                            x.Attribute("ows_EncodedAbsUrl") == null ? string.Empty : x.Attribute("ows_EncodedAbsUrl").Value
                        );

            // debug
            //System.Windows.Browser.HtmlPage.Window.Alert(System.Convert.ToString(query.Count()));

            //run through the data set to add to the list
            foreach (QueryResultItem qItem in query)
            {
                ViewModelItem nItem = new ViewModelItem();
                nItem.Title = qItem.Title;
                nItem.Description = qItem.Description;
                nItem.ItemUri = _siteUrl + "/" + _listName + "/Forms/DispForm.aspx?ID=" +  qItem.ItemUri;
                nItem.ChartImageUri = qItem.ImageUri;
                this._Persons.Add(nItem);  // DELETE - to have mainpage load during a design time
            }

            if (this._Persons.Count == 0)
            {
                ViewModelItem noitem = new ViewModelItem();
                noitem.Title = string.Empty;
                noitem.Description = string.Empty;
                noitem.ItemUri = string.Empty;
                noitem.ChartImageUri = "http://edweb/iebmrp/SiteAssets/Vorsite/ChartViewerSL/NoChartsImage.png";
                this._Persons.Add(noitem);
            }

            this.SelectedPerson = this.Persons[0];  // do this, otherwise the first item showing is an empty data //// DELETE - to have mainpage load during a design time          
        }

        /// <summary>
        /// returns XElement object for a query with filter if needed
        /// Sample query =  <Query><Where><Contains><FieldRef Name='Topic' /><Value Type='TaxonomyFieldTypeMulti'>{0}</Value></Contains></Where></Query> 
        /// </summary>        
        private XElement GetQueryWithFilter()
        {
            XElement query = new XElement("Query");
            
            if (HtmlPage.Document.QueryString.ContainsKey(_queryString1))
            {
                string filterValue = HtmlPage.Document.QueryString[_queryString1];

                XElement value = new XElement("Value", filterValue);
                value.SetAttributeValue("Type", "TaxonomyFieldTypeMulti");
                XElement fieldRef = new XElement("FieldRef");
                fieldRef.SetAttributeValue("Name", "Topic");
                XElement contains = new XElement("Contains", fieldRef, value);
                XElement where = new XElement("Where", contains);
                query.Add(where);
            }
            if (HtmlPage.Document.QueryString.ContainsKey(_queryString2))
            {
                string filterValue = HtmlPage.Document.QueryString[_queryString2];

                XElement value = new XElement("Value", filterValue);
                value.SetAttributeValue("Type", "TaxonomyFieldTypeMulti");
                XElement fieldRef = new XElement("FieldRef");
                fieldRef.SetAttributeValue("Name", "Region");
                XElement contains = new XElement("Contains", fieldRef, value);
                XElement where = new XElement("Where", contains);
                query.Add(where);
            }
            return query;
        }
        #endregion
       
        public ViewModel()
        {
            LoadInitialParameters();

            // add event hanlders
            this.PauseTimerCommand = new DelegateCommand<MouseEventArgs>(this.PauseTimerFunc);
            this.ResumeTimerCommand = new DelegateCommand<MouseEventArgs>(this.ResumeTimerFunc);

            this.SelectNext = new DelegateCommand(this.SelectNextPerson);
            this.SelectPrevious = new DelegateCommand(this.SelectPreviousPerson);
            this.currentTransitionIndex = -1;

            this._Persons = new ObservableCollection<ViewModelItem>();
            this._Transitions = new ObservableCollection<TransitionSet>();

            #region sample persons data source
            /* add Sample persons - moonok
            Item me = new Item();
            me.Title = "title here ...";
            me.Description = "description here ... ";
            me.ItemUri = "http://www.google.com";
            me.ChartImageUri = "http://v-mokim/SnipImage.JPG";
            this._Persons.Add(me);

            Item me2 = new Item();
            me2.Title = "2 title here ...";
            me2.Description = "2 description here ... ";
            me2.ItemUri = "2 item uri here ... ";
            this._Persons.Add(me2);

            Item me3 = new Item();
            me3.Title = "3 title here ...";
            me3.Description = "3 description here ... ";
            me3.ItemUri = "3 item uri here ... ";
            this._Persons.Add(me3);
             */
            #endregion

            // get sharepoint picture lib items
            BindListItems();

            // create a timer with an action to handle on every tick
            _timer.Interval = new TimeSpan(0,0,4);  // 4 sec
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }

        private void LoadInitialParameters()
        {
            if (App.Current.Host.InitParams.ContainsKey(param1))
            {
                _siteUrl = HttpUtility.UrlDecode(App.Current.Host.InitParams[param1]);
            }
            if (App.Current.Host.InitParams.ContainsKey(param2))
            {
                _listGuid = HttpUtility.UrlDecode(App.Current.Host.InitParams[param2]);
            }
            if (App.Current.Host.InitParams.ContainsKey(param3))
            {
                _listName = HttpUtility.UrlDecode(App.Current.Host.InitParams[param3]);
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.Persons.Count == 0)
            {
                return;
            }

            SelectNextPerson(new object());
        }

        private void RestartTimer()
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer.Start();
            }
        }

        private void PauseTimerFunc(object param)
        {
            this._timer.Stop();
        }

        private void ResumeTimerFunc(object param)
        {
            this._timer.Start();
        }

        private int currentTransitionIndex;

        private void SelectNextPerson(object param)
        {
            if (this.Persons != null && this.Persons.Count() > 1)
            {
                this.SelectNextForwardTransition();

                int index = this.Persons.IndexOf(this.SelectedPerson);
                index++;
                if (index >= this.Persons.Count())
                {
                    index = 0;
                }
                this.SelectedPerson = this.Persons[index];

                RestartTimer();
            }
        }

        private void SelectNextForwardTransition()
        {
            if (this.Transitions != null && this.Transitions.Count() > 0)
            {
                this.currentTransitionIndex++;
                if (this.currentTransitionIndex >= this.Transitions.Count())
                {
                    this.currentTransitionIndex = 0;
                }
                this.CurrentTransition = this.Transitions[this.currentTransitionIndex].ForwardTransition;
            }
            else
            {
                this.CurrentTransition = null;
            }
        }

        private void SelectPreviousPerson(object param)
        {
            if (this.Persons != null && this.Persons.Count() > 1)
            {
                this.SelectPreviousBackwardTransition();

                int index = this.Persons.IndexOf(this.SelectedPerson);
                index--;
                if (index < 0)
                {
                    index = this.Persons.Count() - 1;
                }
                this.SelectedPerson = this.Persons[index];

                RestartTimer();
            }
        }

        private void SelectPreviousBackwardTransition()
        {
            if (this.Transitions != null && this.Transitions.Count() > 0)
            {
                this.CurrentTransition = this.Transitions[this.currentTransitionIndex].BackwardTransition;
                this.currentTransitionIndex--;
                if (this.currentTransitionIndex < 0)
                {
                    this.currentTransitionIndex = this.Transitions.Count() - 1;
                }
            }
            else
            {
                this.CurrentTransition = null;
            }
        }

        private TransitionProvider _CurrentTransition;
        public TransitionProvider CurrentTransition
        {
            get
            {
                return this._CurrentTransition;
            }
            private set
            {
                if (value != this._CurrentTransition)
                {
                    this._CurrentTransition = value;
                    this.OnPropertyChanged("CurrentTransition");
                }
            }
        }

        private ICommand _SelectNext;
        public ICommand SelectNext
        {
            get
            {
                return this._SelectNext;
            }
            private set
            {
                this._SelectNext = value;
            }
        }

        private ICommand _SelectPrevious;
        public ICommand SelectPrevious
        {
            get
            {
                return this._SelectPrevious;
            }
            set
            {
                this._SelectPrevious = value;
            }
        }

        private ViewModelItem _SelectedPerson;
        public ViewModelItem SelectedPerson
        {
            get
            {
                return this._SelectedPerson;
            }
            set
            {
                if ((object)this._SelectedPerson != value)
                {
                    this._SelectedPerson = value;
                    this.OnPropertyChanged("SelectedPerson");
                }
            }
        }


        private ObservableCollection<TransitionSet> _Transitions;
        public ObservableCollection<TransitionSet> Transitions
        {
            get
            {
                return this._Transitions;
            }
        }

        private ObservableCollection<ViewModelItem> _Persons;
        public ObservableCollection<ViewModelItem> Persons
        {
            get
            {
                return this._Persons;
            }
            //set { this._Persons = value; } //moonok
        }


        // setting the event handerl
        private ICommand _PauseTimerCommand;
        public ICommand PauseTimerCommand
        {
            get
            {
                return this._PauseTimerCommand;
            }
            private set
            {
                this._PauseTimerCommand = value;
            }
        }

        private ICommand _ResumeTimerCommand;
        public ICommand ResumeTimerCommand
        {
            get { return this._ResumeTimerCommand; }
            private set
            {
                this._ResumeTimerCommand = value;
            }
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if ((object)this.Persons != null && this.Persons.Count() > 0)
            {
                this.SelectedPerson = this.Persons.FirstOrDefault();
            }
        }
    }
}
