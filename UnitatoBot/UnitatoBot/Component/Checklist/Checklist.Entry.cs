using BotCore.Component;

namespace Unitato.Component.Checklist {

    internal partial class Checklist : SavableMessageContainer {

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
