Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports DevExpress.Xpf.Docking
Imports System.Collections.ObjectModel

Namespace Q357154
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Public Sub New()
			InitializeComponent()
			AddHandler dockManager.Loaded, AddressOf dockManager_Loaded
		End Sub
		Private Sub dockManager_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			For Each item As BaseLayoutItem In dockManager.GetItems()
				widths(item) = item.ItemWidth
				heights(item) = item.ItemHeight
			Next item
		End Sub
		Private Sub DoAnimation()
			If dockManager Is Nothing Then
				Return
			End If
			For Each item As BaseLayoutItem In dockManager.GetItems()
				If item Is dockManager.LayoutRoot Then
					Continue For
				End If
				Animate(item)
			Next item
		End Sub
		Private maximizedItem As BaseLayoutItem
		Private widths As New Dictionary(Of BaseLayoutItem, GridLength)()
		Private heights As New Dictionary(Of BaseLayoutItem, GridLength)()
		Private Sub Animate(ByVal layoutItem As BaseLayoutItem)
			BeginAnimationToFixedValue(layoutItem, widths(layoutItem), BaseLayoutItem.ItemWidthProperty)
			BeginAnimationToFixedValue(layoutItem, heights(layoutItem), BaseLayoutItem.ItemHeightProperty)
		End Sub

		Private Sub BeginAnimationToFixedValue(ByVal layoutItem As BaseLayoutItem, ByVal l As GridLength, ByVal prop As DependencyProperty)

			Dim a As GridLengthAnimation = CreateAnimation(layoutItem, prop, l)
			AddHandler a.Completed, Function(s, e) AnonymousMethod1(s, e, layoutItem, prop, a)
			layoutItem.BeginAnimation(prop, a)
		End Sub
		
		Private Function AnonymousMethod1(ByVal s As Object, ByVal e As Object, ByVal layoutItem As BaseLayoutItem, ByVal prop As DependencyProperty, ByVal a As GridLengthAnimation) As Boolean
			layoutItem.SetValue(prop, a.To)
			Return True
		End Function
		Private Function CreateAnimation(ByVal item As BaseLayoutItem, ByVal [property] As DependencyProperty, ByVal value As GridLength) As GridLengthAnimation
			Dim [to] As New GridLength(0, GridUnitType.Star)

			If maximizedItem IsNot Nothing Then
				If maximizedItem Is item OrElse maximizedItem.Parent Is item Then
					[to] = value
				End If
			Else
				[to] = value
			End If
			Dim a As GridLengthAnimation = New GridLengthAnimation With {.From = CType(item.GetValue([property]), GridLength), .To = [to], .Duration = New TimeSpan(0, 0, 0, 0, 500), .FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop}
			Return a
		End Function
		Private Sub OnPanelContentControlBackButtonClicked(ByVal sender As Object, ByVal e As EventArgs)
			maximizedItem = Nothing
			DoAnimation()
			CType(sender, PanelContentControl).IsExpanded = False
		End Sub
		Private Sub OnPanelContentControlMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			maximizedItem = DockLayoutManager.GetLayoutItem(TryCast(sender, DependencyObject))
			DoAnimation()
			CType(sender, PanelContentControl).IsExpanded = True
		End Sub
	End Class


	Public Class PanelContentControl
		Inherits ContentControl
		#Region "static"
		Public Shared ReadOnly IndexNameProperty As DependencyProperty
		Public Shared ReadOnly CurrentChangeProperty As DependencyProperty
		Public Shared ReadOnly CurrentValueProperty As DependencyProperty
		Public Shared ReadOnly IsChangePositiveProperty As DependencyProperty
		Public Shared ReadOnly IsExpandedProperty As DependencyProperty
		Shared Sub New()
			IndexNameProperty = DependencyProperty.Register("IndexName", GetType(String), GetType(PanelContentControl))
			CurrentChangeProperty = DependencyProperty.Register("CurrentChange", GetType(Double), GetType(PanelContentControl), New PropertyMetadata(0.0, AddressOf OnCurrentChangeChanged))
			CurrentValueProperty = DependencyProperty.Register("CurrentValue", GetType(Double), GetType(PanelContentControl), New PropertyMetadata(0.0))
			IsChangePositiveProperty = DependencyProperty.Register("IsChangePositive", GetType(Boolean?), GetType(PanelContentControl), New PropertyMetadata(Nothing, AddressOf OnIsChangePositiveChanged))
			IsExpandedProperty = DependencyProperty.Register("IsExpanded", GetType(Boolean), GetType(PanelContentControl), New PropertyMetadata(False, AddressOf OnIsExpandedChanged))
		End Sub
		Private Shared Sub OnCurrentChangeChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			Dim panelContentControl As PanelContentControl = CType(d, PanelContentControl)
			panelContentControl.IsChangePositive = If(CDbl(e.NewValue) = 0, Nothing, CType(CDbl(e.NewValue) > 0, Boolean?))
		End Sub
		Private Shared Sub OnIsChangePositiveChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			CType(d, PanelContentControl).UpdateVisualState()
		End Sub
		Private Shared Sub OnIsExpandedChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			CType(d, PanelContentControl).UpdateVisualState()
		End Sub
		#End Region
		Public Sub New()
			DefaultStyleKey = GetType(PanelContentControl)
			Background = Brushes.Transparent
			AddHandler Unloaded, AddressOf PanelContentControl_Unloaded
			AddHandler SizeChanged, AddressOf PanelContentControl_SizeChanged
			Cursor = Cursors.Hand
		End Sub
		Private Sub PanelContentControl_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If PartBackButton IsNot Nothing Then
				RemoveHandler PartBackButton.Click, AddressOf PartBackButton_Click
			End If
		End Sub
		Private Sub PanelContentControl_SizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
			UpdateVisualState()
		End Sub
		Public Event BackButtonClicked As EventHandler
		Protected Sub RaiseBackButtonClicked()
			RaiseEvent BackButtonClicked(Me, EventArgs.Empty)
		End Sub
		Private PartBackButton As Button
		Public Overrides Sub OnApplyTemplate()
			MyBase.OnApplyTemplate()
			UpdateVisualState()
			PartBackButton = TryCast(GetTemplateChild("PART_BackButton"), Button)
			If PartBackButton IsNot Nothing Then
				AddHandler PartBackButton.Click, AddressOf PartBackButton_Click
			End If
		End Sub
		Private Sub PartBackButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			RaiseBackButtonClicked()
		End Sub
		Protected Overrides Sub OnMouseEnter(ByVal e As MouseEventArgs)
			MyBase.OnMouseEnter(e)
			UpdateVisualState()
		End Sub
		Protected Overrides Sub OnMouseLeave(ByVal e As MouseEventArgs)
			MyBase.OnMouseEnter(e)
			UpdateVisualState()
		End Sub
		Private Sub UpdateVisualState()
			If IsMouseOver Then
				VisualStateManager.GoToState(Me, "MouseOver", True)
			Else
				VisualStateManager.GoToState(Me, "Normal", True)
			End If
			If IsChangePositive Is Nothing Then
				VisualStateManager.GoToState(Me, "Zero", False)
			End If
			If IsChangePositive = True Then
				VisualStateManager.GoToState(Me, "Positive", False)
			End If
			If IsChangePositive = False Then
				VisualStateManager.GoToState(Me, "Negative", False)
			End If
			If IsExpanded Then
				Cursor = Cursors.Arrow
				VisualStateManager.GoToState(Me, "Checked", False)
			Else
				Cursor = Cursors.Hand
				VisualStateManager.GoToState(Me, "Unchecked", False)
			End If
			If RenderSize.Width < 165 Then
				VisualStateManager.GoToState(Me, "CompactView", False)
			Else
				VisualStateManager.GoToState(Me, "AdvancedView", False)
			End If
		End Sub
		Public Property IndexName() As String
			Get
				Return CStr(GetValue(IndexNameProperty))
			End Get
			Set(ByVal value As String)
				SetValue(IndexNameProperty, value)
			End Set
		End Property
		Public Property CurrentChange() As Double
			Get
				Return CDbl(GetValue(CurrentChangeProperty))
			End Get
			Set(ByVal value As Double)
				SetValue(CurrentChangeProperty, value)
			End Set
		End Property
		Public Property CurrentValue() As Double
			Get
				Return CDbl(GetValue(CurrentValueProperty))
			End Get
			Set(ByVal value As Double)
				SetValue(CurrentValueProperty, value)
			End Set
		End Property
		Public Property IsChangePositive() As Boolean?
			Get
				Return CType(GetValue(IsChangePositiveProperty), Boolean?)
			End Get
			Set(ByVal value? As Boolean)
				SetValue(IsChangePositiveProperty, value)
			End Set
		End Property
		Public Property IsExpanded() As Boolean
			Get
				Return CBool(GetValue(IsExpandedProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(IsExpandedProperty, value)
			End Set
		End Property
	End Class
End Namespace
