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

namespace System.Windows.Controls;

/// <summary>
/// Provides data for the <see cref="VirtualizingStackPanel.CleanUpVirtualizedItemEvent"/> event.
/// </summary>
public class CleanUpVirtualizedItemEventArgs : RoutedEventArgs
{
    internal CleanUpVirtualizedItemEventArgs(object value, UIElement element)
    {
        Value = value;
        UIElement = element;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether this item should not be re-virtualized.
    /// </summary>
    /// <returns>
    /// true if you want to prevent revirtualization of this item; otherwise false.
    /// </returns>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets an instance of the visual element that represents the data value.
    /// </summary>
    /// <returns>
    /// The <see cref="Windows.UIElement"/> that represents the data value.
    /// </returns>
    public UIElement UIElement { get; }

    /// <summary>
    /// Gets an <see cref="object"/> that represents the original data value.
    /// </summary>
    /// <returns>
    /// The <see cref="object"/> that represents the original data value.
    /// </returns>
    public object Value { get; }
}