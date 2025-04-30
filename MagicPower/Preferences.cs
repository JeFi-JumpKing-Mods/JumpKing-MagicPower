using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MagicPower;
public class Preferences : INotifyPropertyChanged
{
    private bool _magicPower = false;
    public bool hasMagicPower
    {
        get => _magicPower;
        set
        {
            _magicPower = value;
            OnPropertyChanged();
        }
    }

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}