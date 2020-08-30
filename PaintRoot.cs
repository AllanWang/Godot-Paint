using Godot;
using System;

public class PaintRoot : Container
{
    private ColorPalette _colorPalette;
    private PaintPanel _paintPanel;
    private Messages _messages;

    public override void _Ready()
    {
        _colorPalette = (ColorPalette) GetNode("ColorPalette");
        _paintPanel = (PaintPanel) GetNode("PaintPanel");
        _messages = (Messages) GetNode("Messages");
    }

    private void SortChildren()
    {
        var isLandscape = RectSize.x > RectSize.y;
        if (isLandscape) SortLandscape();
        else SortPortrait();
    }

    private void SortPortrait()
    {
        var messageHeight = Math.Max(200, RectSize.y * 0.35f);
        var paletteHeight = _colorPalette.RectSize.y;
        var panelHeight = RectSize.y - messageHeight - paletteHeight;

        FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x, panelHeight));
        FitChildInRect(_colorPalette,
            new Rect2(0, panelHeight, RectSize.x, paletteHeight));
        FitChildInRect(_messages, new Rect2(0, panelHeight + paletteHeight, RectSize.x, messageHeight));
    }

    private void SortLandscape()
    {
        const int paletteHeight = 50;
        const int messageWidth = 500;
        FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x - messageWidth, RectSize.y - paletteHeight));
        FitChildInRect(_colorPalette,
            new Rect2(0, RectSize.y - paletteHeight, RectSize.x - messageWidth, paletteHeight));
        FitChildInRect(_messages, new Rect2(RectSize.x - messageWidth, 0, messageWidth, RectSize.y));
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