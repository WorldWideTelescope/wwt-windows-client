using System;
using System.Collections.Generic;
using System.Text;

namespace TerraViewer
{
    public class Undo
    {
        static Stack<IUndoStep> undoStack = new Stack<IUndoStep>();
        static Stack<IUndoStep> redoStack = new Stack<IUndoStep>();
        
        public static void Clear()
        {
            undoStack = new Stack<IUndoStep>();
            redoStack = new Stack<IUndoStep>();
        }
        
        public static void Push(IUndoStep step)
        {
            undoStack.Push(step);
            redoStack = new Stack<IUndoStep>();
        }

        public static string PeekActionString()
        {
            if (undoStack.Count > 0)
            {
                return undoStack.Peek().ToString();
            }
            else
            {
                //todo localize
                return Language.GetLocalizedText(551, "Nothing to Undo");
            }
        }

        public static string PeekRedoActionString()
        {
            if (redoStack.Count > 0)
            {
                return redoStack.Peek().ToString();
            }
            else
            {
                return "";
            }
        }

        public static bool PeekAction()
        {
            return (undoStack.Count > 0);
        }

        public static bool PeekRedoAction()
        {
            return (redoStack.Count > 0);
        }

        public static void StepBack()
        {
            var step = undoStack.Pop();

            step.Undo();

            redoStack.Push(step);
        }
       
        public static void StepForward()
        {
            var step = redoStack.Pop();

            step.Redo();

            undoStack.Push(step);
        }
    }

    public class UndoStep : TerraViewer.IUndoStep
    {
        public UndoStep()
        {
        }
        public void Undo()
        {
        }
        public void Redo()
        {
        }
        override public string ToString()
        {
            //todo localize
            return Language.GetLocalizedText(551, "Nothing to Undo");
        }
    }


}
