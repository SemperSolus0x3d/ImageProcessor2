using ImageProcessor2.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageProcessor2
{
    public class History : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Image CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                OnPropertyChanged();
            }
        }
        private Image currentState;

        private Stack<Image> previousStates;
        private Stack<Image> futureStates;

        public History()
        {
            previousStates = new Stack<Image>();
            futureStates = new Stack<Image>();

            CurrentState = null;
        }

        public void Clear()
        {
            previousStates.Clear();
            futureStates.Clear();

            /*
             * I know that calling GC.Collect()
             * manually is not recommended, but
             * I have to do that just in case
             * there are high-resolution images
             * which consume a lot of RAM and
             * therefore have to be collected ASAP.
             */
            GC.Collect();
        }

        public void Save(Image image)
        {
            if (image == null)
                throw new ArgumentException(
                    $"{nameof(image)} was null"
                );

            if (CurrentState != null)
                previousStates.Push(CurrentState);

            CurrentState = image;
            
            if (futureStates.Count > 0)
            {
                futureStates.Clear();

                /*
                 * I know that calling GC.Collect()
                 * manually is not recommended, but
                 * I have to do that just in case
                 * there are high-resolution images
                 * which consume a lot of RAM and
                 * therefore have to be collected ASAP.
                 */
                GC.Collect();
            }
        }

        public Image GoBack()
        {
            if (previousStates.Count == 0)
                return CurrentState;

            futureStates.Push(CurrentState);
            CurrentState = previousStates.Pop();

            return CurrentState;
        }

        public Image GoForward()
        {
            if (futureStates.Count == 0)
                return CurrentState;

            previousStates.Push(CurrentState);
            CurrentState = futureStates.Pop();

            return CurrentState;
        }

        private void OnPropertyChanged(
            [CallerMemberName]string propertyName = ""
        )
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}
