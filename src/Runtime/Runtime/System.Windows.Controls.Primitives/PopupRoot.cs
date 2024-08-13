﻿
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Collections.Generic;
using System.Windows.Input;
using CSHTML5.Internal;
using DotNetForHtml5.Core;

namespace System.Windows.Controls.Primitives
{
    internal sealed class PopupRoot : FrameworkElement
    {
        protected override int VisualChildrenCount => Content is null ? 0 : 1;

        protected override UIElement GetVisualChild(int index)
        {
            if (index != 0 || Content is not UIElement content)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return content;
        }

        internal Popup ParentPopup { get; set; }

        internal PopupRoot(Window parentWindow, Popup popup)
        {
            BypassLayoutPolicies = true;

            ParentWindow = parentWindow;
            ParentPopup = popup;
        }

        /// <summary>
        /// Gets or sets the visual root of a popup
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValueInternal(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupRoot.Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(UIElement),
                typeof(PopupRoot),
                new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PopupRoot popupRoot = (PopupRoot)d;
            
            if (e.OldValue is UIElement oldContent)
            {
                PropagateSuspendLayout(oldContent);
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldContent, popupRoot);
                oldContent.UpdateIsVisible();
            }

            if (e.NewValue is UIElement newContent)
            {
                PropagateResumeLayout(popupRoot, newContent);
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newContent, popupRoot);
                newContent.UpdateIsVisible();
            }

            popupRoot.SetLayoutSize();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
            // In other words, the parents of a popup that has StayOpen=True will always stay open
            // regardless of the value of their "StayOpen" property.

            HashSet<Popup> listOfPopupThatMustBeClosed = new HashSet<Popup>();
            List<PopupRoot> popupRootList = new List<PopupRoot>();

            foreach (object obj in PopupsManager.GetAllRootUIElements())
            {
                if (obj is PopupRoot)
                {
                    PopupRoot root = (PopupRoot)obj;
                    popupRootList.Add(root);

                    if (root.ParentPopup != null)
                        listOfPopupThatMustBeClosed.Add(root.ParentPopup);
                }
            }

            // We determine which popup needs to stay open after this click
            foreach (PopupRoot popupRoot in popupRootList)
            {
                if (popupRoot.ParentPopup != null)
                {
                    // We must prevent all the parents of a popup to be closed when:
                    // - this popup is set to StayOpen
                    // - or the click happend in this popup

                    Popup popup = popupRoot.ParentPopup;

                    if (popup.StayOpen)
                    {
                        do
                        {
                            if (!listOfPopupThatMustBeClosed.Contains(popup))
                                break;

                            listOfPopupThatMustBeClosed.Remove(popup);

                            popup = popup.ParentPopup;

                        } while (popup != null);
                    }
                }
            }

            foreach (Popup popup in listOfPopupThatMustBeClosed)
            {
                var args = new OutsideClickEventArgs();
                popup.OnOutsideClick(args);
                if (!args.Handled)
                {
                    popup.CloseFromAnOutsideClick();
                }
            }
        }

        private void SetLayoutSize()
        {
            if (Content is UIElement content)
            {
                content.InvalidateMeasure();
                content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                content.Arrange(new Rect(new Point(), content.DesiredSize));
                content.UpdateLayout();
            }
        }

        internal void PutPopupInFront()
        {
            string parentDiv = OpenSilver.Interop.GetVariableStringForJS(ParentWindow.RootDomElement);
            string popupDiv = OpenSilver.Interop.GetVariableStringForJS(OuterDiv);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{parentDiv}.appendChild({popupDiv})");
        }
    }
}
