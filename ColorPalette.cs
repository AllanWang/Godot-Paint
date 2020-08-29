using Godot;
using System;
using Godot.Collections;

public class ColorPalette : Control
{
	private ColorRect _colorRect;
	private GridContainer _colorGrid;

	/**
	 * 22 Color entries
	 */
	private readonly Color[] _colorPalette =
	{
		// First row
		Colors.White,
		Colors.Aqua,
		Colors.Aquamarine,
		Colors.Azure,
		Colors.Beige,
		Colors.Bisque,
		Colors.Aqua,
		Colors.Aquamarine,
		Colors.Azure,
		Colors.Beige,
		Colors.Bisque,
		// Second row
		Colors.Black,
		Colors.Aqua,
		Colors.Aquamarine,
		Colors.Azure,
		Colors.Beige,
		Colors.Bisque,
		Colors.Aqua,
		Colors.Aquamarine,
		Colors.Azure,
		Colors.Beige,
		Colors.Bisque,
	};

	public Color SelectedColor
	{
		get => _colorRect.Color;
		private set
		{
			_colorRect.Color = value;
			_colorRect.Update();
		}
	}

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_colorRect = (ColorRect) GetNode("ColorRect");
		_colorGrid = (GridContainer) GetNode("GridContainer");

		foreach (var color in _colorPalette)
		{
			var rect = new ColorRect {Color = color};
			_colorGrid.AddChild(rect);
			rect.Update();
			Console.WriteLine($"Add color {color}");
			rect.Connect("pressed", this, nameof(UpdateColor), new Godot.Collections.Array {color});
		}
		_colorGrid.Update();
	}

	public void UpdateColor(Color color)
	{
		SelectedColor = color;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
