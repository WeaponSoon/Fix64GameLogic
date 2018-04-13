This project implements a simple game logic framework that uses fixed point numbers:
1. The section in the Fixed64 folder uses the code in FixedMath.Net and modifies the sections that are not supported in older versions of C#. This part implements a fixed-point number algorithm;
2. The entire framework is located in the SWPLogicLayerF namespace. The SFrameWork class is used to manage the entire framework, mainly for the management of the SGameObject lifecycle. Before creating any SGameObject object, you must first call SFrameWork.InitFramework() to initialize the framework;
3. The loop calls SFrameWork.Update(). All SGameObjects and their own SComponents will be updated. Calling SFrameWork.Update() once is called a frame;
4. SComponent is a component class. Components will only be updated when they are loaded into SGameObject. Inherit SComponent and override Update, Start, and OnDestroy methods to implement their own functions;
5. SGameObject is a game object class. Each frame traverses all SGameObjects and updates them.
6. At the beginning of each frame, it will first process the SGameObject object destroyed and created on the previous frame, then iterate over all SGameObjects, perform updates for all its components, and so on;
7. The physical part has not yet been implemented.

Use this framework as follows:
1. Invoke SFrameWork.InitFramework() to initialize the framework;
2. Create some preset SGameObjects (if needed) and add some SComponents to them (if needed). Of course you should have at least one preset SGameObject and add SComponent to dynamically generate other SGameObjects in subsequent Updates;
3. loop call SFrameWork.Update ();
4. Call SFrameWork.UnintFramework() to destroy the framework.


此工程仿照Unity实现了一个简单的游戏逻辑框架：
1. Fixed64文件夹中的部分使用了FixedMath.Net中的代码，并修改了其中在旧版本的C#中不支持的部分。这一部分实现了定点数算法；
2. 整个框架全部位于SWPLogicLayerF命名空间中，SFrameWork类用于管理整个框架，主要为SGameObject生命周期的管理。在创建任何SGameObject对象之前必须首先调用SFrameWork.InitFramework()初始化框架；
3. 循环调用SFrameWork.Update()，所有的SGameObject以及它们所拥有的SComponent都会被更新。调用SFrameWork.Update()一次则称为一帧；
4. SComponent为组件类，组件只有被加载到SGameObject上时才会被更新，继承SComponent并重写Update、Start和OnDestroy等方法以实现自己的功能；
5. SGameObject为游戏对象类，每一帧都会遍历所有的SGameObject并更新；
6. 每一帧的开始首先会处理上一帧销毁和创建的SGameObject对象，然后遍历所有的SGameObject，执行它们的所有组件的Update等等方法;
7. 物理部分尚未实现。

按照以下流程使用此框架：
1. 调用SFrameWork.InitFramework()初始化框架；
2. 创建一些预置SGameObject（如果需要）并给它们添加一些SComponent（如果需要），当然你应该至少有一个预置SGameObject并添加SComponent以用来在后面的Update中动态生成其他SGameObject；
3. 循环调用SFrameWork.Update()；
4. 调用SFrameWork.UnintFramework()用来销毁框架。
