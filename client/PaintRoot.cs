using Godot;
using System;

public class PaintRoot : Control
{
	private ClientNetwork _network;

	private struct PaintLayout
	{
		internal Node Root;
		internal Node ColorPanelContainer;
		internal Node ColorToolsContainer;
		internal Node MessagesContainer;
	}

	private PaintLayout _landscapeLayout;
	private PaintLayout _currentLayout;

	private ColorPalette _colorPalette;
	private PaintPanel _paintPanel;
	private Messages _messages;
	private PaintControls _paintControls;

	public override void _Ready()
	{
		_network = (ClientNetwork) GetNode("/root/PaintNetwork");

		_landscapeLayout = LandscapeLayout();

		_currentLayout = _landscapeLayout;

		_colorPalette = (ColorPalette) _currentLayout.ColorToolsContainer.GetNode("ColorPalette");
		_paintPanel = (PaintPanel) _currentLayout.ColorPanelContainer.GetNode("PaintPanel");
		_messages = (Messages) _currentLayout.MessagesContainer.GetNode("Messages");
		_paintControls = (PaintControls) _currentLayout.ColorToolsContainer.GetNode("PaintControls");

		_paintControls.Connect(nameof(PaintControls.SetBrushMode), _paintPanel, nameof(PaintPanel.SetBrushMode));
		_paintControls.Connect(nameof(PaintControls.SetBrushSize), _paintPanel, nameof(PaintPanel.SetBrushSize));
		_colorPalette.Connect(nameof(ColorPalette.SetColor), _paintPanel, nameof(PaintPanel.SetColor));
	}

	private PaintLayout LandscapeLayout()
	{
		var landscape = GetNode("Landscape");

		return new PaintLayout
		{
			Root = landscape,
			ColorPanelContainer = landscape.GetNode("HC/VC/PaintPanelContainer"),
			ColorToolsContainer = landscape.GetNode("HC/VC/PaintToolsContainer"),
			MessagesContainer = landscape.GetNode("HC/MessagesContainer")
		};
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
		// var panelHeight = RectSize.x * 0.75f;
		// var paletteHeight = 50f;
		// var messageHeight = RectSize.y - panelHeight - paletteHeight;
		//
		// FitChildInRect(_paintPanel, new Rect2(0, 0, RectSize.x, panelHeight));
		// FitChildInRect(_colorPalette,
		//     new Rect2(0, panelHeight, RectSize.x, paletteHeight));
		// FitChildInRect(_messages, new Rect2(0, panelHeight + paletteHeight, RectSize.x, messageHeight));
	}

	private void SortLandscape()
	{
		// const float messageWidth = 500;
		// var paletteHeight = 50f;
		// // Height is either max based on width, or max based on available height
		// var panelHeight = Math.Min((RectSize.x - messageWidth) * 0.75f, RectSize.y - paletteHeight);
		// var panelWidth = panelHeight * 4 / 3;
		//
		//
		// FitChildInRect(_paintPanel, new Rect2(0, 0, panelWidth, panelHeight));
		// FitChildInRect(_colorPalette,
		//     new Rect2(0, panelHeight, panelWidth, paletteHeight));
		// FitChildInRect(_messages, new Rect2(panelWidth, 0, messageWidth, RectSize.y));
	}

	private void HandleResize()
	{
	}

	public override void _Notification(int what)
	{
		switch (what)
		{
			case NotificationResized:
				HandleResize();
				break;
		}
	}
}
