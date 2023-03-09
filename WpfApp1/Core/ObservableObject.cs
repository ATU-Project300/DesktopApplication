using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Odyssey.Core
{
    class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
