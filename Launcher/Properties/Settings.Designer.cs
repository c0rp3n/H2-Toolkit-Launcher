﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace H2CodezLauncher.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string h2codez_dll_version {
            get {
                return ((string)(this["h2codez_dll_version"]));
            }
            set {
                this["h2codez_dll_version"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ci.appveyor.com/api/projects/num0005/h2-toolkit-launcher/artifacts/versio" +
            "n")]
        public string version_url {
            get {
                return ((string)(this["version_url"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ci.appveyor.com/api/projects/num0005/h2-toolkit-launcher/artifacts/Launch" +
            "er/bin/Release/H2CodezLauncher.exe")]
        public string launcher_update_url {
            get {
                return ((string)(this["launcher_update_url"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ci.appveyor.com/api/projects/num0005/h2codez/artifacts/Release/H2Codez.dl" +
            "l")]
        public string h2codez_update_url {
            get {
                return ((string)(this["h2codez_update_url"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ci.appveyor.com/api/projects/num0005/h2codez/artifacts/version")]
        public string h2codez_version_url {
            get {
                return ((string)(this["h2codez_version_url"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ci.appveyor.com/api/projects/num0005/h2codez/artifacts/Patches/")]
        public string patch_fetch_url {
            get {
                return ((string)(this["patch_fetch_url"]));
            }
        }
    }
}
