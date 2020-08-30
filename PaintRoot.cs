using Godot;
using System;

public class PaintRoot : Container
{
	private ColorPalette _colorPalette;
	private PaintPanel _paintPanel;
	private Messages _messages;

	public override void _Ready()
	{
		Console.WriteLine("Container ready");
		_colorPalette = (ColorPalette) GetNode("ColorPalette");
		_paintPanel = (PaintPanel) GetNode("PaintPanel");
		_messages = (Messages) GetNode("Messages");

		// GetViewport().Connect("size_changed", this, nameof(QueueSort));
	}

	private void SortChildren()
	{
		var isLandscape = RectSize.x > RectSize.y;
		Console.WriteLine($"Sort Children {isLandscape} {RectSize}");
		const int paletteHeight = 50;
		const int messageWidth = 500;
		FitChildInRect(_colorPalette,
			new Rect2(0, RectSize.y - paletteHeight, RectSize.x - messageWidth, paletteHeight));
		FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x - messageWidth, RectSize.y - paletteHeight));
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
