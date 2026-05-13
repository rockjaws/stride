using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace stride.Presentation.Common;

// =====================================
//           ObservableObject
//        Author: Oliver + Nicolaj
// =====================================
// Abstract class, as we don't want object creation to be possible.
// Implements INotifyPropertyChanged, allowing property change notifications to update the UI.
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged; // PropertyChangedEventHandler

    // is the event responsible for
    // updating the properties and UI

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) // get's the object/sender
    // that called the OnPropertyChanged
    {
        // If there are any subscribers, raise the PropertyChanged event, passing the property name.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string propertyName = null
    )
    {
        // Check if the current value (field) is equal to the new value.
        // If they are the same, no change is needed, and the event is not raised.
        if (Equals(field, value))
            return false; // Nothing unchanged, so return false.

        // Update the backing field with the new value
        field = value;

        // Notify any data bindings that this property has changed via OnPropertyChanged
        // This triggers UI updates automatically.
        OnPropertyChanged(propertyName);

        // Return true to indicate the value was changed
        return true;
    }
}
