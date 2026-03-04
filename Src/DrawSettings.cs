using System.ComponentModel;

namespace PaintApp.Src;

public class DrawSettings: INotifyPropertyChanged
{
    private Tool? tool = null;
    private byte[] color = [0, 0, 0, 255];
    public byte[] Color
    {
        get => color;
        set
        {
            if (color != value)
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public Tool? Tool
    {
        get => tool;
        set
        {
            if (tool != value)
            {
                tool = value;
                OnPropertyChanged(nameof(Tool));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}