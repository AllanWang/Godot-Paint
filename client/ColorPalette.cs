using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ColorPalette : HBoxContainer
{
    private ColorRect _colorRect;
    private GridContainer _colorGrid;

    public static Color DEFAULT_COLOR = Colors.Black;

    /**
	 * 22 Color Palette
	 *
	 * https://en.wikipedia.org/wiki/List_of_Crayola_crayon_colors#Standard_colors
	 */
    private readonly Color[] _colorPalette =
        new List<string>
        {
            // First row
            "#FFFFFF" /* White */,
            "#C9C0BB" /* Silver */,
            "#FE4C40" /* Sunset Orange */,
            "#FFAE42" /* Yellow-Orange */,
            "#FBE870" /* Yellow */,
            "#C5E17A" /* Yellow-Green */,
            "#02A4D3" /* Cerulean */,
            "#4F69C6" /* Indigo */,
            "#8359A3" /* Violet */,
            "#FFA6C9" /* Carnation-Pink */,
            "#FDD5B1" /* Apricot */,
            // Second row
            "#000000" /* Black */,
            "#726A62" /* Charcoal Gray */,
            "#ED0A3F" /* Red */,
            "#FF681F" /* Red-Orange */,
            "#FED85D" /* Banana */,
            "#39A655" /* Green */,
            "#0095B7" /* Blue-Green */,
            "#0066FF" /* Blue */,
            "#6456B7" /* Blue-Violet */,
            "#F7468A" /* Violet-Red */,
            "#AF593E" /* Brown */,
        }.Select(s => new Color(s)).ToArray();

    [Signal]
    public delegate void SetColor(Color color);

    private Color _selectedColor;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _colorRect = (ColorRect) GetNode("ColorRect");
        _colorGrid = (GridContainer) GetNode("GridContainer");

        UpdateColor(DEFAULT_COLOR);

        foreach (var color in _colorPalette)
        {
            var cell = new ColorRectButton(color);
            _colorGrid.AddChild(cell);
            cell.Connect("pressed", this, nameof(UpdateColor), new Godot.Collections.Array {cell.Color});
        }
    }

    public void UpdateColor(Color color)
    {
        _selectedColor = color;
        _colorRect.Color = color;
        _colorRect.Update();
        EmitSignal(nameof(SetColor), color);
    }
}