namespace Deployer.Raspberry.Gui.ViewModels
{
    public class MessageViewModel
    {
        public string Title { get; }
        public string Text { get; }

        public MessageViewModel(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }
}