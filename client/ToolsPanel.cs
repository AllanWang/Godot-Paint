using Godot;
using System;

public class ToolsPanel : Panel
{
	private Button _buttonUndo;
	private Button _buttonSave;
	private PaintPanel _paintPanel;

	public enum ButtonActions
	{
		Undo
	}

	public override void _Ready()
	{
		_buttonUndo = (Button) GetNode("ButtonUndo");
		_buttonSave = (Button) GetNode("ButtonSave");
		_paintPanel = (PaintPanel) GetParent().GetNode("PaintPanel");

		_buttonUndo.Connect("pressed", this, nameof(ButtonPressed), new Godot.Collections.Array {ButtonActions.Undo});

		// Set physics process so we can update the status label.
		SetPhysicsProcess(true);
	}

	public void ButtonPressed(ButtonActions name)
	{
		switch (name)
		{
			case ButtonActions.Undo:
				_paintPanel.undo_stroke();
				break;
		}
	}
}
