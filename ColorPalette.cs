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
        Colors.LightGray,
        Colors.Red,
        Colors.Orange,
        Colors.Yellow,
        Colors.Green,
        Colors.Turquoise,
        Colors.Blue,
        Colors.MediumPurple,
        Colors.Pink,
        Colors.SandyBrown,
        // Second row
        Colors.Black,
        Colors.DarkGray,
        Colors.DarkRed,
        Colors.Firebrick,
        Colors.PeachPuff,
        Colors.DarkGreen,
        Colors.DarkTurquoise,
        Colors.DarkBlue,
        Colors.Purple,
        Colors.DeepPink,
        Colors.Brown,
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
            var cell = new ColorRectButton(color);
            _colorGrid.AddChild(cell);
            cell.Connect("pressed", this, nameof(UpdateColor), new Godot.Collections.Array {cell.Color});
        }

        SetProcess(true);
        Update();
    }

    public void UpdateColor(Color color)
    {
        SelectedColor = color;
    }

    // public override void _Draw()
    // {
    // 	Console.WriteLine("Draw asdf");
    // 	DrawRect(new Rect2(0, 0, 2000, 2000), Colors.Aqua);
    // 	base._Draw();
    // }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}