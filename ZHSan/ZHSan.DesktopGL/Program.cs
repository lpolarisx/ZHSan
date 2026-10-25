using Platforms;
using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Runtime.InteropServices;

namespace WorldOfTheThreeKingdoms;

public static class Program
{
    private const uint SDL_MESSAGEBOX_ERROR = 0x00000010;

    [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_ShowSimpleMessageBox(uint flags,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string message,
        IntPtr window);

    [STAThread]
    static void Main()
    {

        Console.OutputEncoding = Encoding.UTF8;
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        /*bool flag;
        Mutex mutex = new Mutex(true, "WorldOfTheThreeKingdoms", out flag);
        if (!flag)
        {
            MessageBox.Show("游戏已经在运行中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        else
        {
            mutex.ReleaseMutex();
            new MainProcessManager().Processing();
        }*/

        // 配置Seriol日志
        var logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";

        // 挂着调试器（VSCode调试）时输出全部日志；直接运行exe（包括发给别人的Debug目录）只记警告以上
        var minimumLevel = System.Diagnostics.Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Warning;

        Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Is(minimumLevel)
                            .WriteTo.Console(outputTemplate: logTemplate)
                            .WriteTo.File("logs/game.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3, outputTemplate: logTemplate)
                            .CreateLogger();

        try
        {
            if (Platform.PlatFormType == PlatFormType.Win || Platform.PlatFormType == PlatFormType.Desktop)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    using MainGame game = new MainGame();
                    game.Run();
                }
                else
                {
                    try
                    {
                        using MainGame game = new MainGame();
                        game.Run();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "游戏启动或运行失败");

                        var title = "游戏错误";
                        var message = "中华三国志遇到严重错误，请提交游戏目录/logs/下的日志文件。";
                        try
                        {
                            // MonoGame的MessageBox按系统代码页(GBK)传给SDL，中文会乱码，这里直接按UTF-8调用SDL
                            SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, title, message, IntPtr.Zero);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(title, message, ["OK"]);
                        }
                    }
                }
            }
            else if (Platform.PlatFormType == PlatFormType.UWP)
            {
                Platform.Current.OpenFactory();
            }
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
