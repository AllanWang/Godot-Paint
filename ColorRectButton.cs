using Godot;
using System;

public class ColorRectButton : TextureButton
{
    public readonly Color Color;

    private static Vector2 vec1 = new Vector2(1, 1);

    public ColorRectButton(Color color) => Color = color;

    public override void _Ready()
    {
        TextureNormal = ColorTexture(Color);
        TextureHover = ColorTexture(Color);
        TexturePressed = ColorTexture(Color);

        Expand = true;
        StretchMode = StretchModeEnum.Scale;

        RectMinSize = vec1;
        RectSize = vec1;
        SizeFlagsVertical = (int) SizeFlags.ExpandFill;
        SizeFlagsHorizontal = (int) SizeFlags.ExpandFill;
    }

    private Texture ColorTexture(Color color)
    {
        return new GradientTexture {Gradient = new Gradient {Colors = new[] {color}}};
    }
}