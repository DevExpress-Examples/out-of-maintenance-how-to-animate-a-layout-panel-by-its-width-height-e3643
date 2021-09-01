<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3643)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/Q357154/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/Q357154/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/Q357154/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/Q357154/MainWindow.xaml.vb))
* [PanelAnimationTemplates.xaml](./CS/Q357154/PanelAnimationTemplates.xaml) (VB: [PanelAnimationTemplates.xaml](./VB/Q357154/PanelAnimationTemplates.xaml))
<!-- default file list end -->
# How to animate a layout panel by its width/height


<p>This sample illustrates how to animate layout panels. It is necessary to choose the property for an animation and animation type. Right now our DockLayoutManager only includes the GridLengthAnimation animation type. The last step is to call the layout item BeginAnimation method. When an animation starts, PanelContentControl objects change their appearance (width/height) based on the current state of the PanelContentControl.IsExpanded property. The sample shows how to accomplish this task.</p>

<br/>


