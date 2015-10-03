namespace TerraViewer
{
    using System;

    static public class Patterns
    {
        /// <summary>
        /// This is not necessary a good pattern, actually its terrible! but needed to refactor for now :)
        /// </summary>
        static public void ActIgnoringExceptions(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                action();
            }
            catch (Exception)
            {
                // TODO: trace
            }
        }
    }
}
