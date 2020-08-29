using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PaintControl : Control
{
    abstract class Paint
    {
        public abstract void Add(Vector2 point);

        public abstract void Draw();
    }

    private class Line : Paint
    {
        private readonly Line2D line = new Line2D();

        public Line(Color color, float thickness, Vector2 start)
        {
            line.DefaultColor = color;
            line.Width = thickness;
            line.BeginCapMode = Line2D.LineCapMode.Round;
            line.EndCapMode = Line2D.LineCapMode.Round;
            line.JointMode = Line2D.LineJointMode.Round;
            line.AddPoint(start);
        }

        public override void Add(Vector2 point)
        {
            line.AddPoint(point);
        }

        public override void Draw()
        {
            line.Update();
        }
    }

    private List<Paint> paints = new List<Paint>();

    private const int UNDO_MOVE_SHAPE = -2;

    private const int test = 2;
    private const int UNDO_NONE = -1;
    private Vector2 IMAGE_SIZE = new Vector2(930, 720);

    // Enums for the various modes and brush shapes that can be applied.
    private enum BrushModes
    {
        PENCIL,
        ERASER,
        CIRCLE_SHAPE,
        RECTANGLE_SHAPE
    }

    private enum BrushShapes
    {
        RECTANGLE,
        CIRCLE
    }

    Node2D TL_node;

    Godot.Object[] brush_data_list = { };

    BrushModes brush_mode = BrushModes.PENCIL;

    BrushShapes brush_shape = BrushShapes.CIRCLE;

    float brush_size = 0;

    Color brush_color = Colors.Black;

    bool is_mouse_in_drawing_area = false;
    private Vector2 last_mouse_pos;
    private Vector2? mouse_click_start_pos;

    private bool undo_set = false;
    private int undo_element_list_num = -1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TL_node = (Node2D) GetNode("TLPos");
        SetProcess(true);
    }

    public override void _Process(float delta)
    {
        var mousePos = GetViewport().GetMousePosition();
        is_mouse_in_drawing_area = false;
        if (mousePos.x > TL_node.GlobalPosition.x && mousePos.y > TL_node.GlobalPosition.y)
        {
            is_mouse_in_drawing_area = true;
        }

        if (Input.IsMouseButtonPressed((int) ButtonList.Left))
        {
            if (mouse_click_start_pos == null)
            {
                mouse_click_start_pos = mousePos;
            }
            if (CheckIfMouseIsInsideCanvas())
            {
                if (mousePos.DistanceTo(last_mouse_pos) >= 0.2f)
                {
                    if (brush_mode == BrushModes.PENCIL || brush_mode == BrushModes.ERASER)
                    {
                        if (!undo_set)
                        {
                            undo_set = true;
                            undo_element_list_num = brush_data_list.Length;
                            paints.Add(new Line(color: brush_color, thickness: 10f, start: mousePos));
                            Console.WriteLine($"New point {mousePos}");
                        }
                        else
                        {
                            paints.Last().Add(mousePos);
                            Console.WriteLine($"Add point {mousePos}");
                        }
                        Update();
                    }
                }
            }
        }
        else
        {
            undo_set = false;
            mouse_click_start_pos = null;
        }

        last_mouse_pos = mousePos;
    }

    private bool CheckIfMouseIsInsideCanvas()
    {
        if (mouse_click_start_pos?.x > TL_node.GlobalPosition.x && mouse_click_start_pos?.y > TL_node.GlobalPosition.y)
        {
            if (is_mouse_in_drawing_area) return true;
        }

        return false;
    }

    public override void _Draw()
    {
        foreach (var paint in paints)
        {
          paint.Draw();
        }
    }
}