using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tally.model
{
    public class tallyitems : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public long id;

        private string _first_label;
        public string first_label
        {
            get { return _first_label; }
            set
            {
                if (_first_label != value)
                {
                    _first_label = value;
                    OnPropertyChanged("first_label");
                }
            }
        }
        private string _second_label;
        public string second_label
        {
            get { return _second_label; }
            set
            {
                if (_second_label != value)
                {
                    _second_label = value;
                    OnPropertyChanged("second_label");
                }
            }
        }
        private string _money;
        public string money
        {
            get { return _money; }
            set
            {
                if (_money != value)
                {
                    _money = value;
                    OnPropertyChanged("money");
                }
            }
        }



        public string detail { get; set; }
        public DateTimeOffset date { get; set; }

        public tallyitems(long id, string first, string second, string money, string detail,  DateTimeOffset date)
        {
            this.id = id;
            this._first_label = first;
            this._second_label = second;
            this.money = money;
            this.detail = detail;
            this.date = date;
        }
    }
}
