using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tally.model
{
    class tallyitemViewModel
    {
        private ObservableCollection<model.tallyitems> allitems = new ObservableCollection<model.tallyitems>();
        public ObservableCollection<model.tallyitems> Allitems { get { return this.allitems; } }

        public void Additem(long id, string first, string second, string money, string detail, DateTimeOffset date)
        {
            this.allitems.Add(new model.tallyitems(id, first, second, money, detail, date));
        }

        public void Removeitem(string id)
        {
            allitems.Remove(allitems[int.Parse(id)]);
            MainPage.Current.lv.SelectedItem = null;
        }

        public tallyitems get_item_by_id(long id)
        {
            foreach (tallyitems temp in allitems)
            {
                if (temp.id == id)
                    return temp;
            }
            return null;
        }


        public void Updateitem(string id, string first, string second, string money, string detail, DateTimeOffset date)
        {
            int MyInt = int.Parse(id);
            allitems[MyInt].first_label = first;
            allitems[MyInt].second_label = second;
            allitems[MyInt].detail = detail;
            allitems[MyInt].money = money;
            allitems[MyInt].date = date;
            MainPage.Current.lv.SelectedItem = null;
        }
    }
}
