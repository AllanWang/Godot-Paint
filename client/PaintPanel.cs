using Godot;
using System;

public class PaintPanel : Panel
{
    private Painter _painter;

    private Vector2 _lastMousePos;
    private bool _drawingLine;

    private PaintControls.BrushMode _brushMode = PaintControls.DEFAULT_BRUSH_MODE;
    private PaintControls.BrushSize _brushSize = PaintControls.DEFAULT_BRUSH_SIZE;
    private Color _color = ColorPalette.DEFAULT_COLOR;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _painter = new Painter(this);
    }

    public override void _GuiInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouse eventMouse:
                HandleMouseEvent(eventMouse);
                break;
        }
    }

    public void SetBrushMode(PaintControls.BrushMode brushMode)
    {
        _brushMode = brushMode;
    }

    public void SetBrushSize(PaintControls.BrushSize brushSize)
    {
        _brushSize = brushSize;
    }

    public void SetColor(Color color)
    {
        _color = color;
    }

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        var mousePos = eventMouse.Position;

        switch (eventMouse)
        {
            case InputEventMouseButton eventMouseButton:
                if (eventMouseButton.ButtonIndex == (int) ButtonList.Left)
                {
                    if (!_drawingLine && eventMouseButton.Pressed)
                    {
                        if (_brushMode == PaintControls.BrushMode.Pencil ||
                            _brushMode == PaintControls.BrushMode.Eraser)
                        {
                            _painter.AddLine(color: _color, thickness: (int) _brushSize, start: mousePos);
                            // Console.WriteLine($"New point {mousePos}");
                        }
                    }
                    else if (_drawingLine && !eventMouseButton.Pressed)
                    {
                        if (ShouldAcceptMousePos(mousePos))
                        {
                            _painter.AddPoint(mousePos);
                        }
                    }

                    _drawingLine = eventMouseButton.Pressed;
                }

                break;
            case InputEventMouseMotion eventMouseMotion:
                if (!ShouldAcceptMousePos(mousePos)) break;
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
                _painter?.Resize();
                break;
        }
    }

    public void undo_stroke()
    {
        _painter?.Remove();
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