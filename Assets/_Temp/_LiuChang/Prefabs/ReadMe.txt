CameraTest使用说明：

1.用途：用作测试场景对于镜头是否穿帮

2.用法：

	(1).将CameraTest.unitypackage导入到unity中。

	(2).打开要测试的场景，在场景中加入Assets\_Temp\_LiuChang\Prefabs\CameraTest.prefab。

	(3).将场景中地表的世界坐标归零。

	(4).移动CameraTest\TroopPanel\Self\Player_1_HaoJie至场景入口处的地面上。

	(5).找到CameraTest\Main Camera上的脚本"Camera Test Camera"，设置section(章)和level(节)。例如调试关卡205，section设置为2，level设置为5。

	(6).拷贝摄像机配置文件到Assets\Resources\_Data\BattleField\BattleFlags目录下。例如关卡205的摄像机配置文件名为"Camera_100205.xml"。

	(6).运行。

	(7).键盘的wasd分别用来控制人物的前进方向。

3.注意：

	(1).测试之后的场景导出的包中不应包含摄像机测试的相关文件。

	(2).将场景中地表的世界坐标归零。否则摄像机配置文件有可能不起作用。

	(3).用法实例:Assets\_Temp\_LiuChang\Scenes\CameraTest.unity。

	(4).有问题请联系刘畅。
