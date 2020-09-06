using Godot;
using System;

public class PaintControls : HBoxContainer
{
	public enum BrushMode
	{
		Pencil,
		Eraser,
		Paint
	}

	public static BrushMode DEFAULT_BRUSH_MODE = BrushMode.Pencil;
	public static BrushSize DEFAULT_BRUSH_SIZE = BrushSize.Medium;

	public enum BrushSize
	{
		Small = 1, // 8
		Medium = 3, // 14
		Large = 5, // 24
		Huge = 7 // 28
	}

	private BrushMode _brushMode = DEFAULT_BRUSH_MODE;
	private BrushSize _brushSize = DEFAULT_BRUSH_SIZE;

	[Signal]
	public delegate void SetBrushMode(BrushMode brushMode);

	[Signal]
	public delegate void SetBrushSize(BrushSize brushSize);

	private class SpriteButton
	{
		protected internal readonly TextureButton Button;
		private readonly AnimatedSprite Sprite;

		protected internal SpriteButton(Node parent, string name)
		{
			Button = (TextureButton) parent.GetNode(name);
			Sprite = (AnimatedSprite) Button.GetNode($"{name}Sprite");
			Sprite.Playing = true;
		}
	}

	private SpriteButton _pencil;
	private SpriteButton _eraser;
	private SpriteButton _paint;

	private SpriteButton _small;
	private SpriteButton _medium;
	private SpriteButton _large;
	private SpriteButton _huge;

	public override void _Ready()
	{
		_pencil = new SpriteButton(this, "Pencil");
		_eraser = new SpriteButton(this, "Eraser");
		_paint = new SpriteButton(this, "Paint");

		_small = new SpriteButton(this, "Small");
		_medium = new SpriteButton(this, "Medium");
		_large = new SpriteButton(this, "Large");
		_huge = new SpriteButton(this, "Huge");

		BrushModeConnect(_pencil, BrushMode.Pencil);
		BrushModeConnect(_eraser, BrushMode.Eraser);
		BrushModeConnect(_paint, BrushMode.Paint);

		BrushSizeConnect(_small, BrushSize.Small);
		BrushSizeConnect(_medium, BrushSize.Medium);
		BrushSizeConnect(_large, BrushSize.Large);
		BrushSizeConnect(_huge, BrushSize.Huge);
	}

	private void BrushModeConnect(SpriteButton spriteButton, BrushMode brushMode)
	{
		spriteButton.Button.Connect("pressed", this, nameof(SetBrushModeInternal),
			new Godot.Collections.Array {brushMode});
	}

	private void SetBrushModeInternal(BrushMode brushMode)
	{
		_brushMode = brushMode;
		EmitSignal(nameof(SetBrushMode), brushMode);
	}

	private void BrushSizeConnect(SpriteButton spriteButton, BrushSize brushSize)
	{
		spriteButton.Button.Connect("pressed", this, nameof(SetBrushSizeInternal),
			new Godot.Collections.Array {brushSize});
	}

	private void SetBrushSizeInternal(BrushSize brushSize)
	{
		_brushSize = brushSize;
		EmitSignal(nameof(SetBrushSize), brushSize);
	}
}
