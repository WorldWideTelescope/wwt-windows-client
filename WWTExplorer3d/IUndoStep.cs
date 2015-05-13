using System;
namespace TerraViewer
{
    public interface IUndoStep
    {
        void Redo();
        string ToString();
        void Undo();
    }
}
