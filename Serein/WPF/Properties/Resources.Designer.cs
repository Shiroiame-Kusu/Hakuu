﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Serein.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Serein.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
        public static string buildinfo {
            get {
                return ResourceManager.GetString("BuildInfo" , resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///    &lt;head&gt;
        ///        &lt;meta charset=&quot;UTF-8&quot; /&gt;
        ///        &lt;meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;IE=edge&quot; /&gt;
        ///        &lt;title&gt;Console&lt;/title&gt;
        ///        &lt;meta
        ///            name=&quot;description&quot;
        ///            itemprop=&quot;description&quot;
        ///            content=&quot;Serein https://github.com/Zaitonn/Serein&quot;
        ///        /&gt;
        ///        &lt;script src=&quot;console.js&quot; type=&quot;text/javascript&quot;&gt;&lt;/script&gt;
        ///        &lt;link rel=&quot;stylesheet&quot; href=&quot;preset.css&quot; /&gt;
        ///        &lt;link rel=&quot;stylesheet&quot; href=&quot;vanilla.css&quot; /&gt;
        ///        &lt;li [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string console_html {
            get {
                return ResourceManager.GetString("console_html", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &quot;use strict&quot;;
        ///
        ///var line = 0;
        ///function AppendText(str) {
        ///    var ConsoleDiv = document.querySelector(&quot;#console&quot;);
        ///    line = line + 1;
        ///    if (str == &quot;#clear&quot;) {
        ///        Clear();
        ///    } else {
        ///        var div = document.createElement(&quot;div&quot;);
        ///        div.innerHTML = str;
        ///        ConsoleDiv.appendChild(div);
        ///    }
        ///    if (line &gt; 250) {
        ///        ConsoleDiv.removeChild(ConsoleDiv.children[0]);
        ///        line = line - 1;
        ///    }
        ///    ConsoleDiv.scrollTop = ConsoleDiv.scrollHeight;
        ///}
        ///function Clear() { [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string console_js {
            get {
                return ResourceManager.GetString("console_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 /*
        ///  用于自定义特殊高亮样式
        ///  - 需以 &quot;.noColored&gt;&quot; 开头
        ///  - 用&quot;.[NAME]&quot;指定高亮对象
        ///*/
        ///.noColored &gt; .file {
        ///    color: #688292;
        ///}
        ///.noColored &gt; .server {
        ///    color: #8bd645;
        ///}
        ///.noColored &gt; .debug {
        ///    color: #865fc5;
        ///}
        ///.noColored &gt; .error {
        ///    color: #d16969;
        ///}
        ///.noColored &gt; .warn {
        ///    color: #aa5612;
        ///}
        ///.noColored &gt; .info {
        ///    color: #20b2aa;
        ///}
        ///.noColored &gt; .plugins {
        ///    color: #e4b44c;
        ///}
        ///.noColored &gt; .LiteLoader,
        ///.noColored &gt; .LiteXLoader,
        ///.noColored &gt; .LLMoney {
        ///    color: #5a93c2;
        ///}
        ///.noColore [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string preset_css {
            get {
                return ResourceManager.GetString("preset_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 /* 
        ///  颜色参考来源：https://en.wikipedia.org/wiki/ANSI_escape_code#3-bit_and_4-bit -Windows 10 Console
        ///*/
        ///.vanillaColor30,
        ///.vanillaColor40 {
        ///    color: #0c0c0c;
        ///}
        ///.vanillaColor31,
        ///.vanillaColor41 {
        ///    color: #c50f1f;
        ///}
        ///.vanillaColor32,
        ///.vanillaColor42 {
        ///    color: #13a10e;
        ///}
        ///.vanillaColor33,
        ///.vanillaColor43 {
        ///    color: #c19c00;
        ///}
        ///.vanillaColor34,
        ///.vanillaColor44 {
        ///    color: #0037da;
        ///}
        ///.vanillaColor35,
        ///.vanillaColor45 {
        ///    color: #881798;
        ///}
        ///.vanillaColor36,
        ///.vanillaColor46 {
        ///    col [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string vanilla_css {
            get {
                return ResourceManager.GetString("vanilla_css", resourceCulture);
            }
        }
    }
}
