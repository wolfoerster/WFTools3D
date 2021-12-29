//******************************************************************************************
// Copyright © 2016 - 2022 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the WFTools3D project which can be found on github.com.
//
// WFTools3D is free software: you can redistribute it and/or modify it under the terms 
// of the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.
// 
// WFTools3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//******************************************************************************************

namespace WFTools3D
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// A spin box for doubles. Composed of a Label, a TextBox and a ScrollBar.
    /// </summary>
    public class NumberBox : Grid
    {
        #region DependencyProperty Number

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public double Number
        {
            get { return (double)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        /// <summary>
        /// The NumberProperty.
        /// </summary>
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(double), typeof(NumberBox),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnNumberChanged, OnNumberCoerce));

        static void OnNumberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = obj as NumberBox;
            if (numberBox != null)
                numberBox.MyNumberChanged();
        }

        internal void MyNumberChanged()
        {
            textBox.Text = Number.ToString(FormatString);
            scrollBar.Value = Invert(Number);

            NumberChanged?.Invoke(this, null);
        }

        static object OnNumberCoerce(DependencyObject d, object baseValue)
        {
            NumberBox numberBox = (NumberBox)d;
            double value = (double)baseValue;
            value = MathUtils.Clamp(value, numberBox.Minimum, numberBox.Maximum);
            return value;
        }

        #endregion DependencyProperty Number

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberBox"/> class.
        /// </summary>
        public NumberBox()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            label = new TextBlock();
            label.VerticalAlignment = VerticalAlignment.Center;
            SetColumn(label, 0);
            Children.Add(label);

            textBox = new TextBox();
            textBox.TextChanged += TextBoxTextChanged;
            SetColumn(textBox, 1);
            Children.Add(textBox);

            scrollBar = new ScrollBar();
            scrollBar.Focusable = true;
            scrollBar.ContextMenu = null;
            scrollBar.Scroll += ScrollBarScroll;
            scrollBar.Margin = new Thickness(0, 1, 0, 0);
            scrollBar.MouseRightButtonDown += ScrollBarMouseRightButtonDown;
            SetColumn(scrollBar, 2);
            Children.Add(scrollBar);

            FormatString = "F0";
            Minimum = 0;
            Maximum = 100;
            SmallChange = 1;
            LargeChange = 10;
        }
        TextBlock label;
        TextBox textBox;
        ScrollBar scrollBar;

        void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            double number;
            if (double.TryParse(textBox.Text, out number))
                Number = number;
        }

        void ScrollBarScroll(object sender, ScrollEventArgs e)
        {
            scrollBar.Focus();
            Number = Invert(scrollBar.Value);
        }

        void ScrollBarMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(scrollBar);
            if (pt.Y > scrollBar.ActualHeight * 0.5)
                Number -= scrollBar.LargeChange;
            else
                Number += scrollBar.LargeChange;
        }

        /// <summary>
        /// Gets or sets the value to be added to or subtracted from the Number property when the scroll box is moved a small distance.
        /// </summary>
        public double SmallChange
        {
            get { return scrollBar.SmallChange; }
            set { scrollBar.SmallChange = value; }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the Number property when the scroll box is moved a large distance.
        /// </summary>
        public double LargeChange
        {
            get { return scrollBar.LargeChange; }
            set { scrollBar.LargeChange = value; }
        }

        /// <summary>
        /// Gets or sets the lower limit of values of the scrollable range.
        /// </summary>
        public double Minimum
        {
            get { return scrollBar.Minimum; }
            set
            {
                if (scrollBar.Minimum != value)
                {
                    scrollBar.Minimum = value;
                    MyNumberChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the upper limit of values of the scrollable range.
        /// </summary>
        public double Maximum
        {
            get { return scrollBar.Maximum; }
            set
            {
                if (scrollBar.Maximum != value)
                {
                    scrollBar.Maximum = value;
                    MyNumberChanged();
                }
            }
        }

        /// <summary>
        /// The format string is used to create the text representation of the number.
        /// </summary>
        public string FormatString
        {
            get { return formatString; }
            set
            {
                if (formatString != value)
                {
                    formatString = value;
                    MyNumberChanged();
                }
            }
        }
        string formatString;

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string Label
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
                label.Margin = string.IsNullOrEmpty(value) ? new Thickness(0) : new Thickness(5, 0, 5, 0);
            }
        }

        /// <summary>
        /// Get or set the MinWidth of the textBox.
        /// </summary>
        public double TBMinWidth
        {
            get { return textBox.MinWidth; }
            set { textBox.MinWidth = value; }
        }

        /// <summary>
        /// Need to invert the scrollBar values because if the scrollBar value is max we want our number to be min and v.v.
        /// </summary>
        double Invert(double number)
        {
            return Maximum + Minimum - number;
        }

        /// <summary>
        /// Occurs when the number has changed.
        /// </summary>
        public event EventHandler NumberChanged;
    }
}
