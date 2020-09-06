using Godot;
using System;

public class PaintRoot : Container
{
	private ColorPalette _colorPalette;
	private PaintControls _paintControls;
	private PaintPanel _paintPanel;
	private Messages _messages;

	private ClientNetwork _network;

	private static readonly float PANEL_HEIGHT_WIDTH_RATIO = 3f / 4f;
	private static readonly float PALETTE_HEIGHT_WIDTH_RATIO = 1f / 7f;

	public override void _Ready()
	{
		_colorPalette = (ColorPalette) GetNode("ColorPalette");
		_paintControls = (PaintControls) GetNode("PaintControls");
		_paintPanel = (PaintPanel) GetNode("PaintPanel");
		_messages = (Messages) GetNode("Messages");
		_network = (ClientNetwork) GetNode("/root/PaintNetwork");

		_paintControls.Connect(nameof(PaintControls.SetBrushMode), _paintPanel, nameof(PaintPanel.SetBrushMode));
		_paintControls.Connect(nameof(PaintControls.SetBrushSize), _paintPanel, nameof(PaintPanel.SetBrushSize));
		_colorPalette.Connect(nameof(ColorPalette.SetColor), _paintPanel, nameof(PaintPanel.SetColor));
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
		var panelHeight = RectSize.x * PANEL_HEIGHT_WIDTH_RATIO;
		var paletteHeight = _colorPalette.GetCombinedMinimumSize().y;
		var controlsHeight = _paintControls.GetCombinedMinimumSize().y;
		var messageHeight = RectSize.y - panelHeight - paletteHeight - controlsHeight;

		FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x, panelHeight));
		FitChildInRect(_colorPalette,
			new Rect2(0, panelHeight, RectSize.x, paletteHeight));
		FitChildInRect(_paintControls,
			new Rect2(0, panelHeight + paletteHeight, RectSize.x, controlsHeight));
		FitChildInRect(_messages,
			new Rect2(0, panelHeight + paletteHeight + controlsHeight, RectSize.x, messageHeight));
	}

	private void SortLandscape()
	{
		var padding = 16f;
		var messageWidth = _messages.GetCombinedMinimumSize().x;
		var toolsHeight = Math.Max(_colorPalette.GetCombinedMinimumSize().y, _paintControls.GetCombinedMinimumSize().y);
		var paletteWidth = toolsHeight / PALETTE_HEIGHT_WIDTH_RATIO;
		// Height is either max based on width, or max based on available height
		var panelHeight = Math.Min((RectSize.x - messageWidth) * PANEL_HEIGHT_WIDTH_RATIO,
			RectSize.y - toolsHeight - padding * 2);
		var panelWidth = panelHeight / PANEL_HEIGHT_WIDTH_RATIO;
		var controlsWidth = panelWidth - paletteWidth;


		FitChildInRect(_paintPanel, new Rect2(0, 0, panelWidth, panelHeight));
		FitChildInRect(_colorPalette,
			new Rect2(padding, padding + panelHeight, paletteWidth, toolsHeight));
		FitChildInRect(_paintControls,
			new Rect2(padding * 2 + paletteWidth, padding + panelHeight, controlsWidth, toolsHeight));
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
