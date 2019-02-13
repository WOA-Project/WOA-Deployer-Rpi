using System;

namespace Deployer
{
    public class TaskDescriptionAttribute : Attribute
    {
        public TaskDescriptionAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}