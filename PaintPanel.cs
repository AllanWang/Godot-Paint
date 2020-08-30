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
                var mousePos = eventMouse.Position;
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
                                    Console.WriteLine($"New point {mousePos}");
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

                break;
        }
    }

    public void undo_stroke()
    {
        _paints.Remove();
    }

    private bool CheckIfMouseIsInsideCanvas()
    {
        // TODO only accept points if they are within x pixels of border
        // That way we don't need to save additional points too far out
        return true;
        // if (mouse_click_start_pos?.x > TL_node.GlobalPosition.x && mouse_click_start_pos?.y > TL_node.GlobalPosition.y)
        // {
        // 	if (is_mouse_in_drawing_area) return true;
        // }
        //
        // return false;
    }
}