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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TransitionControl;

namespace ChartViewerRadSL
{
    public class TransitionSet : ViewModelBase
    {
        private TransitionProvider _ForwardTransition;
        public TransitionProvider ForwardTransition
        {
            get
            {
                return this._ForwardTransition;
            }
            set
            {
                if (this._ForwardTransition != value)
                {
                    this._ForwardTransition = value;
                    this.OnPropertyChanged(() => this.ForwardTransition);
                }
            }
        }

        private TransitionProvider _BackwardTransition;
        public TransitionProvider BackwardTransition
        {
            get
            {
                return this._BackwardTransition;
            }
            set
            {
                if (this._BackwardTransition != value)
                {
                    this._BackwardTransition = value;
                    this.OnPropertyChanged(() => this.BackwardTransition);
                }
            }
        }
    }

}
