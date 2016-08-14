using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitatoBot.Component.Checklist {

    internal partial class Checklist {

        private class Entry {

            public string Text { private set; get; }
            public bool IsChecked { private set; get; }
            public string CheckedBy { private set; get; }

            public Entry(string text) {
                Text = text;
                IsChecked = false;
            }

            public void SetState(bool state, string owner) {
                IsChecked = state;
                CheckedBy = owner;
            }

        }

    }
}
