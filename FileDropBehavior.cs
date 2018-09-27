using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Asparlose.Wpf
{
    public class FileDropBehavior : Behavior<FrameworkElement>
    {
        [Obsolete]
        public ICollection<string> FileCollection
        {
            get => (ICollection<string>)(GetValue(FileCollectionProperty));
            set => SetValue(FileCollectionProperty, value);
        }

        [Obsolete]
        public static readonly DependencyProperty FileCollectionProperty
            = DependencyProperty.Register(nameof(FileCollection), typeof(ICollection<string>), typeof(FileDropBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FileDropBehavior));

        public FileDropBehavior()
        {
#pragma warning disable 0612
            FileCollection = new List<string>();
#pragma warning restore 0612
            Drop += FileDropBehavior_Drop;
        }

        private void FileDropBehavior_Drop(object sender, FileDropEventArgs e)
        {
            foreach (var i in e.Files)
#pragma warning disable 0612
                FileCollection?.Add(i);
#pragma warning restore 0612
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Drop += AssociatedObject_Drop;
            AssociatedObject.PreviewDragOver += AssociatedObject_PreviewDragOver;
            if (!AssociatedObject.AllowDrop)
                Debug.WriteLine($"FileDropBehavior の適用されたオブジェクト（{AssociatedObject}）の AllowDrop が false です。", "Warn");
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Drop -= AssociatedObject_Drop;
            AssociatedObject.PreviewDragOver -= AssociatedObject_PreviewDragOver;
        }

        public event EventHandler<FileDropEventArgs> Drop;

        protected virtual void OnDrop(IEnumerable<string> files)
        {
            if (Command == null)
            {
                Drop?.Invoke(this, new FileDropEventArgs(files));
            }
            else
            {
                if (Command?.CanExecute(files) ?? false)
                {
                    Command.Execute(files);
                    Drop?.Invoke(this, new FileDropEventArgs(files));
                }
            }
        }

        private void AssociatedObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                if (Command != null
                    && e.Data.GetData(DataFormats.FileDrop) is string[] files
                    && !Command.CanExecute(files))
                {
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                OnDrop(files);
        }
    }
}
