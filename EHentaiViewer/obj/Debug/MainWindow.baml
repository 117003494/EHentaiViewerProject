   M S B A M L   `   `   `  ����H  DEHentaiViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:EHentaiViewerEHentaiViewer  5   	   =#clr-namespace:EHentaiViewer.includeEHentaiViewer.include  5   	       EHentaiViewer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:EHentaiViewer     5   	   2me#clr-namespace:EHentaiViewer.include     5   	   -   5          � Title$  
MainWindow��5	   	   $	��350��6   $	��525��6)    � 	Resources 5
      ,   &EHentaiViewer.include.ImageDataConvert   
imgConvert �� 
TargetType  stepListBoxItemStyle �� 	Resources �� Color �� Opacity��%3  &	        &	        5   
   63   �� 5   
   " ��6    5      ���� 5      $ 	LightBlue��6T   $ .4Q�6f   (��       6"   * SystemColors.HighlightBrushKey+)6f   �� 5      $ 	LightBlue��6R   $ .4Q�6d   (��       6"   *SystemColors.ControlBrushKey+)6d   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��32 �6   6*   �� 5      $��1* �6   6*   �� 5      $��32 �6   6*   �� 5      $��18 �6   6*   .+�5   
   �� $��0��6   .��5      �  � ColumnDefinitions 5      �� 5      $��50 �6'   64   �� 5      $��4* �6'   64   �� 5      $��50 �6'   64   �� 5       $��4* �6'   64   �� 5!      $��60 �6'   64   �� 5"      $��60 �6'   64   .+�5$      �� $��0��6    �� TextAlignment$ Center=�6,   	 �� Padding$	 0 5 0 0q�6C   	分类：6Z   �� 5%      -   5        $��cagetoryCombo��5%   +   $��1��6   
 �� DisplayMemberPath$	
 Key��6@    �� SelectedValuePath$ Value��6X   6t   �� 5&      $��2��6   $ Center=�6,   $	 0 5 0 0q�6C   关键字：6[   �� 5'      -   5        $��KeyWord��5'      
���   �$��3��6=   6O   �� 5(      -   5        $��4��5(      $��1q�6)   .��6I   	查 找6O   �� 5)      -   5        $��5��5)      $��1q�6)   .��6R   	下 载6X   5*      5+      �� 5,   
   $
��Auto=�6   $
��Auto��6:   $��1��6G   .��5-      �� -   5        $��mainListBox��5-      $
��Auto��6)   ��5.      �� 5/       �� 
TargetType" ��6&   .R�50      �  � Orientation$ 
Horizontal=�6$    K� IsItemsHost . 6=   51      ��53      �� 54      .R�55      � $	��180��6   $	��220��6+    56      �� 57   "   $	��170 �60   6?   �� 58   "   $��35 �60   6>   �� 59   "   $��15 �60   6>   .+�5;      �� $��0��6$   ��5<   "   `� 5=   &    `� 	Converter# [  63   .1�5>   *   ��  �� Path$ Cover�62   �� 5?   *   $
 Name�62   5@   '   5B      �� 5C      $
��Wrap=�6C   $��1��6W   ��6(   �� $
 Name�6f   �� 5D      $��2��6G   ��6(   �� $ Cagetory�6V   5E      5F      5H      5I      �� 5J   
   -   5        $��StepList��5J      $��2��6   $��Disabled=�6/   $��Disabled=�6e   $
 
showString��6�   $ pageId��6�   ��5K      �� 5L      .R�5M      � 6"   5N      5U      � 5V   
   -   5        $��downloadPro��5V   #   $��3��6   6#   5W      5X      