using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Control wrapper to allow for canvas drawing.
 *
 * The painter takes in paints and styles, and is in charge of adding child nodes
 * to draw all elements.
 *
 * The control is expected to have the same ratio regardless of size, and the painter will scale the drawing to fit the control;
 * that way, every client will see the same image.
 */
public class Painter
{
    private readonly Control _control;
    private readonly List<Paint> _paints = new List<Paint>();
    private Vector2 _scale = new Vector2 {x = 1, y = 1};

    public Painter(Control control) => _control = control;

    public void AddLine(Color color, float thickness, Vector2 start)
    {
        Add(new Line(this, color, thickness), start);
    }

    private void Add(Paint paint, Vector2 start)
    {
        _paints.Add(paint);
        AddPoint(start);
        _control.AddChild(paint.CanvasItem);
    }

    private Vector2 Actual(Vector2 normalized)
    {
        return new Vector2(normalized.x * _scale.x, normalized.y * _scale.y);
    }

    private Vector2 Normalize(Vector2 vector)
    {
        return new Vector2(vector.x / _scale.x, vector.y / _scale.y);
    }

    private float Actual(float normalized)
    {
        return normalized * _scale.x;
    }

    private float Normalize(float vector)
    {
        return vector / _scale.x;
    }

    public void AddPoint(Vector2 point)
    {
        var paint = _paints.Last();
        paint.Add(point);
        paint.CanvasItem.Update();
    }

    public void Resize()
    {
        _scale.x = _control.RectSize.x;
        _scale.y = _control.RectSize.y;
        foreach (var paint in _paints)
        {
            paint.Scale();
        }
    }

    public void Remove()
    {
        if (!_paints.Any()) return;
        var paint = _paints.Last();
        _control.RemoveChild(paint.CanvasItem);
        _paints.RemoveAt(_paints.Count - 1);
    }

    private abstract class Paint
    {
        protected  readonly Painter Painter;

        protected  Paint(Painter painter)
        {
            Painter = painter;
        }

        public abstract CanvasItem CanvasItem { get; }

        public abstract void Add(Vector2 point);

        public abstract void Scale();
    }

    private class Line : Paint
    {
        private readonly Line2D _line = new Line2D();

        private readonly List<Vector2> _normalizedPoints = new List<Vector2>();
        private readonly float _normalizedThickness;

        public override CanvasItem CanvasItem => _line;

        public Line(Painter painter, Color color, float thickness) : base(painter)
        {
            _normalizedThickness = painter.Normalize(thickness);
            _line.SetAsToplevel(true);
            _line.DefaultColor = color;
            _line.Width = thickness;
            _line.BeginCapMode = Line2D.LineCapMode.Round;
            _line.EndCapMode = Line2D.LineCapMode.Round;
            _line.JointMode = Line2D.LineJointMode.Round;
            _line.Antialiased = true;
            // line.RoundPrecision = 20;
        }

        public override void Add(Vector2 point)
        {
            _line.AddPoint(point);
            _normalizedPoints.Add(Painter.Normalize(point));
        }

        public override void Scale()
        {
            var newPoints = _normalizedPoints.Select(p => Painter.Actual(p)).ToArray();
            _line.Points = newPoints;
            _line.Width = Painter.Actual(_normalizedThickness);
        }
    }
}