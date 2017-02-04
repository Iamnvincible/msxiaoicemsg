# msxiaoicemsg
- talk with ms-xiaoice through the programm without using the weibo app.

# instructions
- 1 you need to follow ms-xiaoice on Sina Weibo,and adopt her at [here](http://www.msxiaoice.com/)
- 2 after that open Chrome and navigate to the message page
- 3 if you have never messaged her try send a message on the web page on your Chrome
- 4 if you have, just navigate to the message page
- 5 if you are to run the UWP sample, you may just finish instrution 1.

# run the console sample
- clone my repo to your disk, open it in Visual Studio 2015(recommended) or higher version,
- build and run the program
- type anything you want to send to xiaoice.
- have fun :)

# run the UWP sample
- **IMPORTANT**: please clone the [JsBridge](https://github.com/Iamnvincible/JsBridge.git) I forked on [here](https://github.com/deltakosh/JsBridge), I have edited for **msxiaoiceuwp**
- Add a existing project *ChakraBridge* in the *JsBridge* solution to the current solution
- Make the **ChakraBridge** referenced to the **xiaoiceuwp** project.
- Build and deployed the **xiaoiceuwp** project.
- **IMPORTANT** if you want to run the uwp project on ARM platform, just make the Configuration to **Release** mode, there is some problem in the *ChakraBridge* when we run on ARM pltform using **Debug** mode.
- type what you want to send in the TextBox, then click **send** button, you'll be asked to login sina weibo.
- After you sent the message, the respond will be retrieved automatically, if the respond is not retrieved, you can try retrieve the respond manually. If the respond can not be manually retrieved, xiaoice may not respond you request, just check the web page.

# tips
- you can not only talk to xiaoice, but also any one who followed you,
- just mmodify the **userid**, xiaoice's id is **5175429989**

# License
- Licensed under the MIT License.

#中文说明

这个项目包括了一个控制台项目和UWP项目，用于登录新浪微博并和微软小冰进行私信。在运行之前，请确保你的新浪微博账号已经关注并收养微软小冰，并已经进行过私信。
#控制台项目
- 克隆项目后直接编译运行，在控制台输入你想要对小冰说的话，就这么简单。。。^_^

# UWP项目
- **重要**  请先克隆这个项目 [JsBridge](https://github.com/Iamnvincible/JsBridge.git) 我fork的这里 [here](https://github.com/deltakosh/JsBridge)，是个开源的js引擎，用于在UWP下直接运行javascript脚本，我对原项目做了一点修改。
- 在解决方案中添加已有项目，在JsBridge\src\ChakraBridge目录下的ChakraBridge.csproj项目文件
- 对UWP项目添加上述项目的引用
- 编译并部署到你的设备上，在文本框里输入你要对小冰说的话，还没登录会自动提示登录，如果一切顺利，会自动获取回复，当然你也可以手动获取
- **重要**  如果你要运行在ARM设备上，请把项目改成Release模式，因为这个js引擎在Debug下有问题。
# 小提示
- 项目里有个用户id，就是你要私信的对象，你可以改成任何你要私信的用户，小冰的id是**5175429989**

# 许可协议
- MIT许可证
