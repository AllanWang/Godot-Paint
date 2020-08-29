using Godot;
using System;

public class PaintRoot : Container
{
    private ColorPalette _colorPalette;
    private PaintControl _paintControl;

    public override void _Ready()
    {
        Console.WriteLine("Container ready");
        _colorPalette = (ColorPalette) GetNode("ColorPalette");
        _paintControl = (PaintControl) GetNode("PaintControl");

        // GetViewport().Connect("size_changed", this, nameof(QueueSort));
    }

    private void SortChildren()
    {
        var isLandscape = RectSize.x > RectSize.y;
        Console.WriteLine($"Sort Children {isLandscape} {RectSize}");
        FitChildInRect(_colorPalette, new Rect2(0, RectSize.y - 50, RectSize.x, 50));
        FitChildInRect(_paintControl, new Rect2(0, 0, RectSize.x, RectSize.y - 50));
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case NotificationSortChildren:
                SortChildren();
                break;
            case NotificationResized:
                QueueSort();
                break;
        }
    }
}