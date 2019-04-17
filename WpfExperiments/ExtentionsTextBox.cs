using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExperiments
{
    public static class ExtentionsTextBox
    {
        public static readonly DependencyProperty HintProp = 
            DependencyProperty.RegisterAttached(
            "Hint", typeof(object), typeof(ExtentionsTextBox), 
            new FrameworkPropertyMetadata(default(string), 
            FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets the hint.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetHint(DependencyObject element, object value)
        {
            element.SetValue(HintProp, value);
        }

        /// <summary>
        /// Gets the hint.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public static object GetHint(DependencyObject element)
        {
            return element.GetValue(HintProp);
        }
    }
}
