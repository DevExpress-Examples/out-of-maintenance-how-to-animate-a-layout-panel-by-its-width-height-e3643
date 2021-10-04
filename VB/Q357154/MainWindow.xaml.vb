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
    Public Partial Class MainWindow
        Inherits System.Windows.Window

        Public Sub New()
            InitializeComponent()
             ''' Cannot convert AssignmentExpressionSyntax, System.NullReferenceException: Object reference not set to an instance of an object.
'''    at ICSharpCode.CodeConverter.VB.NodesVisitor.VisitAssignmentExpression(AssignmentExpressionSyntax node)
'''    at Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    at ICSharpCode.CodeConverter.VB.CommentConvertingVisitorWrapper`1.Accept(SyntaxNode csNode, Boolean addSourceMapping)
''' 
''' Input:
'''             dockManager.Loaded += new System.Windows.RoutedEventHandler(this.dockManager_Loaded)
'''  End Sub

        Private Sub dockManager_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            For Each item As BaseLayoutItem In dockManager.GetItems()
                Me.widths(item) = item.ItemWidth
                Me.heights(item) = item.ItemHeight
            Next
        End Sub

        Private Sub DoAnimation()
            If dockManager Is Nothing Then Return
            For Each item As BaseLayoutItem In dockManager.GetItems()
                If item Is dockManager.LayoutRoot Then Continue For
                Me.Animate(item)
            Next
        End Sub

        Private maximizedItem As BaseLayoutItem

        Private widths As System.Collections.Generic.Dictionary(Of BaseLayoutItem, System.Windows.GridLength) = New System.Collections.Generic.Dictionary(Of BaseLayoutItem, System.Windows.GridLength)()

        Private heights As System.Collections.Generic.Dictionary(Of BaseLayoutItem, System.Windows.GridLength) = New System.Collections.Generic.Dictionary(Of BaseLayoutItem, System.Windows.GridLength)()

        Private Sub Animate(ByVal layoutItem As BaseLayoutItem)
            BeginAnimationToFixedValue(layoutItem, Me.widths(layoutItem), BaseLayoutItem.ItemWidthProperty)
            BeginAnimationToFixedValue(layoutItem, Me.heights(layoutItem), BaseLayoutItem.ItemHeightProperty)
        End Sub

        Private Sub BeginAnimationToFixedValue(ByVal layoutItem As BaseLayoutItem, ByVal l As System.Windows.GridLength, ByVal prop As System.Windows.DependencyProperty)
            Dim a As GridLengthAnimation = Me.CreateAnimation(layoutItem, prop, l)
            a.Completed += Function(s, e)
                layoutItem.SetValue(prop, a.[To])
            End Function
            layoutItem.BeginAnimation(prop, a)
        End Sub

        Private Function CreateAnimation(ByVal item As BaseLayoutItem, ByVal [property] As System.Windows.DependencyProperty, ByVal value As System.Windows.GridLength) As GridLengthAnimation
            Dim [to] As System.Windows.GridLength = New System.Windows.GridLength(0, System.Windows.GridUnitType.Star)
            If Me.maximizedItem IsNot Nothing Then
                If Me.maximizedItem Is item OrElse Me.maximizedItem.Parent Is item Then [to] = value
            Else
                [to] = value
            End If

            Dim a As GridLengthAnimation = New GridLengthAnimation With {.From = CType(item.GetValue([property]), System.Windows.GridLength), .[To] = [to], .Duration = New System.TimeSpan(0, 0, 0, 0, 500), .FillBehavior = System.Windows.Media.Animation.FillBehavior.[Stop]}
            Return a
        End Function

        Private Sub OnPanelContentControlBackButtonClicked(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.maximizedItem = Nothing
            Me.DoAnimation()
            CType(sender, Q357154.PanelContentControl).IsExpanded = False
        End Sub

        Private Sub OnPanelContentControlMouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
            Me.maximizedItem = DockLayoutManager.GetLayoutItem(TryCast(sender, System.Windows.DependencyObject))
            Me.DoAnimation()
            CType(sender, Q357154.PanelContentControl).IsExpanded = True
        End Sub
    End Class

    Public Class PanelContentControl
        Inherits System.Windows.Controls.ContentControl

'#Region "static"
        Public Shared ReadOnly IndexNameProperty As System.Windows.DependencyProperty

        Public Shared ReadOnly CurrentChangeProperty As System.Windows.DependencyProperty

        Public Shared ReadOnly CurrentValueProperty As System.Windows.DependencyProperty

        Public Shared ReadOnly IsChangePositiveProperty As System.Windows.DependencyProperty

        Public Shared ReadOnly IsExpandedProperty As System.Windows.DependencyProperty

        Shared Sub New()
            Q357154.PanelContentControl.IndexNameProperty = System.Windows.DependencyProperty.Register("IndexName", GetType(String), GetType(Q357154.PanelContentControl))
            Q357154.PanelContentControl.CurrentChangeProperty = System.Windows.DependencyProperty.Register("CurrentChange", GetType(Double), GetType(Q357154.PanelContentControl), New System.Windows.PropertyMetadata(0.0, AddressOf Q357154.PanelContentControl.OnCurrentChangeChanged))
            Q357154.PanelContentControl.CurrentValueProperty = System.Windows.DependencyProperty.Register("CurrentValue", GetType(Double), GetType(Q357154.PanelContentControl), New System.Windows.PropertyMetadata(0.0))
            Q357154.PanelContentControl.IsChangePositiveProperty = System.Windows.DependencyProperty.Register("IsChangePositive", GetType(Boolean?), GetType(Q357154.PanelContentControl), New System.Windows.PropertyMetadata(Nothing, AddressOf Q357154.PanelContentControl.OnIsChangePositiveChanged))
            Q357154.PanelContentControl.IsExpandedProperty = System.Windows.DependencyProperty.Register("IsExpanded", GetType(Boolean), GetType(Q357154.PanelContentControl), New System.Windows.PropertyMetadata(False, AddressOf Q357154.PanelContentControl.OnIsExpandedChanged))
        End Sub

        Private Shared Sub OnCurrentChangeChanged(ByVal d As System.Windows.DependencyObject, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)
            Dim panelContentControl As Q357154.PanelContentControl = CType(d, Q357154.PanelContentControl)
            panelContentControl.IsChangePositive = If(CDbl((e.NewValue)) = 0, Nothing, CType((CDbl((e.NewValue)) > 0), Boolean?))
        End Sub

        Private Shared Sub OnIsChangePositiveChanged(ByVal d As System.Windows.DependencyObject, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)
            CType(d, Q357154.PanelContentControl).UpdateVisualState()
        End Sub

        Private Shared Sub OnIsExpandedChanged(ByVal d As System.Windows.DependencyObject, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)
            CType(d, Q357154.PanelContentControl).UpdateVisualState()
        End Sub

'#End Region
        Public Sub New()
            MyBase.DefaultStyleKey = GetType(Q357154.PanelContentControl)
            MyBase.Background = System.Windows.Media.Brushes.Transparent
            AddHandler Unloaded, New System.Windows.RoutedEventHandler(AddressOf Me.PanelContentControl_Unloaded)
            AddHandler SizeChanged, New System.Windows.SizeChangedEventHandler(AddressOf Me.PanelContentControl_SizeChanged)
            MyBase.Cursor = System.Windows.Input.Cursors.Hand
        End Sub

        Private Sub PanelContentControl_Unloaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            If Me.PartBackButton IsNot Nothing Then
                RemoveHandler Me.PartBackButton.Click, AddressOf Me.PartBackButton_Click
            End If
        End Sub

        Private Sub PanelContentControl_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs)
            Me.UpdateVisualState()
        End Sub

        Public Event BackButtonClicked As System.EventHandler

        Protected Sub RaiseBackButtonClicked()
            RaiseEvent BackButtonClicked(Me, System.EventArgs.Empty)
        End Sub

        Private PartBackButton As System.Windows.Controls.Button

        Public Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            Me.UpdateVisualState()
            Me.PartBackButton = TryCast(MyBase.GetTemplateChild("PART_BackButton"), System.Windows.Controls.Button)
            If Me.PartBackButton IsNot Nothing Then
                AddHandler Me.PartBackButton.Click, New System.Windows.RoutedEventHandler(AddressOf Me.PartBackButton_Click)
            End If
        End Sub

        Private Sub PartBackButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            Me.RaiseBackButtonClicked()
        End Sub

        Protected Overrides Sub OnMouseEnter(ByVal e As System.Windows.Input.MouseEventArgs)
            MyBase.OnMouseEnter(e)
            Me.UpdateVisualState()
        End Sub

        Protected Overrides Sub OnMouseLeave(ByVal e As System.Windows.Input.MouseEventArgs)
            MyBase.OnMouseEnter(e)
            Me.UpdateVisualState()
        End Sub

        Private Sub UpdateVisualState()
            If MyBase.IsMouseOver Then
                Call System.Windows.VisualStateManager.GoToState(Me, "MouseOver", True)
            Else
                Call System.Windows.VisualStateManager.GoToState(Me, "Normal", True)
            End If

            If Me.IsChangePositive Is Nothing Then Call System.Windows.VisualStateManager.GoToState(Me, "Zero", False)
            If Me.IsChangePositive = True Then Call System.Windows.VisualStateManager.GoToState(Me, "Positive", False)
            If Me.IsChangePositive = False Then Call System.Windows.VisualStateManager.GoToState(Me, "Negative", False)
            If Me.IsExpanded Then
                MyBase.Cursor = System.Windows.Input.Cursors.Arrow
                Call System.Windows.VisualStateManager.GoToState(Me, "Checked", False)
            Else
                MyBase.Cursor = System.Windows.Input.Cursors.Hand
                Call System.Windows.VisualStateManager.GoToState(Me, "Unchecked", False)
            End If

            If MyBase.RenderSize.Width < 165 Then
                Call System.Windows.VisualStateManager.GoToState(Me, "CompactView", False)
            Else
                Call System.Windows.VisualStateManager.GoToState(Me, "AdvancedView", False)
            End If
        End Sub

        Public Property IndexName As String
            Get
                Return CStr(MyBase.GetValue(Q357154.PanelContentControl.IndexNameProperty))
            End Get

            Set(ByVal value As String)
                MyBase.SetValue(Q357154.PanelContentControl.IndexNameProperty, value)
            End Set
        End Property

        Public Property CurrentChange As Double
            Get
                Return CDbl(MyBase.GetValue(Q357154.PanelContentControl.CurrentChangeProperty))
            End Get

            Set(ByVal value As Double)
                MyBase.SetValue(Q357154.PanelContentControl.CurrentChangeProperty, value)
            End Set
        End Property

        Public Property CurrentValue As Double
            Get
                Return CDbl(MyBase.GetValue(Q357154.PanelContentControl.CurrentValueProperty))
            End Get

            Set(ByVal value As Double)
                MyBase.SetValue(Q357154.PanelContentControl.CurrentValueProperty, value)
            End Set
        End Property

        Public Property IsChangePositive As Boolean?
            Get
                Return CType(MyBase.GetValue(Q357154.PanelContentControl.IsChangePositiveProperty), Boolean?)
            End Get

            Set(ByVal value As Boolean?)
                MyBase.SetValue(Q357154.PanelContentControl.IsChangePositiveProperty, value)
            End Set
        End Property

        Public Property IsExpanded As Boolean
            Get
                Return CBool(MyBase.GetValue(Q357154.PanelContentControl.IsExpandedProperty))
            End Get

            Set(ByVal value As Boolean)
                MyBase.SetValue(Q357154.PanelContentControl.IsExpandedProperty, value)
            End Set
        End Property
    End Class
End Namespace
