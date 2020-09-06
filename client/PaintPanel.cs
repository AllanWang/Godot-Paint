using Godot;
using System;

public class PaintPanel : Panel
{
    private Painter _painter;

    // Enums for the various modes and brush shapes that can be applied.
    private enum BrushModes
    {
        PENCIL,
        ERASER,
        CIRCLE_SHAPE,
        RECTANGLE_SHAPE
    }

    BrushModes brush_mode = BrushModes.PENCIL;

    private Vector2 _lastMousePos;
    private bool _drawingLine;

    private ColorPalette _colorPalette;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _painter = new Painter(this);
        _colorPalette = (ColorPalette) GetParent().GetNode("ColorPalette");
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouse eventMouse:
                HandleMouseEvent(eventMouse);
                break;
        }
    }

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        var mousePos = eventMouse.Position;
        if (!ShouldAcceptMousePos(mousePos)) return;

        switch (eventMouse)
        {
            case InputEventMouseButton eventMouseButton:
                if (eventMouseButton.ButtonIndex == (int) ButtonList.Left)
                {
                    if (!_drawingLine && eventMouseButton.Pressed)
                    {
                        if (brush_mode == BrushModes.PENCIL || brush_mode == BrushModes.ERASER)
                        {
                            _painter.Add(new Painter.Line(color: _colorPalette.SelectedColor, thickness: 3f), mousePos);
                            // Console.WriteLine($"New point {mousePos}");
                        }
                    }
                    else if (_drawingLine && !eventMouseButton.Pressed)
                    {
                        _painter.AddPoint(mousePos);
                    }

                    _drawingLine = eventMouseButton.Pressed;
                }

                break;
            case InputEventMouseMotion eventMouseMotion:
                if (_drawingLine && mousePos.DistanceSquaredTo(_lastMousePos) >= 1f)
                {
                    _painter.AddPoint(mousePos);
                }

                break;
        }

        _lastMousePos = mousePos;
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case NotificationResized:
                _painter.Resize();
                break;
        }
    }

    public void undo_stroke()
    {
        _painter.Remove();
    }

    private bool ShouldAcceptMousePos(Vector2 mousePos)
    {
        // TODO threshold should be at least radius of largest font size + 1
        const float threshold = 100f;
        var size = RectSize;
        if (mousePos.x < -threshold || mousePos.x > size.x + threshold) return false;
        if (mousePos.y < -threshold || mousePos.y > size.y + threshold) return false;
        return true;
    }
}