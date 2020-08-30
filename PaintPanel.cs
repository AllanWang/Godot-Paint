using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PaintPanel : Panel
{
    abstract class Paint
    {
        public abstract CanvasItem CanvasItem { get; }

        public abstract void Add(Vector2 point);
    }

    private class Line : Paint
    {
        private readonly Line2D line = new Line2D();

        public override CanvasItem CanvasItem => line;

        public Line(Color color, float thickness, Vector2 start)
        {
            line.DefaultColor = color;
            line.Width = thickness;
            line.BeginCapMode = Line2D.LineCapMode.Round;
            line.EndCapMode = Line2D.LineCapMode.Round;
            line.JointMode = Line2D.LineJointMode.Round;
            line.Antialiased = true;
            // line.RoundPrecision = 20;
            line.AddPoint(start);
        }

        public override void Add(Vector2 point)
        {
            line.AddPoint(point);
        }
    }

    private class Paints
    {
        private readonly Node _control;
        private readonly List<Paint> _paints = new List<Paint>();

        public Paints(Node control) => _control = control;

        public void Add(Paint paint)
        {
            _paints.Add(paint);
            _control.AddChild(paint.CanvasItem);
        }

        public void AddPoint(Vector2 point)
        {
            var paint = _paints.Last();
            paint.Add(point);
            paint.CanvasItem.Update();
        }

        public void Remove()
        {
            if (!_paints.Any()) return;
            var paint = _paints.Last();
            _control.RemoveChild(paint.CanvasItem);
            _paints.RemoveAt(_paints.Count - 1);
        }
    }

    private Paints _paints;

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
        _paints = new Paints(this);
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
                    _drawingLine = eventMouseButton.Pressed;
                    if (_drawingLine)
                    {
                        if (brush_mode == BrushModes.PENCIL || brush_mode == BrushModes.ERASER)
                        {
                            _paints.Add(new Line(color: _colorPalette.SelectedColor, thickness: 3f,
                                start: mousePos));
                            // Console.WriteLine($"New point {mousePos}");
                        }
                    }
                }

                break;
            case InputEventMouseMotion eventMouseMotion:
                if (_drawingLine && mousePos.DistanceSquaredTo(_lastMousePos) >= 1f)
                {
                    _paints.AddPoint(eventMouseMotion.Position);
                }

                break;
        }

        _lastMousePos = mousePos;
    }

    public void undo_stroke()
    {
        _paints.Remove();
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