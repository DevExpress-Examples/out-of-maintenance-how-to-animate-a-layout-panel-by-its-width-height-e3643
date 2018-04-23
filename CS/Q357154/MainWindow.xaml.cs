using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using System.Collections.ObjectModel;

namespace Q357154 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            dockManager.Loaded += new RoutedEventHandler(dockManager_Loaded);
        }
        void dockManager_Loaded(object sender, RoutedEventArgs e) {
            foreach(BaseLayoutItem item in dockManager.GetItems()) {
                widths[item] = item.ItemWidth;
                heights[item] = item.ItemHeight;
            }
        }
        void DoAnimation() {
            if(dockManager == null) return;
            foreach(BaseLayoutItem item in dockManager.GetItems()) {
                if(item == dockManager.LayoutRoot) continue;
                Animate(item);
            }
        }
        BaseLayoutItem maximizedItem;
        Dictionary<BaseLayoutItem, GridLength> widths = new Dictionary<BaseLayoutItem, GridLength>();
        Dictionary<BaseLayoutItem, GridLength> heights = new Dictionary<BaseLayoutItem, GridLength>();
        void Animate(BaseLayoutItem layoutItem) {
            BeginAnimationToFixedValue(layoutItem, widths[layoutItem], BaseLayoutItem.ItemWidthProperty);
            BeginAnimationToFixedValue(layoutItem, heights[layoutItem], BaseLayoutItem.ItemHeightProperty);
        }

        void BeginAnimationToFixedValue(BaseLayoutItem layoutItem, GridLength l, DependencyProperty prop) {

            GridLengthAnimation a = CreateAnimation(layoutItem, prop, l);
            a.Completed += (s, e) =>
            {
                layoutItem.SetValue(prop, a.To);
            };
            layoutItem.BeginAnimation(prop, a);
        }
        GridLengthAnimation CreateAnimation(BaseLayoutItem item, DependencyProperty property, GridLength value) {
            GridLength to = new GridLength(0, GridUnitType.Star);

            if(maximizedItem != null) {
                if(maximizedItem == item || maximizedItem.Parent == item) to = value;
            }
            else {
                to = value;
            }
            GridLengthAnimation a = new GridLengthAnimation
            {
                From = (GridLength)item.GetValue(property),
                To = to,
                Duration = new TimeSpan(0, 0, 0, 0, 500),
                FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop
            };
            return a;
        }
        void OnPanelContentControlBackButtonClicked(object sender, EventArgs e) {
            maximizedItem = null;
            DoAnimation();
            ((PanelContentControl)sender).IsExpanded = false;
        }
        void OnPanelContentControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            maximizedItem = DockLayoutManager.GetLayoutItem(sender as DependencyObject);
            DoAnimation();
            ((PanelContentControl)sender).IsExpanded = true;
        }
    }


    public class PanelContentControl : ContentControl {
        #region static
        public static readonly DependencyProperty IndexNameProperty;
        public static readonly DependencyProperty CurrentChangeProperty;
        public static readonly DependencyProperty CurrentValueProperty;
        public static readonly DependencyProperty IsChangePositiveProperty;
        public static readonly DependencyProperty IsExpandedProperty;
        static PanelContentControl() {
            IndexNameProperty = DependencyProperty.Register("IndexName", typeof(string), typeof(PanelContentControl));
            CurrentChangeProperty = DependencyProperty.Register("CurrentChange", typeof(double), typeof(PanelContentControl),
                new PropertyMetadata(0.0, OnCurrentChangeChanged));
            CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(PanelContentControl),
                new PropertyMetadata(0.0));
            IsChangePositiveProperty = DependencyProperty.Register("IsChangePositive", typeof(bool?), typeof(PanelContentControl),
                new PropertyMetadata(null, OnIsChangePositiveChanged));
            IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PanelContentControl),
                new PropertyMetadata(false, OnIsExpandedChanged));
        }
        private static void OnCurrentChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            PanelContentControl panelContentControl = (PanelContentControl)d;
            panelContentControl.IsChangePositive = (double)(e.NewValue) == 0 ? null : (bool?)((double)(e.NewValue) > 0);
        }
        private static void OnIsChangePositiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((PanelContentControl)d).UpdateVisualState();
        }
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((PanelContentControl)d).UpdateVisualState();
        }
        #endregion
        public PanelContentControl() {
            DefaultStyleKey = typeof(PanelContentControl);
            Background = Brushes.Transparent;
            Unloaded += new RoutedEventHandler(PanelContentControl_Unloaded);
            SizeChanged += new SizeChangedEventHandler(PanelContentControl_SizeChanged);
            Cursor = Cursors.Hand;
        }
        void PanelContentControl_Unloaded(object sender, RoutedEventArgs e) {
            if(PartBackButton != null) {
                PartBackButton.Click -= PartBackButton_Click;
            }
        }
        void PanelContentControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            UpdateVisualState();
        }
        public event EventHandler BackButtonClicked;
        protected void RaiseBackButtonClicked() {
            if(BackButtonClicked != null)
                BackButtonClicked(this, EventArgs.Empty);
        }
        Button PartBackButton;
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            UpdateVisualState();
            PartBackButton = GetTemplateChild("PART_BackButton") as Button;
            if(PartBackButton != null) {
                PartBackButton.Click += new RoutedEventHandler(PartBackButton_Click);
            }
        }
        void PartBackButton_Click(object sender, RoutedEventArgs e) {
            RaiseBackButtonClicked();
        }
        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            UpdateVisualState();
        }
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseEnter(e);
            UpdateVisualState();
        }
        private void UpdateVisualState() {
            if(IsMouseOver)
                VisualStateManager.GoToState(this, "MouseOver", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
            if(IsChangePositive == null)
                VisualStateManager.GoToState(this, "Zero", false);
            if(IsChangePositive == true)
                VisualStateManager.GoToState(this, "Positive", false);
            if(IsChangePositive == false)
                VisualStateManager.GoToState(this, "Negative", false);
            if(IsExpanded) {
                Cursor = Cursors.Arrow;
                VisualStateManager.GoToState(this, "Checked", false);
            }
            else {
                Cursor = Cursors.Hand;
                VisualStateManager.GoToState(this, "Unchecked", false);
            }
            if(RenderSize.Width < 165)
                VisualStateManager.GoToState(this, "CompactView", false);
            else
                VisualStateManager.GoToState(this, "AdvancedView", false);
        }
        public string IndexName {
            get { return (string)GetValue(IndexNameProperty); }
            set { SetValue(IndexNameProperty, value); }
        }
        public double CurrentChange {
            get { return (double)GetValue(CurrentChangeProperty); }
            set { SetValue(CurrentChangeProperty, value); }
        }
        public double CurrentValue {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        public bool? IsChangePositive {
            get { return (bool?)GetValue(IsChangePositiveProperty); }
            set { SetValue(IsChangePositiveProperty, value); }
        }
        public bool IsExpanded {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
    }
}
