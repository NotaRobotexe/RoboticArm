   M S B A M L   `   `   `  ����b  ^PresentationFramework.Aero2, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35hKclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero2Microsoft.Windows.Themes  5   	    Robotic_Arm_Desktop%   Robotic_Arm_Desktop.StartScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   J FWpfAnimatedGif, Version=1.4.15.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  5   	   XThemesKclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero2  5   	     � Title$  StartScreen��5   	   $
��1080��6   $
��1920��6+    � WindowStyle$
 None=�68    � WindowStartupLocation$ CenterScreen=�6K    � WindowState$ 	Maximized=�6p    � 
ResizeMode$ NoResize=�6�    � 	Resources 5         ComboBox.Static.Background �� EndPoint �� 
StartPoint � Color	 � Offset  ComboBox.Static.Border
 �� Color ' #ComboBox.Static.Editable.Background # ComboBox.Static.Editable.Border . *ComboBox.Static.Editable.Button.Background * &ComboBox.Static.Editable.Button.Border  ComboBox.MouseOver.Glyph ! ComboBox.MouseOver.Background  ComboBox.MouseOver.Border *	 &ComboBox.MouseOver.Editable.Background &
 "ComboBox.MouseOver.Editable.Border 1 -ComboBox.MouseOver.Editable.Button.Background - )ComboBox.MouseOver.Editable.Button.Border  ComboBox.Pressed.Glyph  ComboBox.Pressed.Background  ComboBox.Pressed.Border ( $ComboBox.Pressed.Editable.Background $  ComboBox.Pressed.Editable.Border / +ComboBox.Pressed.Editable.Button.Background + 'ComboBox.Pressed.Editable.Button.Border  ComboBox.Disabled.Glyph   ComboBox.Disabled.Background  ComboBox.Disabled.Border ) %ComboBox.Disabled.Editable.Background % !ComboBox.Disabled.Editable.Border 0 ,ComboBox.Disabled.Editable.Button.Background , (ComboBox.Disabled.Editable.Button.Border  ComboBox.Static.Glyph  ComboBoxToggleButton �� 
TargetType �� Property �� Value �� 
TargetType ��Name J� SnapsToDevicePixels F�Name �� Triggers ^� 
Conditions �� Binding �� RelativeSource �� AncestorType �� Value �� 
TargetName P� Property P� Value  ComboBoxTemplate �Name � ColumnDefinitions �Name � AllowsTransparency �� Mode5   /Microsoft.Windows.Themes.SystemDropShadowChrome   Name!   Color" �� MaxDropDownHeight# �� ElementName$ ��Name% �� ClearTypeHint& ��Name' ��Name( ��Name-  'System.Windows.Input.KeyboardNavigation)   DirectionalNavigation* d�Name+ d� 	IsChecked, ��Name- �� SelectionBoxItemTemplate. �� SelectionBoxItem#/ �� SelectionBoxItemStringFormat0 �� ContentStringFormat1 J� IsHitTestVisible2 P� 
SourceName3 ]� 
Conditions4 �� Property  TextBox.Static.Background  ComboBoxEditableTextBox   ComboBoxEditableTemplate5 ��Name6 �� 
IsReadOnly ! ComboBoxStyle1 " FocusVisual7 �� Triggers��%	-  &	        &	 �     &	 �     &	 �     &	     &	 *    &	 O    &	 r    &	     &		 1    &	
 T    &	 w    &	     &	 6    &	 Y    &	 �    &	     &	 ;    &	 ^    &	 �    &	     &	 @    &	 c    &	 �    &	 �    &	 �    &	 �    &	     &	 5    7  7   7  7  7  7  7  7  7  7  7 	 7 
 7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  &	 S    7  &	 �    &	 �    &	      7  7  7  &	! �(    7 " 7   7  7  7   �� 5   
   $	 0,1&�6A   $	 0,0&�6P   .��5      � $ 	#FF2E2E2E��6   $		 0.0Q�6-   � 5      $ 	#FF2E2E2E��6   $		 1.0Q�6-   5      �� 5   
   $
 	#FFACACAC��69   �� 5   
   $
 	#FFFFFFFF��6F   �� 5   
   $
 	#FFABADB3��6B   �� 5   
   $
 Transparent��6M   �� 5   
   $
 Transparent��6I   �� 5   
   $
 	#FF000000��6;   �� 5   
   $	 0,1&�6D   $	 0,0&�6S   .��5      � $ 	#FF2E2E2E��6   $		 0.0Q�6-   � 5      $ 	#FF2E2E2E��6   $		 1.0Q�6-   5      �� 5   
   $
 	#FF7EB4EA��6<   �� 5   
   $
 	#FFFFFFFF��6I   �� 5   
   $
 	#FF7EB4EA��6E   �� 5   
   $	 0,1&�6T   $	 0,0&�6c   .��5       � $ 	#FFEBF4FC��6   $		 0.0Q�6-   � 5!      $ 	#FFDCECFC��6   $		 1.0Q�6-   5"      �� 5#   
   $
 	#FF7EB4EA��6L   �� 5$   
   $
 	#FF000000��69   �� 5%   
   $	 0,1&�6B   $	 0,0&�6Q   .��5&      � $ 	#FF2E2E2E��6   $		 0.0Q�6-   � 5'      $ 	#FF2E2E2E��6   $		 1.0Q�6-   5(      �� 5)   
   $
 	#FF569DE5��6:   �� 5*   
   $
 	#FFFFFFFF��6G   �� 5+   
   $
 	#FF569DE5��6C   �� 5,   
   $	 0,1&�6R   $	 0,0&�6a   .��5-      � $ 	#FFDAEBFC��6   $		 0.0Q�6-   � 5.      $ 	#FFC4E0FC��6   $		 1.0Q�6-   5/      �� 50   
   $
 	#FF569DE5��6J   �� 51   
   $
 	#FFBFBFBF��6:   �� 52   
   $
 	#FFF0F0F0��6?   �� 53   
   $
 	#FFD9D9D9��6;   �� 54   
   $
 	#FFFFFFFF��6H   �� 55   
   $
 	#FFBFBFBF��6D   �� 56   
   $
 Transparent��6O   �� 57   
   $
 Transparent��6K   �� 58   
   $
 White��68   �� 59   
   " d�6-   .�5:      ��  �@�OverridesDefaultStyle . 66   �� 5;       � �� .  6*   �� 5<       � |� .  6*   �� 5=       �@��	ClickMode$ Press=�6*   �� 5>       � �� 5?      �� 5@      " d�6&   .R�5A      �� $ templateRoot��6!   8��  #��z��6m   8��  . ._�5B      �� $ splitBorder��6%   
������ $��1q�6T   $��Right=�6h   $��0q�6�    . #��� r�6�   ._�5C   "   F� $ arrow��6'   F����        ���( ��( јaQ     јaQ P����̰( � �    P����        8�� $��Center=�6�   $��0q�6�   $��Center=�6�   5D      5E       5F      ^� 5G       5H   "   �� 5I   &    60   �� *
IsEditable+ �� " �� true6�   �� 5J   &    60   �� *IsMouseOver+ �� *Self+	 false6v   �� 5K   &    60   �� *	IsPressed+ �� *Self+	 false6t   �� 5L   &    60   �� *	IsEnabled+ �� *Self+ true6t   .0�5N   "   �� $ templateRoot��6?   8   � ��6Y   �� 5O   "   $ templateRoot��6@   8   � ��6Z   �� 5P   "   $ splitBorder��6?   8   � ��6X   �� 5Q   "   $ splitBorder��6@   8   � ��6Y   5R      P� 5S       �@J�IsMouseOver . . �5T   "   �� $ arrow��69   8   � ��6L   5U      ^� 5V       5W   "   �� 5X   &    60   �� *IsMouseOver+ �� *Self+ true6v   �� 5Y   &    60   �� *
IsEditable+ �� " ��	 false6�   .0�5[   "   �� $ templateRoot��6?   8   � ��6Y   �� 5\   "   $ templateRoot��6@   8 	  � ��6Z   5]      ^� 5^       5_   "   �� 5`   &    60   �� *IsMouseOver+ �� *Self+ true6v   �� 5a   &    60   �� *
IsEditable+ �� " �� true6�   .0�5c   "   �� $ templateRoot��6?   8 
  � ��6Y   �� 5d   "   $ templateRoot��6@   8   � ��6Z   �� 5e   "   $ splitBorder��6?   8   � ��6X   �� 5f   "   $ splitBorder��6@   8   � ��6Y   5g      P� 5h       � �� . . �5i   "   �� $ arrow��69   8   � ��6L   5j      ^� 5k       5l   "   �� 5m   &    60   �� *	IsPressed+ �� *Self+ true6t   �� 5n   &    60   �� *
IsEditable+ �� " ��	 false6�   .0�5p   "   �� $ templateRoot��6?   8   � ��6Y   �� 5q   "   $ templateRoot��6@   8   � ��6Z   5r      ^� 5s       5t   "   �� 5u   &    60   �� *	IsPressed+ �� *Self+ true6t   �� 5v   &    60   �� *
IsEditable+ �� " �� true6�   .0�5x   "   �� $ templateRoot��6?   8   � ��6Y   �� 5y   "   $ templateRoot��6@   8   � ��6Z   �� 5z   "   $ splitBorder��6?   8   � ��6X   �� 5{   "   $ splitBorder��6@   8   � ��6Y   5|      P� 5}       � {� .  . �5~   "   �� $ arrow��69   8   � ��6L   5      ^� 5�       5�   "   �� 5�   &    60   �� *	IsEnabled+ �� *Self+	 false6t   �� 5�   &    60   �� *
IsEditable+ �� " ��	 false6�   .0�5�   "   �� $ templateRoot��6?   8   � ��6Y   �� 5�   "   $ templateRoot��6@   8   � ��6Z   5�      ^� 5�       5�   "   �� 5�   &    60   �� *	IsEnabled+ �� *Self+	 false6t   �� 5�   &    60   �� *
IsEditable+ �� " �� true6�   .0�5�   "   �� $ templateRoot��6?   8   � ��6Y   �� 5�   "   $ templateRoot��6@   8   � ��6Z   �� 5�   "   $ splitBorder��6?   8   � ��6X   �� 5�   "   $ splitBorder��6@   8   � ��6Y   5�      5�      5�      5�      �� 5�   
   " ��63   .R�5�      � $ templateRoot��6    .  5�      �� 5�      $��* �6'   �� 5�      #��� r�6'   $��0 �6z   .+�5�      � $ 
PART_Popup��6    . $��2��6F   $��1q�6�   #��� .�6�   $��Bottom=�6  ��6Z   �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+.��5�        $  shadow��64   $! Transparent��6D   #��z" 6X   ��6�   �� *ActualWidth+$# templateRoot��._�5�      �� $ dropDownBorder��6!   #��� ��69   $��1q�6�   #��� ��6�   ._�5�      �� $$ DropDownScrollViewer��6+   .��5�   "   � $
 grid��6'   $% Enabled=�65   .+�5�   &   �� $& canvas��6-   $
��Left=�6=   $��0��6X   $	��Top=�6c   $��0��6{   .+�5�   *   �� $' 
opaqueRect��64   ��6H   �� *
Background+$# dropDownBorder����6�   �� *ActualHeight+$# dropDownBorder����6�   �� *ActualWidth+$# dropDownBorder��5�   '   �� 5�   &   $( ItemsPresenter��65   $) 	Contained=�6M   # z 6�   5�   #   5�      5�      5�      5�      d� 5�      $* toggleButton��6   #��z��65   #��z��6a   #��z��6�   $��2��6�   8��  + 6�   �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+66  �� 5�      $, contentPresenter��6#   #��z- 6=   #��z��6z   #��z. 6�   #0 z/ 6�   #��z��6-  1 .  #��z��6�  # z 6�  #��z��6�  5�       5�      P� 5�      $2 
PART_Popup��63    �@�HasDropShadow . . �5�      �� $ shadow��6/    � ��$ 0,0,5,5q�6C   �� 5�      $ shadow��6.    �@ Color$ 	#71000000��6B   5�      P� 5�       �@��HasItems .  . �5�      �� $ dropDownBorder��6/    � ��$ 95��6K   5�      ]� 5�      3 5�      �� 5�      4 �@��
IsGrouping . 6:   �� 5�      "4 �@%�IsVirtualizingWhenGrouping .  6\   ./�5�      ��  � �� .  6F   5�      P� 5�      $2 DropDownScrollViewer��6C    � �� .  . �5�      �� $ 
opaqueRect��63    6K   �� *VerticalOffset+$# DropDownScrollViewer�� �@��Top�� 5�      $ 
opaqueRect��64    6L   �� *HorizontalOffset+$# DropDownScrollViewer�� �@��Left5�      5�      �� 5�   
   $
 	#FFFFFFFF��6<   �� 5�   
   " ��60   .�5�      ��  �@�OverridesDefaultStyle . 66   �� 5�       �@J�	AllowDrop . 6*   �� 5�       � ��$ 0��6)   �� 5�       � ��$ 0��6*   �� 5�       �@�FocusVisualStyle 61   Z� �� 5�       �@��PanningMode$ VerticalFirst=�69   �� 5�       �@��IsFlicksEnabled .  67   �� 5�       � �� 5�      �� 5�      " ��6&   .R�5�      �� $$ PART_ContentHost��6'   
������ |�.  $��Hidden=�6l   $��Hidden=�6�   5�      5�      5�      �� 5�   
   " ��6;   .R�5�      � $ templateRoot��6    .  5�      �� 5�      $��* �6'   �� 5�      #��� r�6'   $��0 �6z   .+�5�      � $ 
PART_Popup��6    . $��2��6F   #��� .�6�   $��Bottom=�6  ��6Z   �� *IsDropDownOpen+ �� *TemplatedParent+.��5�        $  shadow��64   $! Transparent��6D   #��z" 6X   ��6�   �� *ActualWidth+$# templateRoot��._�5�      �� $ dropDownBorder��6!   #��� ��69   $��1q�6�   #��� ��6�   ._�5�      �� $$ DropDownScrollViewer��6+   .��5�   "   � $
 grid��6'   $% Enabled=�65   .+�5�   &   �� $& canvas��6-   $
��Left=�6=   $��0��6X   $	��Top=�6c   $��0��6{   .+�5�   *   �� $' 
opaqueRect��64   ��6H   �� *
Background+$# dropDownBorder����6�   �� *ActualHeight+$# dropDownBorder����6�   �� *ActualWidth+$# dropDownBorder��5�   '   �� 5�   &   $( ItemsPresenter��65   $) 	Contained=�6M   # z 6�   5�   #   5�      5�      5�      5�      d� 5�      $* toggleButton��6   #��z��65   #��z��6a   #��z��6�   $��2��6�   8��  + 6�   �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+66  �� 5�      $ border��6   8�� #��z��6a   ._�5�      �� $5 PART_EditableTextBox��6   #��z��6<   #��z��6�   8�� #��z��6-  6 6�   �� *
IsReadOnly+ �� *TemplatedParent+6-  5�      5�       5�      P� 5�       � {� .  . �5�      �� $ border��60    �@J�Opacity$
 0.56Q�6D   5�      P� 5�       �@J�IsKeyboardFocusWithin . . �5�      ��  � ��
 �   �63   5�      P� 5�      $2 
PART_Popup��63    �@�HasDropShadow . . �5�      �� $ shadow��6/    � ��$ 0,0,5,5q�6C   �� 5�      $ shadow��6.    �@ Color$ 	#71000000��6B   5�      P� 5�       �@��HasItems .  . �5�      �� $ dropDownBorder��6/    � ��$ 95��6K   5�      ]� 5�      3 5�      �� 5�      4 �@��
IsGrouping . 6:   �� 5�      "4 �@%�IsVirtualizingWhenGrouping .  6\   ./�5      ��  � �� .  6F   5     P� 5     $2 DropDownScrollViewer��6C    � �� .  . �5     �� $ 
opaqueRect��63    6K   �� *VerticalOffset+$# DropDownScrollViewer�� �@��Top�� 5     $ 
opaqueRect��64    6L   �� *HorizontalOffset+$# DropDownScrollViewer�� �@��Left5     5     �� 5  
   " ��6'   .�5	     ��  �@�FocusVisualStyle8   61   �� 5
      � ��8  6+   �� 5      � ��8  6,   �� 5      � ��# � ��6+   �� 5      � ��$ 1q�60   �� 5      � ��$
 Auto=�6K   �� 5      � ��$
 Auto=�6I   �� 5      � ��$ 6,3,5,3q�6(   �� 5      � �� . 6>   �� 5      �@��PanningMode$
 Both=�69   �� 5      �@��IsFlicksEnabled .  67   �� 5      � ��8  6)   7 5     P� 5      �@��
IsEditable . . �5     ��  � �� .  62   �� 5      � ��$ 2q�60   �� 5      � ��8  61   5     5     5     .��5     � ���	#FF2D2D30.+�5   
   �� -   5        8 ��Name$
8 exit��5      $�� ��6   $
��Left=�6*   $��1893,10,0,0q�6E   $	��Top=�6Z   $��17��6r   $��17��6}   |�.  ��6�   Z� ��5!     �� 5"     9 �� ImageSource$9 icons/clear-button.png��6   5$     �� 5%  
   -   5        $
��Left=�5%     $��1864,14,0,0q�6,   $	��Top=�6A   $��18��6Y   $��18��6d   ��6p   Z� ��5&     �� 5'     $9 icons/minus-symbol.png��6   5)     � 5*  
   $
��Left=�6   $	��456��6*   $��792,276,0,0q�67   $	��Top=�6L   $	��375��6d   ���	#FF161617.+�5+     �� -   5        $��ipadd��5+     $
��Left=�6#   $��29��6>   $��
190,26,0,0q�6J   : �� TextWrapping$
: Wrap=�6^   $�� ��6r   $	��Top=�6z   $	��157��6�   ���	#FF2E2E2E���	#FFD7D8D8���	#FFEAEAEA$��18(�6�   �� 5,     -   5         # ButtonStyle1#��� # 5,     $
��Exit��6<   $
��Left=�6K   $��
44,394,0,0q�6f   $	��Top=�6z   $��80��6�   ���	#FF2E2E2E
�������$��2q�6�   $��30��6�   $��17(�6�   $��Segoe UI Semibold)�6�   ; � ForceCursor; . 
�������6?  �� 5-     -   5        $8 	tothemain��5-  K   #��� # 6   $
��Next��6<   $
��Left=�6^   $��255,394,0,0q�6y   $	��Top=�6�   $��80��6�   ���	#FF2E2E2E
�������$��2q�6�   $��30��6�   $��17(�6   $��Segoe UI Semibold)�6  ; . 
�������6S  �� 5.     $��Find Device :��6   $
��Left=�6,   $��	44,60,0,0q�6G   $	��Top=�6Z   $��19(�6r   $��Segoe UI Semibold)�6�   
�������< J� RenderTransformOrigin$< 0.219,0.543&�6�   �� 5/     -   5        = ��Name$= loading��5/     $
��Left=�6%   "  WpfAnimatedGif.ImageBehavior>   RepeatBehavior$> 0x��6@   ?   AnimatedSource$? icons/preloader.gif��6f   $	��100��6�   $��142,189,0,0q�6�   $	��Top=�6�   $	��100��6�   $y�Hidden=�6�   �� 50     -   5        $��findb��50     #��� # 6"   
�������$��80��6\   ���	#FF2E2E2E
�������$��2q�6�   $��30��6�   $��17(�6�   $��Segoe UI Semibold)�6�   $
��Find��6�   $
��Left=�6�   $��
267,63,0,0q�6  $	��Top=�6!  69  �� 51     $��IP addres : ��6   $
��Left=�6+   $��	44,20,0,0q�6F   $	��Top=�6Y   $��19(�6q   $��Segoe UI Semibold)�6   
�������6�   52     53     55     