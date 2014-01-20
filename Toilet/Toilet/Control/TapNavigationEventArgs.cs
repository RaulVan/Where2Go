using Com.AMap.Api.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toilet.Control
{
    public class TapNavigationEventArgs:EventArgs
    {
        //public TapNavigationEventArgs()
        public AMapPOI MarkerAMapPOI
        {
            get;
            set;
        }
    }
}
