   M S B A M L   `   `   `  ����   ExpManagerToOwnMoney%    ExpManagerToOwnMoney.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  
MainWindow��$	��600��$
��1000��#��[B� � WindowState$ 	Maximized=� � Icon$ exp.ico��.���  � RowDefinitions �� $��* ��� $��40 ��� $��* � � ColumnDefinitions �� $��* �.+��� -   $��LvEMOperation��$��0��$��0������  � DataContext ��  �� View �� P��� $��Id�� �� Width$ 50�� �� DisplayMemberBinding �� 	 �� Path$	 Id��� $��Дата операции��$	 100�� �� $
	 Date�
 �� StringFormat$
 
dd.MM.yyyy���� $��Тип операции��$	 100�� �� $
	 Type��� $��
Сумма��$	 200�� �� $	 Summa��� $��Счёт��$	 300�� �� $	 Account��� $.��(Статья расхода/дохода��$	 200�� �� $	 Category��� $��Описание��$	 300�� �� $	 Comment��� $��1��$��5q�$
��Left=� �� Orientation$ 
Horizontal=�.+��� -   $��BtMove��$��Перенести��$	��100��$��30��$��	0 0 5px 0q��� -   $��	BtRefresh��$��Обновить��$	��100��$��30��$��	0 0 5px 0q��� -   $��LvOMOperation��$��0��$��2������  ��  �� P��� $��Id��$ 50�� �� $	 Id��� $��Дата операции��$	 100�� �� $
	 Date�$
 
dd.MM.yyyy���� $��Тип операции��$	 100�� �� $
	 Type��� $��
Сумма��$	 200�� �� $	 Summa��� $��Счёт��$	 300�� �� $	 Account��� $.��(Статья расхода/дохода��$	 200�� �� $	 Category��� $��Описание��$	 300�� �� $	 Comment�