using Godot;
using System;

public class PaintRoot : Container
{
	private ColorPalette _colorPalette;
	private PaintPanel _paintPanel;
	private Messages _messages;

	private ClientNetwork _network;

	public override void _Ready()
	{
		_colorPalette = (ColorPalette) GetNode("ColorPalette");
		_paintPanel = (PaintPanel) GetNode("PaintPanel");
		_messages = (Messages) GetNode("Messages");
		_network = (ClientNetwork) GetNode("/root/PaintNetwork");
	}

	/**
	 * Manual rearrangement
	 *
	 * Note that the PaintPanel must have the same aspect ratio, or a user will be able to draw in areas not visible to everyone.
	 * For now, we will keep the ratio at 4:3
	 */
	private void SortChildren()
	{
		var isLandscape = RectSize.x > RectSize.y;
		if (isLandscape) SortLandscape();
		else SortPortrait();
	}

	private void SortPortrait()
	{
		var panelHeight = RectSize.x * 0.75f;
		var paletteHeight = 50f;
		var messageHeight = RectSize.y - panelHeight - paletteHeight;

		FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x, panelHeight));
		FitChildInRect(_colorPalette,
			new Rect2(0, panelHeight, RectSize.x, paletteHeight));
		FitChildInRect(_messages, new Rect2(0, panelHeight + paletteHeight, RectSize.x, messageHeight));
	}

	private void SortLandscape()
	{
		const float messageWidth = 500;
		var paletteHeight = 50f;
		// Height is either max based on width, or max based on available height
		var panelHeight = Math.Min((RectSize.x - messageWidth) * 0.75f, RectSize.y - paletteHeight);
		var panelWidth = panelHeight * 4 / 3;


		FitChildInRect(_paintPanel, new Rect2(0, 0, panelWidth, panelHeight));
		FitChildInRect(_colorPalette,
			new Rect2(0, panelHeight, panelWidth, paletteHeight));
		FitChildInRect(_messages, new Rect2(panelWidth, 0, messageWidth, RectSize.y));
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
