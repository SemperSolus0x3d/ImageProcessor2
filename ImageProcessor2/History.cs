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

        private LinkedList<Image> previousStates;
        private LinkedList<Image> futureStates;

        public History()
        {
            previousStates = new LinkedList<Image>();
            futureStates = new LinkedList<Image>();

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
                previousStates.AddLast(CurrentState);

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

            futureStates.AddLast(CurrentState);
            CurrentState = previousStates.Last?.Value;

            previousStates.RemoveLast();

            return CurrentState;
        }

        public Image GoForward()
        {
            if (futureStates.Count == 0)
                return CurrentState;

            previousStates.AddLast(CurrentState);
            CurrentState = futureStates.Last?.Value;

            futureStates.RemoveLast();

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
