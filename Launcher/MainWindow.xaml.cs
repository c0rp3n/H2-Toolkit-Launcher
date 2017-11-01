﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net;
using static System.Environment;
using static System.Object;
using static System.Diagnostics.Process;
using H2CodezLauncher.Properties;

namespace Halo2CodezLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string H2Ek_install_path = GetFolderPath(SpecialFolder.ProgramFilesX86) + "\\Microsoft Games\\Halo 2 Map Editor\\";
        private string Halo_install_path = GetFolderPath(SpecialFolder.ProgramFilesX86) + "\\Microsoft Games\\Halo 2\\";
        [Flags] enum level_compile_type : Byte
        {
            none = 0,
            compile = 2,
            light = 4,
        }
        level_compile_type levelCompileType;

        enum light_quality
        {
            checkerboard,
            direct_only,
            draft_low,
            draft_medium,
            draft_high,
            low,
            medium,
            high,
            super
        }

        [Flags]
        enum model_compile : Byte
        {
            none = 0,
            render = 2,
            physics = 4,
            obj = 8,
        }
        model_compile model_compile_type;

        enum object_type
        {
            biped,
            vehicle,
            weapon,
            equipment,
            garbage,
            projectile,
            scenery,
            machine,
            control,
            light_fixture,
            sound_scenery,
            crate,
            creature
        }

        enum patch_status
        {
            bad_version,
            unpatched,
            patched
        }

        [Flags]
        enum file_list : Byte
        {
            none = 0,
            tool = 2,
            sapien = 4,
            guerilla = 8
        }

        public MainWindow()
        {
            H2Ek_install_path = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Halo 2", "tools_directory", H2Ek_install_path).ToString();
            InitializeComponent();
#if !DEBUG
            var cmd_args = GetCommandLineArgs();
            if (cmd_args.Length > 1 && cmd_args[1] == "--update")
                File.Delete("H2CodezLauncher.exe.old");
            Assembly assembly = Assembly.GetExecutingAssembly();
            var wc = new WebClient();
            string our_version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            string latest_version = wc.DownloadString(Settings.Default.version_url);
            if (latest_version != our_version)
            {
                MessageBoxResult user_answer = MessageBox.Show("Latest version is: " + latest_version + " You are using: " + our_version + " \nDo you want to update?",
                     "Outdated Version!", MessageBoxButton.YesNo);
                if (user_answer == MessageBoxResult.Yes)
                {
                    wc.DownloadFile(Settings.Default.launcher_update_url, "H2CodezLauncher.exe.new");
                    ForceMove("H2CodezLauncher.exe", "H2CodezLauncher.exe.old");
                    ForceMove("H2CodezLauncher.exe.new", "H2CodezLauncher.exe");
                    Start("H2CodezLauncher.exe", "--update");
                    Exit(0);
                }
            }
            new Thread(delegate ()

            {
                file_list files_to_patch = file_list.none;
                string h2codez_version = wc.DownloadString(Settings.Default.h2codez_version_url);

                if (!check_files(ref files_to_patch))
                {
                    MessageBox.Show("You are using a version of the Toolkit not supported by H2Codez, Features added by H2Codez will not be available.",
                     "Version Error!");
                    return;
                }
                if (!File.Exists(H2Ek_install_path + "H2Codez.dll") || h2codez_version != Settings.Default.h2codez_dll_version || files_to_patch != file_list.none)
                {
                    MessageBoxResult user_answer = MessageBox.Show("Your have not installed H2Codez or your version is outdated.\nDo you want to installed H2Codez?",
                     "H2Codez Install", MessageBoxButton.YesNo);
                    if (user_answer == MessageBoxResult.No) return;

                    ApplyPatches(files_to_patch, wc);
                    wc.DownloadFile(Settings.Default.h2codez_update_url, H2Ek_install_path + "H2Codez.dll");
                    Settings.Default.h2codez_dll_version = h2codez_version;
                    Settings.Default.Save();
                }
            }).Start();
#endif
        }

        private void ForceMove(string sourceFilename, string destinationFilename)
        {
            if (File.Exists(destinationFilename))
            {
                System.IO.File.Delete(destinationFilename);
            }

            System.IO.File.Move(sourceFilename, destinationFilename);
        }
        private void RunHalo2Sapien(object sender, RoutedEventArgs e)
        {
           Start(H2Ek_install_path + "h2sapien.exe");
        }
        private void RunHalo2Guerilla(object sender, RoutedEventArgs e)
        {
            Start(H2Ek_install_path + "h2guerilla.exe");
        }
        private void RunHalo2(object sender, RoutedEventArgs e)
        {
            Start(Halo_install_path + "halo2.exe");
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private bool check_files(ref file_list files_to_patch)
        {
            string h2tool = CalculateMD5(H2Ek_install_path + "h2tool.exe");
            if (h2tool == "dc221ca8c917a1975d6b3dd035d2f862")
                files_to_patch |= file_list.tool;
            else if (h2tool != "f81c24da93ce8d114caa8ba0a21c7a63")
                return false;

            string h2sapien = CalculateMD5(H2Ek_install_path + "h2sapien.exe");
            if (h2sapien == "d86c488b7c8f64b86f90c732af01bf50")
                files_to_patch |= file_list.sapien;
            else if (h2sapien != "975c0d0ad45c1687d11d7d3fdfb778b8")
                return false;

            string h2guerilla = CalculateMD5(H2Ek_install_path + "h2guerilla.exe");
            if (h2guerilla == "ce3803cc90e260b3dc59854d89b3ea88")
                files_to_patch |= file_list.guerilla;
            else if (h2guerilla != "55b09d5a6c8ecd86988a5c0f4d59d7ea")
                return false;

            return true;
        }

        private void ApplyPatches(file_list files_to_patch, WebClient wc)
        {
            Directory.CreateDirectory(H2Ek_install_path + "backup");
            if (files_to_patch.HasFlag(file_list.tool))
                patch_file("h2tool.exe", wc);
            if (files_to_patch.HasFlag(file_list.guerilla))
                patch_file("h2guerilla.exe", wc);
            if (files_to_patch.HasFlag(file_list.sapien))
                patch_file("h2sapien.exe", wc);
        }

        private void patch_file(string name, WebClient wc)
        {
            byte[] patch_data = wc.DownloadData(Settings.Default.patch_fetch_url + name + ".patch");
            using (FileStream unpatched_file = new FileStream(H2Ek_install_path + name, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream patched_file = new FileStream(H2Ek_install_path + name + ".patched", FileMode.Create))
                BsDiff.BinaryPatchUtility.Apply(unpatched_file, () => new MemoryStream(patch_data), patched_file);
            ForceMove(H2Ek_install_path + name, H2Ek_install_path + "backup\\" + name);
            ForceMove(H2Ek_install_path + name + ".patched", H2Ek_install_path + name);
        }

        private void ResetSapienDisplay(object sender, RoutedEventArgs e)
        {
            RegistryKey myKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\bungie\\sapien", true);
            if (myKey != null)
            {
                myKey.DeleteValue("sapien_loading_window_rect", false);
                myKey.DeleteValue("warnings_window_rect", false);
                myKey.DeleteValue("main_window_frame", false);
                myKey.DeleteValue("output_window_rect", false);
                myKey.DeleteValue("options_window_rect", false);
                myKey.DeleteValue("hierarchy_window_rect", false);
                myKey.DeleteValue("property_window_rect", false);
                myKey.DeleteValue("main_window_rect", false);
                myKey.DeleteValue("type_list_rect", false);
                myKey.Close();
            }
        }

        private void HandleClickCompile(object sender, RoutedEventArgs e)
        {
            string level_path = compile_level_path.Text;
            if (File.Exists(level_path))
            {
                light_quality light_level = (light_quality)light_quality_level.SelectedIndex;
                new Thread(delegate ()
                {
                    CompileLevel(level_path.ToLower(), light_level);
                }).Start();
            }
            else
            {
                MessageBox.Show("Error: No such file!");
            }
        }

        private void CompileLevel(string level_path, light_quality lightQuality)
        {
            if (levelCompileType.HasFlag(level_compile_type.compile))
            {
                string command = (level_path .Contains(".ass") ? "structure-new-from-ass" : "structure-new-from-jms");
                var proc = Start(H2Ek_install_path + "h2tool.exe", command + " \"" + level_path + "\" yes");
                proc.WaitForExit(-1);
            }
            if (levelCompileType.HasFlag(level_compile_type.light))
            {
                level_path = level_path.Replace(".ass", "");
                level_path = level_path.Replace(".jms", "");
                level_path = level_path.Replace("\\data\\", "\\tags\\");
                level_path = level_path.Replace("\\structure\\", "\\");
                Start(H2Ek_install_path + "h2tool.exe", "lightmaps \"" + level_path + "\" " 
                    + System.IO.Path.GetFileNameWithoutExtension(level_path) + " " + lightQuality);
                System.IO.Path.GetFileNameWithoutExtension(level_path);
            }
        }

        private void CompileText(object sender, RoutedEventArgs e)
        {
            string text_path = compile_text_path.Text;
            if (File.Exists(text_path))
            {
                string path = new FileInfo(text_path).Directory.FullName;
                Start(H2Ek_install_path + "h2tool.exe", "new-strings \"" + path + "\"");
            }
            else
            {
                MessageBox.Show("Error: No such file!");
            }
        }

        private void CompileImage(object sender, RoutedEventArgs e)
        {
            string image_path = compile_image_path.Text;
            if (File.Exists(image_path))
            {
                string path = new FileInfo(image_path).Directory.FullName;
                Start(H2Ek_install_path + "h2tool.exe", "bitmaps \"" + path + "\"");
            }
            else
            {
                MessageBox.Show("Error: No such file!");
            }
        }

        private void PackageLevel(object sender, RoutedEventArgs e)
        {
            string level_path = package_level_path.Text;
            if (File.Exists(level_path))
            {
                level_path = level_path.Replace(".scenario", "");
                Start(H2Ek_install_path + "h2tool.exe", "build-cache-file \"" + level_path + "\"");
            }
            else
            {
                MessageBox.Show("Error: No such file!");
            }
        }

        private void CompileOnly_Checked(object sender, RoutedEventArgs e)
        {
            levelCompileType = level_compile_type.compile;
            light_quality_select_box.IsEnabled = false;
        }

        private void LightOnly_Checked(object sender, RoutedEventArgs e)
        {
            levelCompileType = level_compile_type.light;
            light_quality_select_box.IsEnabled = true;
        }

        private void CompileAndLight_Checked(object sender, RoutedEventArgs e)
        {
            levelCompileType = level_compile_type.compile | level_compile_type.light;
            light_quality_select_box.IsEnabled = true;
        }

        private void browse_level_compile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Uncompiled level";
            dlg.Filter = "Uncompiled map geometry|*.ASS;*.JMS";
            if (dlg.ShowDialog() == true)
            {
                compile_level_path.Text = dlg.FileName;
            }
        }

        private void Browse_text_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select unicode encoded .txt file to compile.";
            dlg.Filter = "Unicode encoded .txt files|*.txt";
            if (dlg.ShowDialog() == true)
            {
                compile_text_path.Text = dlg.FileName;
            }
        }

        private void browse_bitmap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Image File";
            dlg.Filter = "Supported image files|*.tif;*.tga;*.jpg;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                compile_image_path.Text = dlg.FileName;
            }
        }

        private void browse_package_level_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select Scenario";
            dlg.Filter = "Unpackaged Map|*.scenario";
            if (dlg.ShowDialog() == true)
            {
                package_level_path.Text = dlg.FileName;
            }
        }

        private void run_cmd_Click(object sender, RoutedEventArgs e)
        {
            Start("cmd", "/K \"cd /d \"" + H2Ek_install_path + "\"");
        }

        private void custom_h2tool_cmd_Click(object sender, RoutedEventArgs e)
        {
            Custom_Command.Visibility = Visibility.Visible;
        }

        private void custom_cancel_Click(object sender, RoutedEventArgs e)
        {
            Custom_Command.Visibility = Visibility.Collapsed;
            custom_command_text.Text = "";
        }

        private void custom_run_Click(object sender, RoutedEventArgs e)
        {
            Custom_Command.Visibility = Visibility.Collapsed;
            Start(H2Ek_install_path + "h2tool.exe", custom_command_text.Text);
            custom_command_text.Text = "";
        }

        private void compile_model_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Annoy num0005#8646 on discord if you want this to work.");
        }

        private void browse_model_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Annoy num0005#8646 on discord if you want this to work.");
        }

        private void model_compile_render_Checked(object sender, RoutedEventArgs e)
        {
            model_compile_type = model_compile.render;
            model_compile_obj_type.IsEnabled = false;
        }

        private void model_compile_physics_Checked(object sender, RoutedEventArgs e)
        {
            model_compile_type = model_compile.physics;
            model_compile_obj_type.IsEnabled = false;
        }

        private void model_compile_obj_Checked(object sender, RoutedEventArgs e)
        {
            model_compile_type = model_compile.obj;
            model_compile_obj_type.IsEnabled = true;
        }

        private void model_compile_all_Checked(object sender, RoutedEventArgs e)
        {
            model_compile_type = model_compile.render | model_compile.physics | model_compile.obj;
            model_compile_obj_type.IsEnabled = true;
        }
    }

    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null 
            // (to avoid crash bugs for views in the designer) 
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
